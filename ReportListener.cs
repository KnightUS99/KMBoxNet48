using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using KMBoxNet48.Structures;

namespace KMBoxNet48
{
    /// <summary>
    /// Report listener. <br/>
    /// Allows listening to keyboard/mouse inputs.
    /// </summary>
    public sealed class ReportListener : IDisposable
    {
        /// <summary>
        /// Is this listener stopped?
        /// </summary>
        public bool Stopped { get; private set; }

        /// <summary>
        /// Your custom event handler goes here.
        /// </summary>
        public Action<CompositeReport> EventListener { get; set; }

        private readonly KmBoxClient _client;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Thread _listenerThread;
        private ManualResetEventSlim _resetEvent;
        private UdpClient _udpClient;

        internal ReportListener(KmBoxClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Starts listening to incoming events and enables 'monitor' mode.
        /// </summary>
        /// <param name="resetEvent">Custom event which will be signalled when <see cref="ReportListener"/> will stop.</param>
        /// <exception cref="InvalidOperationException">This listener is already active!</exception>
        public void Start(ManualResetEventSlim resetEvent = null)
        {
            if (_listenerThread != null && _listenerThread.IsAlive)
                throw new InvalidOperationException("Listener is already running!");

            _resetEvent = resetEvent;
            Stopped = false;

            _listenerThread = new Thread(ListenerThread)
            {
                IsBackground = true
            };
            _listenerThread.Start();
        }

        private void ListenerThread()
        {
            try
            {
                var localEndpoint = new IPEndPoint(IPAddress.Any, _client.Port + 1);
                _udpClient = new UdpClient(localEndpoint);

                _client.EnableMonitor(true).Wait();

                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        byte[] buffer = _udpClient.Receive(ref remoteEndpoint);
                        var report = StructHelper.ByteArrayToStruct<CompositeReport>(buffer);

                        EventListener?.Invoke(report);
                    }
                    catch (SocketException ex)
                    {
                        if (_cancellationTokenSource.IsCancellationRequested)
                            break;
                        // Handle other socket errors if needed
                        Thread.Sleep(100); // Prevent tight loop on error
                    }
                }
            }
            catch (Exception)
            {
                // Handle or log exceptions as needed
            }
            finally
            {
                if (_udpClient != null)
                {
                    _udpClient.Close();
                    _udpClient = null;
                }
                Stopped = true;
                _resetEvent?.Set();
            }
        }

        /// <summary>
        /// Stop listening and disable 'monitor' mode.
        /// </summary>
        public void Stop()
        {
            if (Stopped)
                return;

            try
            {
                _client.EnableMonitor(false).Wait();
                _cancellationTokenSource.Cancel();

                if (_udpClient != null)
                {
                    _udpClient.Close();
                }

                if (_listenerThread != null && _listenerThread.IsAlive)
                {
                    _listenerThread.Join(1000); // Wait up to 1 second for thread to exit
                }
            }
            finally
            {
                Stopped = true;
            }
        }

        /// <summary>
        /// Same as <see cref="Stop"/>.
        /// </summary>
        public void Dispose()
        {
            Stop();
            _cancellationTokenSource.Dispose();
        }
    }
}
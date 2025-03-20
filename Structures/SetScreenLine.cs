using System.Runtime.InteropServices;

namespace KMBoxNet48.Structure
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ScreenLineBuffer
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] Buffer;
    }
}

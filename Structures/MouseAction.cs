﻿using System.Runtime.InteropServices;

namespace KMBox.NET.Structures
{


    /// <summary>
    /// Mouse action.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MouseAction
    {

        /// <summary>
        /// Active buttons. Can be set using <see cref="MouseButton"/> flags.
        /// </summary>
        [FieldOffset(0)]
        public int Buttons;

        /// <summary>
        /// Movement on X axis. Can be positive or negative. Always relative.
        /// </summary>
        [FieldOffset(4)]
        public int X;

        /// <summary>
        /// Movement on Y axis. Can be positive or negative. Always relative.
        /// </summary>
        [FieldOffset(8)]
        public int Y;

        /// <summary>
        /// Mouse wheel scroll. Can be positive or negative.
        /// </summary>
        [FieldOffset(12)]
        public int Wheel;

        /// <summary>
        /// Points where to move mouse. Different types of movement expect different values here.
        /// </summary>
        [FieldOffset(16)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public int[] Points;

    }
}
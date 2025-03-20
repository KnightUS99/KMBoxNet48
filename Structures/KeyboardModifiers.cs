﻿
using System;

namespace KMBoxNet48.Structures
{

    /// <summary>
    /// Keyboard modifiers.
    /// </summary>
    [Flags]
    public enum KeyboardModifiers : byte
    {
        LeftСontrol = 0x01,
        LeftShift = 0x02,
        LeftAlt = 0x04,
        LeftGui = 0x08,
        RightControl = 0x10,
        RightShift = 0x20,
        RightAlt = 0x40,
        RightGui = 0x80
    }

}

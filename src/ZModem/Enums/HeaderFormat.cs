using System;

namespace ZModem_Protocol
{
    /// <summary> Header Format for ZModem protocol</summary>
    enum HeaderFormat
    {
        /// <summary> Hex Header</summary>
        Hex = 0,
        /// <summary> 16bit CRC Binary Header </summary>
        Bin16 = 1,
        /// <summary> 32bit CRC Binary Header </summary>
        Bin32 = 2
    }
}
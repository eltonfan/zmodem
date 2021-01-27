using System;

namespace ZModem_Protocol
{
    /// <summary> Parameters for ZSINIT Header, in ZF0 </summary>
    enum ZSINIT_Parameter
    {
        /// <summary> Transmitter expects ctl chars to be escaped</summary>
        TESCCTL = 64,
        /// <summary> Transmitter expects 8th bit to be escaped </summary>
        TESC8 = 128
    }
}
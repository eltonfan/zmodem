using System;

namespace ZModem_Protocol
{
    /// <summary> Bit Masks for ZRINIT header, one of these ored in ZF0 </summary>
    [Flags]
    enum ZRINIT_Header_ZF0
    {
        /// <summary> Rx can send and receive true FDX </summary>
        CANFULLDUPLEX = 0x1,
        /// <summary> Rx can receive data during disk I/O </summary>
        CANOVERLAPIO = 0x2,
        /// <summary> Rx can send a break signal </summary>
        CANBREAK = 0x4,
        /// <summary> Receiver can decrypt </summary>
        CANDECRYPT = 0x10,
        /// <summary> Receiver can uncompress </summary>
        CANLZW = 0x20,
        /// <summary> Receiver can use 32 bit Frame Check </summary>
        CANCRC32 = 0x40,
        /// <summary> Receiver expects ctl chars to be escaped </summary>
        ESCAPECONTOL = 0x100,
        /// <summary> Receiver expects 8th bit to be escaped </summary>
        ESCAPE8BIT = 0x200
    }
}
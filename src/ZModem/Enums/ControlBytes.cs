using System;

namespace ZModem_Protocol
{
    /// <summary> Control Bytes used for ZModem protocol </summary>
    enum ControlBytes
    {
        /// <summary> Padding char begins all frames ('*' = 42 = O52)</summary>
        ZPAD = (int)'*',
        /// <summary> Ctrl-X Zmodem escape char (24 = O30)</summary>
        ZDLE = 24,
        /// <summary> Escaped ZDLE as transmitted (88 = O30^O100)</summary>
        ZDLEE = 88,
        /// <summary> Binary frame indicator ('A')</summary>
        ZBIN = (int)'A',
        /// <summary> HEX frame indicator ('B')</summary>
        ZHEX = (int)'B',
        /// <summary> Binary frame with 32bits FCS indicator ('C')</summary>
        ZBINC = (int)'C',

        /// <summary> XON caracter (17)</summary>
        XON = 0x011,//'Q'& 037;
        /// <summary> XOFF caracter (19)</summary>
        XOFF = 0x013,//'S'& 037;
        /// <summary> CR caracter (13)</summary>
        CR = 0x0d,
        /// <summary> LF caracter (10)</summary>
        LF = 0x0a
    }
}
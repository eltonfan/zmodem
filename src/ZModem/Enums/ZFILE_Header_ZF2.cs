using System;

namespace ZModem_Protocol
{
    /// <summary> Management options for ZFILE Header, one of these in ZF2 (Transport options)</summary>
    enum ZFILE_Header_ZF2
    {
        /// <summary> No option = sender default option </summary>
        NOTHING = 0,
        /// <summary> Lempel-Ziv compression </summary>
        ZTLZW = 1,
        /// <summary> Encryption </summary>
        ZTCRYPT = 2,
        /// <summary> Run Length encoding </summary>
        ZTRLE = 3
    }
}
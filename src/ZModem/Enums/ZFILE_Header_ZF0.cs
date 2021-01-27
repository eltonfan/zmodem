using System;

namespace ZModem_Protocol
{
    /// <summary> Parameters for ZFILE frame : one of these in ZF0 (Conversion options) </summary>
    enum ZFILE_Header_ZF0
    {
        /// <summary> No option = sender default option.</summary> 
        NOTHING = 0,
        /// <summary> Binary transfer - inhibit conversion</summary> 
        ZCBIN = 1,
        /// <summary> Convert NL to local end of line convention </summary>
        ZCNL = 2,
        /// <summary> Resume interrupted file transfer </summary>
        ZCRESUM = 3
    }
}
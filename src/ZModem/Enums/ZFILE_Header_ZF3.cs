using System;

namespace ZModem_Protocol
{
    /// <summary> Management options for ZFILE Header, one of these in ZF3 (Extended options)</summary>
    enum ZFILE_Header_ZF3
    {
        /// <summary> No option = sender default option </summary>
        NOTHING = 0,
        /// <summary> Encoding for sparse file operations  </summary>
        ZXSPARS = 64
    }
}
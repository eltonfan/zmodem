using System;

namespace ZModem_Protocol
{
    /// <summary> Management options for ZFILE Header, one of these ored in ZF1 (Management options)</summary>
    [Flags]
    enum ZFILE_Header_ZF1
    {
        /// <summary> No option = sender default option</summary>
        NOTHING = 0,
        /// <summary> Skip file if not present at rx (O200)</summary>
        ZMSKNOLOC = 128,
        /// <summary> Mask for the choices below (O37) </summary>
        ZMMASK = 31,
        /// <summary> Transfer if source newer or longer </summary>
        ZMNEWL = 1,
        /// <summary> Transfer if different file CRC or length </summary>
        ZMCRC = 2,
        /// <summary> Append contents to existing file (if any) </summary>
        ZMAPND = 3,
        /// <summary> Replace existing file </summary>
        ZMCLOB = 4,
        /// <summary> Transfer if source newer </summary>
        ZMNEW = 5,
        /// <summary> Transfer if dates or lengths different </summary>
        ZMDIFF = 6,
        /// <summary> Protect destination file </summary>
        ZMPROT = 7
    }
}
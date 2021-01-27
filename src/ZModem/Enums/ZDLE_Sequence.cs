using System;

namespace ZModem_Protocol
{
    /// <summary> ZDLE sequences </summary>
    enum ZDLE_Sequence
    {
        /// <summary> CRC next, frame ends, header packet follows </summary>
        ZCRCE = (int)'h',
        /// <summary> CRC next, frame continues nonstop </summary>
        ZCRCG = (int)'i',
        /// <summary> CRC next, frame continues, ZACK expected </summary>
        ZCRCQ = (int)'j',
        /// <summary> CRC next, ZACK expected, end of frame </summary>
        ZCRCW = (int)'k',
        /// <summary> Translate to rubout 0177 </summary>
        ZRUB0 = (int)'l',
        /// <summary> Translate to rubout 0377 </summary>
        ZRUB1 = (int)'m',
    }
}
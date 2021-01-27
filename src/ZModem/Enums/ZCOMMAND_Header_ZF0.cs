using System;

namespace ZModem_Protocol
{
    /// <summary> Parameters for ZCOMMAND Header, one of these in ZF0 (otherwise 0) </summary>
    enum ZCOMMAND_Header_ZF0
    {
        /// <summary> Acknowledge, then do command </summary>
        ZCACK1 = 1
    }

}
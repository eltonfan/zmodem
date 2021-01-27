using System;

namespace ZModem_Protocol
{
    /// <summary> Header types for ZModem protocol</summary>
    enum HeaderType
    {
        /// <summary> Request receive init </summary>
        ZRQINIT = 0,
        /// <summary> Receive init </summary>
        ZRINIT = 1,
        /// <summary>Send init sequence (optional)</summary>
        ZSINIT = 2,
        /// <summary> ACK to above </summary>
        ZACK = 3,
        /// <summary> File name from sender </summary>
        ZFILE = 4,
        /// <summary> To sender: skip this file </summary>
        ZSKIP = 5,
        /// <summary> Last packet was garbled </summary>
        ZNAK = 6,
        /// <summary>  Abort batch transfers </summary>
        ZABORT = 7,
        /// <summary> Finish session </summary>
        ZFIN = 8,
        /// <summary>  Resume data transmition at this position </summary>
        ZRPOS = 9,
        /// <summary> Data packet(s) follow </summary>
        ZDATA = 10,
        /// <summary>  End of file </summary>
        ZEOF = 11,
        /// <summary> Fatal Read or Write error Detected </summary>
        ZFERR = 12,
        /// <summary> Request for file CRC and response </summary>
        ZCRC = 13,
        /// <summary> Receiver's Challenge </summary>
        ZCHALLENGE = 14,
        /// <summary> Request is complete </summary>
        ZCOMPL = 15,
        /// <summary> Other end canned session with CAN*5 </summary>
        ZCAN = 16,
        /// <summary> Request for free bytes on filesystem </summary>
        ZFREECNT = 17,
        /// <summary> Command from sending program </summary>
        ZCOMMAND = 18,
        /// <summary> Output to standard error, data follows </summary>
        ZESTERR = 19
    }
}
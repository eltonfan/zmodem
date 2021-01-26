using System;

namespace ZModem_Protocol
{
    public partial class ZModem
    {
        /// <summary> Header types for ZModem protocol</summary>
        public enum HeaderType
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
        };

        /// <summary> Bit Masks for ZRINIT header, one of these ored in ZF0 </summary>
        [FlagsAttribute]
        public enum ZRINIT_Header_ZF0
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

        /// <summary> Parameters for ZSINIT Header, in ZF0 </summary>
        public enum ZSINIT_Parameter
        {
            /// <summary> Transmitter expects ctl chars to be escaped</summary>
            TESCCTL = 64,
            /// <summary> Transmitter expects 8th bit to be escaped </summary>
            TESC8 = 128
        }

        /// <summary> Parameters for ZSINIT frame: Max length of attention string = 32 </summary>
        public static int ZATTNLEN = 32;

        /// <summary> Parameters for ZFILE frame : one of these in ZF0 (Conversion options) </summary>
        public enum ZFILE_Header_ZF0
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

        /// <summary> Management options for ZFILE Header, one of these ored in ZF1 (Management options)</summary>
        [FlagsAttribute]
        public enum ZFILE_Header_ZF1
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

        /// <summary> Management options for ZFILE Header, one of these in ZF2 (Transport options)</summary>
        public enum ZFILE_Header_ZF2
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

        /// <summary> Management options for ZFILE Header, one of these in ZF3 (Extended options)</summary>
        public enum ZFILE_Header_ZF3
        {
            /// <summary> No option = sender default option </summary>
            NOTHING = 0,
            /// <summary> Encoding for sparse file operations  </summary>
            ZXSPARS = 64
        }

        /// <summary> Parameters for ZCOMMAND Header, one of these in ZF0 (otherwise 0) </summary>
        public enum ZCOMMAND_Header_ZF0
        {
            /// <summary> Acknowledge, then do command </summary>
            ZCACK1 = 1
        }

        /// <summary> Control Bytes used for ZModem protocol </summary>
        public enum ControlBytes
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

        /// <summary> ZDLE sequences </summary>
        public enum ZDLE_Sequence
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



        //Non implémenté car pas d'utilité en ce moment
        ///* zdlread return values (internal) */
        ///* -1 is general error, -2 is timeout */
        //public static string GOTOR = "0400";
        //public static string GOTCRCE = "h|0400";//(ZCRCE | GOTOR);	/* ZDLE-ZCRCE received */
        //public static string GOTCRCG = "i|0400";//(ZCRCG|GOTOR);	/* ZDLE-ZCRCG received */
        //public static string GOTCRCQ = "j|0400";//(ZCRCQ|GOTOR);	/* ZDLE-ZCRCQ received */
        //public static string GOTCRCW = "k|0400";//(ZCRCW|GOTOR);	/* ZDLE-ZCRCW received */
        //public static string GOTCAN = "0400|030";//(GOTOR|030);	/* CAN*5 seen */


        ///* Byte positions within header array */
        //public static int ZF0 = 3;	/* First flags byte */
        //public static int ZF1 = 2;
        //public static int ZF2 = 1;
        //public static int ZF3 = 0;
        //public static int ZP0 = 0;	/* Low order 8 bits of position */
        //public static int ZP1 = 1;
        //public static int ZP2 = 2;
        //public static int ZP3 = 3;	/* High order 8 bits of file position */

    }
}
using System;

namespace ZModem_Protocol
{
    partial class ZModem
    {
        /// <summary> Parameters for ZSINIT frame: Max length of attention string = 32 </summary>
        static int ZATTNLEN = 32;

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace ZModem_Protocol
{
    public partial class ZModem
    {
        static string pEndOfCommand = null;
        static string EndOfCommand
        {
            get
            {
                if (pEndOfCommand == null)
                {
                    pEndOfCommand = "";
                    pEndOfCommand += ((char)ControlBytes.CR).ToString();
                    pEndOfCommand += ((char)ControlBytes.LF).ToString();
                    //pEndOfCommand += ((char)ControlBytes.XON).ToString();
                }
                return pEndOfCommand;
            }
        }

        static string pBinCommonHeader;
        static string BinCommonHeader
        {
            get
            {
                if (pBinCommonHeader == null)
                {
                    pBinCommonHeader = ((char)ControlBytes.ZPAD).ToString() + ((char)ControlBytes.ZDLE).ToString() + ((char)ControlBytes.ZBIN).ToString();
                }
                return pBinCommonHeader;
            }
        }

        static string pHexCommonHeader;
        static string HexCommonHeader
        {
            get
            {
                if (pHexCommonHeader == null)
                {
                    pHexCommonHeader = ((char)ControlBytes.ZPAD).ToString() + ((char)ControlBytes.ZPAD).ToString() +
                        ((char)ControlBytes.ZDLE).ToString() + ((char)ControlBytes.ZHEX).ToString();
                }
                return pHexCommonHeader;
            }
        }

        static byte[] pSessionAbortSeq;
        static byte[] SessionAbortSeq
        {
            get
            {
                if (pSessionAbortSeq == null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pSessionAbortSeq = null;
                        for (int i = 0; i < 8; i++)
                            ms.WriteByte(Convert.ToByte((char)24));
                        for (int i = 0; i < 10; i++)
                            ms.WriteByte(Convert.ToByte((char)8));
                        ms.WriteByte(Convert.ToByte((char)0));
                        pSessionAbortSeq = ms.ToArray();
                    }
                }
                return pSessionAbortSeq;
            }
        }

        static byte[] pEndOfSession;
        static byte[] EndOfSession
        {
            get
            {
                if (pEndOfSession == null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        for (int i = 0; i < 5; i++)
                            ms.WriteByte((byte)ControlBytes.ZDLE);
                        pEndOfSession = ms.ToArray();
                    }
                }
                return pEndOfSession;
            }
        }

        static byte[] pAttSeq;
        static byte[] AttSeq
        {
            get
            {
                if (pAttSeq == null)
                {
                    pAttSeq = new byte[2] { 0xDD, 0x00 };
                }
                return pAttSeq;
            }
        }

         

        static string BuildZRPOSFrame(int Position)
        {
            string pos = Position.ToString("x8");
            string frame = HexCommonHeader + ((int)HeaderType.ZRPOS).ToString("x2") + pos.Substring(6, 2) + pos.Substring(4, 2) + pos.Substring(2, 2) + pos.Substring(0, 2);
            ComputeHexHeaderCRC(ref frame);
            return frame;
        }

        static string BuildHexFrame(HeaderType type, int arg0, int arg1, int arg2, int arg3)
        {
            string frame = HexCommonHeader + ((int)type).ToString("x2") + arg0.ToString("x2")
                + arg1.ToString("x2") + arg2.ToString("x2") + arg3.ToString("x2");

            //Calcul du crc
            int crc = ComputeHeaderCRC((int)type, arg0, arg1, arg2, arg3);
            frame += crc.ToString("x4");

            //Ajout de la fin de trame
            frame += EndOfCommand;
            return frame;
        }

        static string BuildHexFrame(HeaderType type, ZRINIT_Header_ZF0 trcapa)
        {
            int trpfort = ((int)trcapa) >> 0x8;
            int trpfaible = ((int)trcapa) - trpfort * 0xff;

            string frame = BuildHexFrame(type, 0, 0, trpfort, trpfaible);
            return frame;
        }

        static string BuildZRINITFrame(ZRINIT_Header_ZF0 trcapa, int BufferLength)
        {
            int trpfort = ((int)trcapa) >> 0x8;
            int trpfaible = ((int)trcapa) - trpfort * 256;

            int BLfort = ((int)BufferLength) >> 0x8;
            int BLfaible = ((int)BufferLength) - BLfort * 256;


            string frame = BuildHexFrame(HeaderType.ZRINIT, BLfaible, BLfort, trpfort, trpfaible);
            return frame;
        }

        static string BuildZRQINITFrame()
        {
            string frame = BuildHexFrame(HeaderType.ZRQINIT, 0, 0, 0, 0);
            return frame;
        }

        public static byte[] BuildZFILEFrame(string FileName, int FileSize)
        {
            Queue<byte> bframe2send = new Queue<byte>();
            string frame = BuildHexFrame(HeaderType.ZFILE, (int)ZFILE_Header_ZF0.NOTHING, (int)ZFILE_Header_ZF1.ZMCLOB, (int)ZFILE_Header_ZF2.NOTHING, (int)ZFILE_Header_ZF3.NOTHING);
            foreach (char c in frame)
                bframe2send.Enqueue((byte)c);

            Queue<byte> binframe = new Queue<byte>();
            foreach (char c in FileName)
                binframe.Enqueue((byte)c);
            binframe.Enqueue((byte)'\0');
            
            byte[] b4crc = binframe.Concat(new byte[] {(byte)ZDLE_Sequence.ZCRCW}).ToArray();
            
            //string size = FileSize.ToString();
            //foreach (char c in size)
            //    binframe.Enqueue((byte)c);

            //binframe.Enqueue((byte)' ');
            //for (int i = 0; i < 7; i++)
            //{
            //    binframe.Enqueue((byte)'0');
            //    binframe.Enqueue((byte)'');
            //}

            binframe.Enqueue((byte)ControlBytes.ZDLE);
            binframe.Enqueue((byte)ZDLE_Sequence.ZCRCW);

            Crc16Ccitt crc = new Crc16Ccitt(Crc16Ccitt.InitialCrcValue.Zeros);
            byte[] crcdata = crc.ComputeChecksumBytes(b4crc);

            binframe.Enqueue(crcdata[0]);
            binframe.Enqueue(crcdata[1]);



            return bframe2send.Concat(binframe).ToArray();
        }

        static string BuildZACKFrame(int currentPosition)
        {
            string pos = currentPosition.ToString("x8");
            string frame = HexCommonHeader + ((int)HeaderType.ZRPOS).ToString("x2") + pos.Substring(6, 2) + pos.Substring(4, 2) + pos.Substring(2, 2) + pos.Substring(0, 2);
            ComputeHexHeaderCRC(ref frame);
            return frame;
        }

        static string BuildZNACKFrame()
        {
            return BuildHexFrame(HeaderType.ZNAK, 0, 0, 0, 0);
        }

        static string BuildZFINFrame()
        {
            return BuildHexFrame(HeaderType.ZFIN, 0, 0, 0, 0);
        }

        static string BuildZABORTFrame()
        {
            return BuildHexFrame(HeaderType.ZABORT, 0, 0, 0, 0);
        }

        static string BuildBinFrame(HeaderType type, int arg0, int arg1, int arg2, int arg3)
        {
            string frame = BinCommonHeader + ((char)(int)type).ToString() + ((char)(int)arg0).ToString() +
                ((char)(int)arg1).ToString() + ((char)(int)arg2).ToString() + ((char)(int)arg3).ToString();

            //Calcul du crc
            int crc = ComputeHeaderCRC((int)type, arg0, arg1, arg2, arg3);
            int r1 = crc / 0xFF;
            int r2 = crc - (r1 * 0xFF);
            frame += ((char)r1).ToString();
            frame += ((char)r2).ToString();

            return frame;
        }
    }
}
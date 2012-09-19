using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace ZModem_Protocol
{
    /// <summary>
    /// Class to get file through ZModem.
    /// </summary>
    public partial class ZModem : IDisposable
    {
        class HeaderReceiver
        {
            public HeaderFormat format;
            public HeaderType type;
            public Byte[] crc;
            public Byte[] args;
            
            bool valid = false;
            int receivedBytes =-1;
            bool lastByteisZDLE = false;
            string tempoHex = "";

            public HeaderReceiver()
            {
                crc = new Byte[4];
                args = new Byte[4];
            }

            public void ReceivedNewByteInHeader(byte b, ref bool errorInHeader, ref bool HeaderInProgress)
            {
                if (receivedBytes == -1)
                {
                        if (b == Convert.ToByte(ControlBytes.ZPAD))
                            return;

                        else if (b == Convert.ToByte(ControlBytes.ZDLE))
                        {
                            receivedBytes = 0;
                        }
                        return;
                }

                if (lastByteisZDLE)
                {
                    b = Convert.ToByte(b ^ 64);
                    lastByteisZDLE = false;
                }
                else if (b == Convert.ToByte(ControlBytes.ZDLE))
                {
                    lastByteisZDLE = true;
                    return;
                }

                if (receivedBytes == 0)
                { // Find the type
                    if (b == Convert.ToByte(ControlBytes.ZBIN))
                        format = HeaderFormat.Bin16;
                    else if (b == Convert.ToByte(ControlBytes.ZBINC))
                        format = HeaderFormat.Bin32;
                    else if (b == Convert.ToByte(ControlBytes.ZHEX))
                        format = HeaderFormat.Hex;

                        //format = (HeaderFormat)Enum.Parse(typeof(HeaderFormat), ((int)b).ToString());
                        receivedBytes++;
                        tempoHex = "";
                        return;
                }

                if (format == HeaderFormat.Bin16)
                {
                    switch (receivedBytes)
                    {
                        case 1:

                            type = (HeaderType)Enum.Parse(typeof(HeaderType), ((int)b).ToString());
                            receivedBytes++;
                            return;

                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            args[receivedBytes - 2] = b;
                            receivedBytes++;
                            break;

                        case 6:
                            crc[receivedBytes - 6] = b;
                            receivedBytes++;
                            break;
                        case 7:
                            crc[receivedBytes - 6] = b;
                            receivedBytes++;
                            if (CheckCRC(type, args, crc))
                            {
                                valid = true;
                                errorInHeader = false;
                                HeaderInProgress = false;
                            }
                            else
                            {
                                valid = false;
                                errorInHeader = true;
                            }
                            break;
                    }
                }
                else if (format == HeaderFormat.Hex)
                {
                    switch (receivedBytes)
                    {
                        case 1:
                            if (string.IsNullOrEmpty(tempoHex))
                                tempoHex = ((char)b).ToString();
                            else
                            {
                                tempoHex += ((char)b).ToString();
                                type = (HeaderType)Enum.Parse(typeof(HeaderType), tempoHex);
                                tempoHex = "";
                                receivedBytes++;
                            }
                            return;

                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            if (string.IsNullOrEmpty(tempoHex))
                                tempoHex = ((char)b).ToString();
                            else
                            {
                                tempoHex += ((char)b).ToString();
                                int a1 = Convert.ToInt32(tempoHex, 16);
                                args[receivedBytes-2] = Convert.ToByte(a1);
                                tempoHex = "";
                                receivedBytes++;
                            }
                            return;
                        
                        case 6:
                        case 7:
                            if (string.IsNullOrEmpty(tempoHex))
                                tempoHex = ((char)b).ToString();
                            else
                            {
                                tempoHex += ((char)b).ToString();
                                int a1 = Convert.ToInt32(tempoHex, 16);
                                crc[receivedBytes - 6] = Convert.ToByte(a1);
                                tempoHex = "";
                                receivedBytes++;
                            }
                            if (CheckCRC(type, args, crc))
                                valid = true;
                            else
                                valid = false;
                            break;

                        case 8:
                        case 9:
                        case 10:
                            if (b == Convert.ToByte(ControlBytes.CR))
                            {
                                receivedBytes++;
                                HeaderInProgress = false;
                                return;
                            }
                            else if (b == Convert.ToByte(ControlBytes.LF))
                            {
                                receivedBytes++;
                                HeaderInProgress = false;
                                return;
                            }
                            else if (b == Convert.ToByte(ControlBytes.XON))
                            {
                                receivedBytes++;
                                HeaderInProgress = false;
                                return;
                            }
                            else
                                throw new Exception("CR LR XON expeced after an Hex header!");
                            break;
                    }
                }
                else if (format == HeaderFormat.Bin32)
                {
                    switch (receivedBytes)
                    {
                        case 1:
                            type = (HeaderType)Enum.Parse(typeof(HeaderType), ((int)b).ToString());
                            receivedBytes++;
                            return;

                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            args[receivedBytes - 2] = b;
                            receivedBytes++;
                            break;

                        case 6:
                        case 7:
                        case 8:
                        case 9:
                            crc[receivedBytes - 6] = b;
                            receivedBytes++;
                            throw new Exception("CRC check not implemented for binary 32bits headers");
                            HeaderInProgress = false;
                            break;
                    }
                }

            }

            bool CheckCRC(HeaderType type,Byte[] args, Byte[] crc)
            {
                Crc16Ccitt crc16 = new Crc16Ccitt(ZModem_Protocol.Crc16Ccitt.InitialCrcValue.Zeros);
                Byte[] HeaderCrc = crc16.ComputeChecksumBytes(new Byte[5] { Convert.ToByte((int)type), args[0], args[1], args[2], args[3] });

                if ((HeaderCrc[0] == crc[1]) && (HeaderCrc[1] == crc[0]))
                    return true;
                else
                    return false;
            }

            internal int GetIntArg()
            {
                int val = 0;
                for (int i = 3; i >= 0; i--)
                    val = val * 256 + args[i];
                return val;
            }

        }
    }
}

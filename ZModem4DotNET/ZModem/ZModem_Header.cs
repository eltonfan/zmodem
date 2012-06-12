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
        /// <summary>
        /// internal class to decrypt ZModem header
        /// </summary>
        class Header
        {
            public HeaderType type;
            public Byte arg1;
            public Byte arg2;
            public Byte arg3;
            public Byte arg4;
            public Byte crc1;
            public Byte crc2;
            public bool valid = false;
            public int endofHeader;

            /// <summary>
            /// Decode a potential ZDLE encoded byte
            /// </summary>
            /// <param name="data">Byte array</param>
            /// <param name="index">index of the byte in the array</param>
            /// <returns>Byte ZDLE decoded</returns>
            private Byte DecodeByte(Byte[] data, ref int index)
            {
                Byte r = data[index];
                if (r == Convert.ToByte(ControlBytes.ZDLE))
                {
                    index++;
                    r = Convert.ToByte(data[index] ^ 64);
                }
                return r;
            }

            /// <summary>
            /// Decrypt header value and provide it as an int
            /// </summary>
            /// <returns> arguments value (int)</returns>
            public int GetArgValue()
            {
                int val = (int)arg1;
                val += ((int)arg2 << 8);
                val += ((int)arg3 << 16);
                val += ((int)arg4 << 24);
                return val;
            }

            /// <summary>
            /// Constructor. It decode a Byte array to a ZModem header.
            /// </summary>
            /// <param name="data">Byte array</param>
            /// <param name="startIndex">index of the byte in the array</param>
            public Header(Byte[] data, int startIndex)
            {
                if (startIndex < 0)
                    return;

                int index = startIndex;

                if (data[index] == Convert.ToByte(ControlBytes.ZPAD))
                {
                    index++;
                    if (data[index] == Convert.ToByte(ControlBytes.ZDLE))
                    {
                        index++;
                        if (data[index] == Convert.ToByte(ControlBytes.ZBIN))
                        {
                            index++;

                            type = (HeaderType)Enum.Parse(typeof(HeaderType), ((int)DecodeByte(data, ref index)).ToString());
                            index++;
                            arg1 = DecodeByte(data, ref index);
                            index++;
                            arg2 = DecodeByte(data, ref index);
                            index++;
                            arg3 = DecodeByte(data, ref index);
                            index++;
                            arg4 = DecodeByte(data, ref index);
                            index++;
                            crc1 = DecodeByte(data, ref index);
                            index++;
                            crc2 = DecodeByte(data, ref index);

                            Byte[] b = new Byte[5];
                            b[0] = Convert.ToByte((int)type);
                            b[1] = arg1;
                            b[2] = arg2;
                            b[3] = arg3;
                            b[4] = arg4;

                            Crc16Ccitt crccomp = new Crc16Ccitt(0);
                            Byte[] crcAttempted = crccomp.ComputeChecksumBytes(b);
                            if ((crc1 != crcAttempted[1]) || (crc2 != crcAttempted[0]))
                            {
                                valid = false;
                                return; //erreur crc
                            }
                            else
                            {
                                valid = true;
                            }
                            endofHeader = index;
                        }
                    }
                    else if (data[index] == Convert.ToByte(ControlBytes.ZPAD)) //Header Hex
                    {
                        index++;
                        if (data[index] == Convert.ToByte(ControlBytes.ZDLE))
                        {
                            index++;

                            if (data[index] == Convert.ToByte(ControlBytes.ZHEX))
                            {
                                index++;
                                string val = ((char)DecodeByte(data, ref index)).ToString();
                                index++;
                                val += ((char)DecodeByte(data, ref index)).ToString();
                                type = (HeaderType)Enum.Parse(typeof(HeaderType), val);

                                index++;
                                val = ((char)DecodeByte(data, ref index)).ToString();
                                index++;
                                val += ((char)DecodeByte(data, ref index)).ToString();
                                int a1 = Convert.ToInt32(val, 16);
                                arg1 = Convert.ToByte(a1);

                                index++;
                                val = ((char)DecodeByte(data, ref index)).ToString();
                                index++;
                                val += ((char)DecodeByte(data, ref index)).ToString();
                                int a2 = Convert.ToInt32(val, 16);
                                arg2 = Convert.ToByte(a2);

                                index++;
                                val = ((char)DecodeByte(data, ref index)).ToString();
                                index++;
                                val += ((char)DecodeByte(data, ref index)).ToString();
                                int a3 = Convert.ToInt32(val, 16);
                                arg3 = Convert.ToByte(a3);

                                index++;
                                val = ((char)DecodeByte(data, ref index)).ToString();
                                index++;
                                val += ((char)DecodeByte(data, ref index)).ToString();
                                int a4 = Convert.ToInt32(val, 16);
                                arg4 = Convert.ToByte(a4);

                                index++;
                                val = ((char)DecodeByte(data, ref index)).ToString();
                                index++;
                                val += ((char)DecodeByte(data, ref index)).ToString();
                                int c1 = Convert.ToInt32(val, 16);
                                crc1 = Convert.ToByte(c1);

                                index++;
                                val = ((char)DecodeByte(data, ref index)).ToString();
                                index++;
                                val += ((char)DecodeByte(data, ref index)).ToString();
                                int c2 = Convert.ToInt32(val, 16);
                                crc2 = Convert.ToByte(c2);

                                Byte[] b = new Byte[5];
                                b[0] = Convert.ToByte((int)type);
                                b[1] = arg1;
                                b[2] = arg2;
                                b[3] = arg3;
                                b[4] = arg4;

                                Crc16Ccitt crccomp = new Crc16Ccitt(0);
                                Byte[] crcAttempted = crccomp.ComputeChecksumBytes(b);
                                if ((crc1 != crcAttempted[1]) || (crc2 != crcAttempted[0]))
                                {
                                    valid = false;
                                    return; //erreur crc
                                }
                                else
                                {
                                    valid = true;
                                }
                                endofHeader = index;
                            }
                        }
                    }
                }
            }
        }
    }
}
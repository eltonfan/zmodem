//using System;
//using System.IO;

//namespace ZModem_Protocol
//{
//    public partial class ZModem
//    {
//        /// <summary>
//        /// internal class used to decrypt a ZDATA frame
//        /// </summary>
//        class BinaryFrame
//        {
//            public Byte[] data;
//            public Byte EndOfFrame;
//            public Byte[] crc;
//            public int nbvalue;
//            public bool state = false;
//            public int LastIndexOfFrame;

//            public BinaryFrame()
//            {
//                data = new Byte[0];
//                nbvalue = 0;
//                crc = new Byte[2];
//            }

//            public bool Addvalue(Byte b)
//            {
//                if (nbvalue == data.Length)
//                    Array.Resize(ref data, nbvalue + 1);

//                data[nbvalue] = b;
//                nbvalue++;
//                return true;
//            }

//            /// <summary>
//            /// Used to check transmission errors
//            /// </summary>
//            /// <returns>true if frame is valid, false if a transmission error occured</returns>
//            public bool CheckCRC()
//            {
//                Crc16Ccitt crc16 = new Crc16Ccitt(InitialCrcValue.Zeros);
//                Byte[] b = new Byte[nbvalue + 1];
//                for (int i = 0; i < nbvalue; i++)
//                {
//                    b[i] = Convert.ToByte(data[i]);
//                }
//                b[nbvalue] = Convert.ToByte(EndOfFrame);
//                Byte[] r = crc16.ComputeChecksumBytes(b);

//                if ((r[0] == crc[1]) && (r[1] == crc[0]))
//                    state = true;
//                else
//                    state = false;
//                return state;
//            }
//        }

//        private bool ExtractDataBinaryFrame(MemoryStream msInput, MemoryStream msOutput)
//        {
//            BinaryFrame bf = new BinaryFrame();
//            //int i = startIndex;
//            msInput.Seek(zDataLastIndexofFrame, SeekOrigin.Begin);
//            int b;
//            while (msInput.Position < msInput.Length)
//            {
//                b = msInput.ReadByte();

//                if (b != Convert.ToByte(ControlBytes.ZDLE)) // Not an Esc car -> direct add
//                {
//                    bf.Addvalue((byte)b);
//                }
//                else // Esc car... check what come next
//                {
//                    b = msInput.ReadByte();
//                    if ((b == Convert.ToByte(ZDLE_Sequence.ZCRCG)) || (b == Convert.ToByte(ZDLE_Sequence.ZCRCE)))
//                    {
//                        bf.EndOfFrame = (byte)b;
//                        bf.crc[0] = ReadByte(msInput);
//                        bf.crc[1] = ReadByte(msInput);
//                        bf.LastIndexOfFrame = (int)msInput.Position;
//                        zDataLastIndexofFrame = bf.LastIndexOfFrame;

//                        if (!bf.CheckCRC()) // Check CRC
//                            return false;

//                        //If good, add to msOutput
//                        msOutput.Write(bf.data, 0, bf.nbvalue);
//                        if (BytesTransferedEvent != null)
//                            BytesTransferedEvent(null, new BytesTransferedEventArgs((int)msOutput.Position));
//                        bf = new BinaryFrame();

//                        if (bf.EndOfFrame == Convert.ToByte(ZDLE_Sequence.ZCRCE)) // end of frame, get out!
//                            return true;
//                    }
//                    else if ((b == Convert.ToByte(ZDLE_Sequence.ZCRCQ)) || (b == Convert.ToByte(ZDLE_Sequence.ZCRCW)))
//                    {
//                        bf.EndOfFrame = (byte)b;

//                        bf.crc[0] = ReadByte(msInput);
//                        bf.crc[1] = ReadByte(msInput);
//                        bf.LastIndexOfFrame = (int)msInput.Position;
//                        zDataLastIndexofFrame = bf.LastIndexOfFrame;

//                        if (!bf.CheckCRC()) // Check CRC
//                            return false;

//                        //If good, add to msOutput
//                        msOutput.Write(bf.data, 0, bf.nbvalue);
//                        if (BytesTransferedEvent != null)
//                            BytesTransferedEvent(null, new BytesTransferedEventArgs((int)msOutput.Position));
//                        bf = new BinaryFrame();

//                        //Send ZACK
//                        string frame = BuildZACKFrame((int)msDownloadedFile.Position);
//                        SendCommand(frame);
//                        state = TransfertState.Initialized;

//                        if (bf.EndOfFrame == Convert.ToByte(ZDLE_Sequence.ZCRCW)) // end of frame, get out!
//                            return true;
//                    }
//                    else
//                    {
//                        if (b < 0)
//                            break; //On ne sait jamais... lecture hors du memory stream

//                        bf.Addvalue(Convert.ToByte((char)(b ^ 64))); // car escaped
//                    }
//                }
//            }
//            //if ((portserie.BytesToRead == 0) && (ZModemPositionIndex >= msInput.Length - 1))
//            //    return false;
//            msInput.Seek(0, SeekOrigin.End);
//            return true; //wait next frame to complete the frame
//        }




//        Byte ReadByte(Byte[] data, ref int index)
//        {
//            if (data[index] != Convert.ToByte(ControlBytes.ZDLE))
//                return data[index];
//            else
//            {
//                index++;
//                return (Convert.ToByte((char)(data[index] ^ 64)));
//            }
//        }

//        Byte ReadByte(MemoryStream ms)
//        {
//            byte b = (byte)ms.ReadByte();
//            if (b != Convert.ToByte(ControlBytes.ZDLE))
//                return b;
//            else
//            {
//                return (Convert.ToByte((char)(ms.ReadByte() ^ 64)));
//            }
//        }
//    }


//}

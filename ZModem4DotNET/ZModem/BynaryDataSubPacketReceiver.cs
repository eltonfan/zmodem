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
        class BynaryDataSubPacketReceiver
        {
            MemoryStream msDestination;
            byte[] data;
            byte frameEnd;
            byte crc1;
            byte crc2;
            bool lastByteisZDLE;
            int sizeOfFrame;
            int postFrame;

            internal BynaryDataSubPacketReceiver(ref MemoryStream destination)
            {
                InitFrame();
                msDestination = destination;
            }

            void InitFrame()
            {
                data = new byte[1025];
                lastByteisZDLE = false;
                sizeOfFrame = 0;
                postFrame = 0;
            }
            
            /// <summary>
            ///     Treatment function when a data is received during frame reception
            /// </summary>
            /// <param name="b">the byte received</param>
            /// <param name="endOfFrame"> Is this byte indicate a end of the frame?</param>
            /// <param name="ZackRequested"> Is a Zackanswer requeste?</param>
            /// <param name="errorOfTransmission"> Was there a transmission error?</param>
            /// <param name="needAttnSequence"> Do we nee to send the Attn sequence?</param>
            /// <param name="nextBytesHeader"> Next received bytes are a header</param>
            internal void ReceivedNewByteInDataFrame(byte b, ref bool endOfFrame, ref bool ZackRequested, ref bool errorOfTransmission,  ref bool needAttnSequence, ref bool nextBytesHeader)
            {
                endOfFrame = false;
                ZackRequested = false;
                errorOfTransmission = false;
                needAttnSequence = false;
                nextBytesHeader = false;

                if ((!lastByteisZDLE) && (postFrame == 0))
                {//previous one was not ZDLE
                    if (b == Convert.ToByte(ControlBytes.ZDLE))
                    { //this byte is ZDLE... nothing to add this time
                        lastByteisZDLE = true;
                    }
                    else
                    { // Not an Esc car -> direct add
                        if (sizeOfFrame == 1025)
                        {
                            errorOfTransmission = true;
                            needAttnSequence = true;
                            InitFrame();
                            return;
                        }
                        data[sizeOfFrame] = b;
                        sizeOfFrame++;
                    }
                }
                else                
                { // Previous car was ZDLE.
                    switch (postFrame)
                    {
                        case 0:
                            {
                                if ((b == Convert.ToByte(ZDLE_Sequence.ZCRCG)) || (b == Convert.ToByte(ZDLE_Sequence.ZCRCE)) ||
                                    (b == Convert.ToByte(ZDLE_Sequence.ZCRCQ)) || (b == Convert.ToByte(ZDLE_Sequence.ZCRCW)))
                                {
                                    frameEnd = b;
                                    postFrame++;
                                    data[sizeOfFrame] = b; // for CRC computation
                                    sizeOfFrame++;
                                }
                                else
                                { //this byte was escaped... 
                                    if (sizeOfFrame == 1025)
                                    {
                                        errorOfTransmission = true;
                                        needAttnSequence = true;
                                        InitFrame();
                                        return;
                                    }
                                    data[sizeOfFrame] = Convert.ToByte((char)(b ^ 64)); // car escaped
                                    sizeOfFrame++;
                                }
                                lastByteisZDLE = false;
                                break;
                            }
                        case 1:
                            {
                                if (lastByteisZDLE)
                                {
                                    crc1 = Convert.ToByte((char)(b ^ 64)); // car escaped
                                    lastByteisZDLE = false;
                                }
                                else
                                {
                                    if (b == Convert.ToByte(ControlBytes.ZDLE))
                                    {
                                        lastByteisZDLE = true;
                                        return;
                                    }
                                    else
                                        crc1 = b;
                                }
                                postFrame++;
                                break;
                            }
                        case 2:
                            {
                                if (lastByteisZDLE)
                                {
                                    crc2 = Convert.ToByte((char)(b ^ 64)); // car escaped
                                    lastByteisZDLE = false;
                                }
                                else
                                {
                                    if (b == Convert.ToByte(ControlBytes.ZDLE))
                                    {
                                        lastByteisZDLE = true;
                                        return;
                                    }
                                    else
                                        crc2 = b;
                                }
                                postFrame++;

                                if (CheckCRC())//Compute CRC
                                { //CRC ok
                                    msDestination.Write(data, 0, sizeOfFrame - 1);
                                    if (BytesTransferedEvent != null)
                                        BytesTransferedEvent(null, new BytesTransferedEventArgs((int)msDestination.Position));

                                    if (frameEnd == Convert.ToByte(ZDLE_Sequence.ZCRCG))
                                    { // A new packet is following
                                        //reinit the frame structure for next frame
                                        InitFrame();
                                    }
                                    else if (frameEnd == Convert.ToByte(ZDLE_Sequence.ZCRCE))
                                    { // End of the frame, no answer requested.
                                        endOfFrame = true;
                                        //nextBytesHeader = true;
                                    }
                                    else if (frameEnd == Convert.ToByte(ZDLE_Sequence.ZCRCQ))
                                    { // A ZACK frame is required to continue
                                        ZackRequested = true;
                                        //reinit the frame structure for next frame
                                        InitFrame();
                                    }
                                    else if (frameEnd == Convert.ToByte(ZDLE_Sequence.ZCRCW))
                                    { // A ZACK frame is required
                                        //This part is not clear, the spec says it's not a end of frame, but Zmodem.h says it's a end of frame...
                                        endOfFrame = true;
                                        ZackRequested = true;
                                        //reinit the frame structure for next frame
                                        InitFrame();
                                    }
                                }
                                else
                                { //CRC not ok
                                    errorOfTransmission = true;

                                    if ((frameEnd == Convert.ToByte(ZDLE_Sequence.ZCRCG)) || (frameEnd == Convert.ToByte(ZDLE_Sequence.ZCRCE)))
                                    { // A new packet is following
                                        needAttnSequence = true; // pas toujours
                                    }

                                    //reinit the frame structure for next frame
                                    InitFrame();
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
            }

            bool CheckCRC()
            {
                Crc16Ccitt crc16 = new Crc16Ccitt(ZModem_Protocol.Crc16Ccitt.InitialCrcValue.Zeros);
                if (data.Length > sizeOfFrame)
                    Array.Resize<byte>(ref data, sizeOfFrame);

                Byte[] r = crc16.ComputeChecksumBytes(data);
                MemoryStream ms = new MemoryStream();
                if ((r[0] == crc2) && (r[1] == crc1))
                    return true;
                else
                    return false;
            }

            //Event for size
            internal delegate void BytesTransferedHandler(object sender, BytesTransferedEventArgs e);
            //internal class BytesTransferedEventArgs : EventArgs
            //{
            //    public int size;

            //    public BytesTransferedEventArgs(int size)
            //    {
            //        this.size = size;
            //    }
            //}
            /// <summary>
            /// This event is used to report progress during transfert.
            /// </summary>
            internal event BytesTransferedHandler BytesTransferedEvent;
        }


    }
}

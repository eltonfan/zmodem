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


        bool HeaderInProgress = false;
        bool FrameInProgress = false;
        bool ErrorInHeader = false;
        bool ErrorOfTransmission = false;
        bool NeedAttnSeq = false;
        bool EndOfFrame = false;
        bool ZackRequested = false;
        bool NextByteHeader = false;
        bool SessionInit = false;
        bool SessionClose = false;
        bool ErrorInData = false;

        volatile bool StopThread = false;

        static object lockAcces = null;

        //SerialPort portserie = null;
        ZModem.BynaryDataSubPacketReceiver bdata = null;
        ZModem.HeaderReceiver hdata = null;
        ZModem.SessionReceiver sdata = null;
        MemoryStream msZfile = null;
        MemoryStream msfile = null;
        Queue<Byte> receivedBytes = null; // this variable is shared
        volatile bool ThreadRequestSerialPort = false; // this variable is also shared between the 2 thread.
        Thread t = null;
        ZFileInfo zfileInfo = null;

        /// <summary>
        /// Constructor. Make sure that the serial port is accessible...
        /// </summary>
        /// <param name="serialPort">Serial port object</param>
        public ZModem(SerialPort serialPort)
        {
            receivedBytes = new Queue<byte>();
            lockAcces = new object();

            t = new Thread(new ThreadStart(DataReceived));
            t.Name = "ByteTreatment Thread";

            portserie = serialPort;
            if (portserie.IsOpen)
                portserie.Close();

            portserie.ReceivedBytesThreshold = 1;
            portserie.ReadBufferSize = 32768;
            portserie.WriteBufferSize = 32768;

            portserie.Open();
        }

        ///// <summary>
        ///// Function to get a file. Make sure you unsuscribe DataReceived events from SerialPort before calling this function!
        ///// </summary>
        ///// <param name="FileName">FileName on sender system</param>
        ///// <param name="timeOutS">Break transfert before after specified number of seconds.</param>
        ///// <returns>Return a Memory Stream. It's up to you to write it to a file or deserialize it, or whatever...</returns>
        public MemoryStream ZModemTransfert(string fileName, int timeOutS)
        {
            state = TransfertState.Initializing;

            //Init of variables to allow multiple download with a simple object.
            HeaderInProgress = false;
            FrameInProgress = false;
            ErrorInHeader = false;
            ErrorOfTransmission = false;
            NeedAttnSeq = false;
            EndOfFrame = false;
            ZackRequested = false;
            NextByteHeader = false;
            SessionInit = false;
            SessionClose = false;
            ErrorInData = false;

            StopThread = false;
            Console.WriteLine("");
            Console.WriteLine("Start transfert of " + fileName + "...");

            receivedBytes = new Queue<byte>();
            lockAcces = new object();

            if (!t.IsAlive)
                t = new Thread(new ThreadStart(DataReceived));

            t.Start(); 
            portserie.DataReceived += new SerialDataReceivedEventHandler(portserie_DataReceived);
            msZfile = new MemoryStream();
            msfile = new MemoryStream();
            sdata = new SessionReceiver();
            hdata = new HeaderReceiver();
            SendCommand("sz " + fileName);
            state = TransfertState.Initialized;

            DateTime start = DateTime.Now;
            while (!SessionClose)
            {
                Thread.Sleep(50);
                if (timeOutS > 0)
                    if ((DateTime.Now - start).TotalSeconds > timeOutS)
                    {
                        Console.WriteLine("Timeout for this operation!");
                        portserie.DataReceived -= new SerialDataReceivedEventHandler(portserie_DataReceived);
                        NeedAttnSeq = true;
                        StopThread = true;
                        t.Join();
                        Thread.Sleep(1000);
                        ThreadRequestSerialPort = true;
                        lock (lockAcces)
                        {
                            portserie.DiscardInBuffer();
                            SendCommand(SessionAbortSeq);
                            portserie.DiscardInBuffer();
                            return msfile;
                        }
                    }
            }
            StopThread = true;
            portserie.DataReceived -= new SerialDataReceivedEventHandler(portserie_DataReceived);

            GC.Collect();
            
            state = TransfertState.Ended;
            return msfile;
        }


        void portserie_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DateTime lastPause = DateTime.Now;
            Byte b;
            while ((portserie.BytesToRead > 0) && (!ThreadRequestSerialPort))
            {
                lock (lockAcces)
                {
                    b = (Byte)portserie.ReadByte();
                    receivedBytes.Enqueue(b);

                    if (portserie.BytesToRead > 16384)
                    {
                        if ((DateTime.Now - lastPause).TotalMilliseconds >= 1000)
                        {
                            SendCommand(new byte[1] { 0xDE });
                            lastPause = DateTime.Now;
                            Console.WriteLine("InBuffer too high: " + portserie.BytesToRead + ". Pause sender stream for 1s");
                        }
                    }
                }
            }

            if (NeedAttnSeq)
            {
                portserie.DiscardOutBuffer();
                SendCommand(AttSeq);
                Thread.Sleep(50);
                NeedAttnSeq = false;
                Thread.Sleep(0);
            }
            if (ThreadRequestSerialPort)
            {
                portserie.DiscardOutBuffer();
                portserie.DiscardInBuffer();
                receivedBytes.Clear();
                ThreadRequestSerialPort = false;
            }
        }



        void DataReceived()
        {
            Byte b;
            DateTime StartReceiveHeader = DateTime.Now;
            //Console.WriteLine("Treatment Thread Starts!");
            while (!StopThread)
            {
                while (receivedBytes.Count > 0)
                {

                        if (StopThread)
                            return;
                        else
                            lock (lockAcces)
                            {
                                b = receivedBytes.Dequeue();
                            }


                    //Console.Write("Buffer:" + portserie.BytesToRead + "\r");
                    if (FrameInProgress)
                    {
                        bdata.ReceivedNewByteInDataFrame(b, ref EndOfFrame, ref ZackRequested, ref ErrorOfTransmission, ref NeedAttnSeq, ref NextByteHeader);
                        if (ErrorOfTransmission)
                        {
                            state = TransfertState.Error;
                            if (NeedAttnSeq)
                                Console.WriteLine("NeedAttn");

                            Console.WriteLine("CRC error detected, long pause (to make sure that no data remains in buffer), then try to resume");
                            ThreadRequestSerialPort = true;
                            Thread.Sleep(0);
                            portserie.DiscardOutBuffer();
                            portserie.DiscardInBuffer();
                            Thread.Sleep(500);
                            receivedBytes.Clear();
                            SendCommand(BuildZRPOSFrame((int)msfile.Position));
                            Thread.Sleep(0);
                            EndOfFrame = true;
                            FrameInProgress = false;
                            NextByteHeader = true;
                            continue;
                        }
                        else if (EndOfFrame)
                        {
                            FrameInProgress = false;
                            //HeaderInProgress = false;
                            if (hdata.type == HeaderType.ZFILE)
                            {
                                receivedBytes.Clear();
                                //Console.WriteLine("ZFILE FRAME: ");
                                //Byte[] buf = msZfile.ToArray();
                                //for (int i = 0; i < buf.Length; i++)
                                //    Console.Write((char)buf[i]);
                                //Console.WriteLine("");

                                 zfileInfo = new ZFileInfo(hdata, msZfile);

                                if (FileInfoEvent != null)
                                    FileInfoEvent(null, new FileInfoEventArgs(zfileInfo.fileName, zfileInfo.size));

                                SendCommand(BuildZRPOSFrame((int)msfile.Position));
                                continue;
                            }
                            else if (ZackRequested)
                            {
                                SendCommand(BuildZACKFrame((int)msfile.Position));
                                continue;
                            }
                            else if (hdata.type == HeaderType.ZDATA)
                            {
                                if (zfileInfo.size != msfile.Length)
                                    Console.WriteLine("File not fully transfered, " + msfile.Length.ToString() + " / " + zfileInfo.size + "Bytes transfered");
                                else
                                    Console.WriteLine("File successfully transfered, " + msfile.Length.ToString() + " Bytes transfered!");
                                //Thread.Sleep(200);

                                continue;
                            }
                            else
                            {
                                Console.WriteLine("An unattempted header had been received: " + hdata.type.ToString());
                            }
                            continue;
                        }
                        else if (ZackRequested)
                        {
                            SendCommand(BuildZACKFrame((int)msfile.Position));
                            continue;
                        }
                        else
                            continue;
                    }
                    else if (HeaderInProgress)
                    {
                        hdata.ReceivedNewByteInHeader(b, ref ErrorInHeader, ref HeaderInProgress);

                        if (ErrorInHeader)
                        {
                            state = TransfertState.Error;
                            receivedBytes.Clear();
                            portserie.DiscardOutBuffer();
                            SendCommand(BuildZNACKFrame());
                            HeaderInProgress = false;
                            continue;
                        }
                        else if (HeaderInProgress)
                        {
                            continue;
                        }
                        else
                        {
                            //Analyze header and answer accordingly
                            if (hdata.type == HeaderType.ZRQINIT)
                            {
                                state = TransfertState.Initialized;
                                portserie.DiscardOutBuffer();
                                SendCommand(BuildZRINITFrame(ZRINIT_Header_ZF0.CANBREAK, 1024));
                            }
                            else if (hdata.type == HeaderType.ZFILE)
                            {
                                bdata = new BynaryDataSubPacketReceiver(ref msZfile);
                                FrameInProgress = true;
                            }
                            else if (hdata.type == HeaderType.ZDATA)
                            {
                                if (hdata.GetIntArg() != msfile.Position)
                                {
                                    throw new Exception("Position Error: Stream is not starting from the right place!");
                                }

                                state = TransfertState.Transfering;
                                bdata = new BynaryDataSubPacketReceiver(ref msfile);
                                bdata.BytesTransferedEvent += new BynaryDataSubPacketReceiver.BytesTransferedHandler(bdata_BytesTransferedEvent);
                                FrameInProgress = true;
                            }
                            else if (hdata.type == HeaderType.ZEOF)
                            {
                                Console.Write("End of file... ");
                                state = TransfertState.Ended;
                                portserie.DiscardOutBuffer();
                                HeaderInProgress = false;
                                SendCommand(BuildZRINITFrame(ZRINIT_Header_ZF0.CANBREAK, 1024));
                            }
                            else if (hdata.type == HeaderType.ZFIN)
                            {
                                Console.Write("End of File List...");
                                state = TransfertState.ClosingSession;
                                SendCommand(BuildZFINFrame());
                            }
                            else
                            {
                                throw new Exception("Unknow case??");
                            }
                            continue;
                        }
                    }
                    else
                    {
                        
                        sdata.ReceivedNewByte(b, ref SessionInit, ref SessionClose, ref HeaderInProgress, ref ErrorInData);

                        if (ErrorInData)
                        {
                            portserie.DiscardInBuffer();
                            portserie.DiscardOutBuffer();
                            receivedBytes.Clear();
                            Thread.Sleep(100);
                            SendCommand("");
                            ErrorInData = false;
                        }
                        else if (HeaderInProgress)
                        {
                            hdata = new HeaderReceiver();
                            StartReceiveHeader = DateTime.Now;
                        }
                        else if (NextByteHeader)
                        {
                            Console.WriteLine("This byte should have been * !!");
                            NeedAttnSeq = true;
                            Thread.Sleep(0);
                            Thread.Sleep(100);
                            portserie.DiscardInBuffer();
                            receivedBytes.Clear();
                            SendCommand(ZModem.EndOfSession);
                        }
                        else if (SessionClose)
                        {
                            return;
                        }
                        continue;
                    }
                }
                if (HeaderInProgress)
                {
                    if ((DateTime.Now - StartReceiveHeader).TotalMilliseconds > 500)
                    {
                        Console.WriteLine("Too much time without an header!");
                        NeedAttnSeq = true;
                        ThreadRequestSerialPort = true;
                        portserie.DiscardOutBuffer();
                        portserie.DiscardInBuffer();
                        receivedBytes.Clear();
                        Thread.Sleep(500);
                        SendCommand(BuildZRPOSFrame((int)msfile.Position));
                        HeaderInProgress = false;
                    }
                }
                else
                    Thread.Sleep(0);

            }
            //Console.WriteLine("Treatment Thread Stops!");
        }

        void bdata_BytesTransferedEvent(object sender, BytesTransferedEventArgs e)
        {
            if (BytesTransferedEvent != null)
                BytesTransferedEvent(null, e);
        }

    }
}
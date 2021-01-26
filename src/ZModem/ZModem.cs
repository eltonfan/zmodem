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
        #region IDisposable Membres
        public void Dispose()
        {
            receivedBytes = null;
            zfileInfo = null;
            msZfile = null;
            sdata = null;
            hdata = null;
            bdata = null;
            GC.Collect();
        }

        #endregion


        SerialPort portserie;

        TransfertState pstate;
        TransfertState state
        {
            get { return pstate; }
            set
            {
                if (value != pstate)
                {
                    if (TransfertStateEvent != null)
                        TransfertStateEvent(null, new TransfertStateEventArgs(state));
                    pstate = value;
                }
            }
        }


        bool SendCommand(Byte[] Command)
        {
            lock (lockAcces)
            {
                if (!portserie.IsOpen)
                    portserie.Open();
                portserie.DiscardOutBuffer();
                portserie.Write(Command, 0, Command.Length);
                portserie.Write(EndOfCommand);
            }
            Thread.Sleep(100);
            return true;
        }

        bool SendCommand(string Command)
        {
            lock (lockAcces)
            {
                if (!portserie.IsOpen)
                    portserie.Open();
                portserie.DiscardOutBuffer();
                portserie.Write(Command);
                portserie.Write(EndOfCommand);
            }
            Thread.Sleep(100);
            return true;
        }

        //public enum TransfertState { Initializing, Initialized, Transfering, Ended, ClosingSession, Error };

        ////Event for state
        //public delegate void TransfertStateHandler(object sender, TransfertStateEventArgs e);
        //public class TransfertStateEventArgs : EventArgs
        //{
        //    public TransfertState state;

        //    public TransfertStateEventArgs(TransfertState state)
        //    {
        //        this.state = state;
        //    }
        //}
        //public event TransfertStateHandler TransfertStateEvent;

        ////Event for FileInfo
        //public delegate void FileInfoHandler(object sender, FileInfoEventArgs e);
        //public class FileInfoEventArgs : EventArgs
        //{
        //    public string fileName;
        //    public int size;

        //    public FileInfoEventArgs(string fileName, int size)
        //    {
        //        this.fileName = fileName;
        //        this.size = size;
        //    }
        //}
        //public event FileInfoHandler FileInfoEvent;

        ////Event for size
        //public delegate void BytesTransferedHandler(object sender, BytesTransferedEventArgs e);
        //public class BytesTransferedEventArgs : EventArgs
        //{
        //    public int size;

        //    public BytesTransferedEventArgs(int size)
        //    {
        //        this.size = size;
        //    }
        //}
        //public event BytesTransferedHandler BytesTransferedEvent;

        //int ZModemPositionIndex = -1;
        //bool ZModemSession = false;

        //MemoryStream msReceivedDatabytes;
        //MemoryStream msDownloadedFile;
        //int zDataLastIndexofFrame;
        //Byte[] ReceivedBytes;
        //ZFileInfo zFileInfo = null;
        //Header LastHeader = null;
        //List<BinaryFrame> lbf;
        //int lastpos = 0;




        ///// <summary>
        ///// Constructor. Make sure that the serial port is accessible...
        ///// </summary>
        ///// <param name="serialPort">Serial port object</param>
        //public ZModem(SerialPort serialPort)
        //{
        //    portserie = serialPort;
        //    msReceivedDatabytes = new MemoryStream();
        //    if (portserie.ReadBufferSize <= 16384)
        //    {
        //        if (portserie.IsOpen)
        //        {
        //            portserie.Close();
        //            portserie.ReadBufferSize = 16384;
        //            portserie.Open();
        //            BytesTransferedEvent += new BytesTransferedHandler(ZModem_BytesTransferedEvent); 
        //        }
        //        else
        //            portserie.ReadBufferSize = 16384;
        //    }

        //    lbf = new List<BinaryFrame>();
        //}

        //void ZModem_BytesTransferedEvent(object sender, ZModem.BytesTransferedEventArgs e)
        //{
        //    Console.Write("\r" + e.size.ToString() + " bytes received ");
        //}

        //void portserie_DataReceived_InitSession(object sender, SerialDataReceivedEventArgs e)
        //{
        //    lock (msReceivedDatabytes)
        //    {
        //        lock (portserie)
        //        {
        //            while (portserie.BytesToRead > 0)
        //            {
        //                msReceivedDatabytes.WriteByte((Byte)portserie.ReadByte());
        //            }
        //        }

        //        //Search for frame start
        //        if ((!ZModemSession) && (state != TransfertState.Error))
        //        {

        //            int pos = SearchTextInBytes(msReceivedDatabytes, "rz\r", lastpos);
        //            if (pos < 0) // L'envoi n'a pas commencé
        //            {
        //                //Console.WriteLine("Impossible to start ZModem session");
        //                ZModemSession = false;
        //                SendCommand(ZModem.SessionAbortSeq);
        //                state = TransfertState.Error;
        //                return;
        //            }
        //            else
        //            {
        //                lastpos = pos;
        //                ZModemSession = true;
        //            }
        //        }

        //        //Recherche d'un éventuel header
        //        int nextHeaderPos = SearchTextInBytes(msReceivedDatabytes, HexCommonHeader, lastpos);
        //        if (nextHeaderPos < 0)
        //            nextHeaderPos = SearchTextInBytes(msReceivedDatabytes, BinCommonHeader, lastpos);


        //        if ((nextHeaderPos > 0) && (ZModemSession))
        //        {
        //            LastHeader = new Header(msReceivedDatabytes, nextHeaderPos);
        //            //si le header n'est pas valide, alors on envoi un ZNACK
        //            if (!LastHeader.valid)
        //            {
        //                if (portserie.BytesToRead > 0) // si le header n'est pas complet, on boucle 
        //                    return;

        //                ZModemPositionIndex = LastHeader.endofHeader;
        //                string frame = BuildZNACKFrame();
        //                SendCommand(frame);
        //                Thread.Sleep(50);
        //            }
        //            else
        //            {
        //                lastpos = LastHeader.endofHeader + 1;
        //                if (LastHeader.type == HeaderType.ZRQINIT)
        //                { // si ZRQINIT, alors on envoi un ZRINIT
        //                    ZModemPositionIndex = LastHeader.endofHeader;
        //                    string frame = BuildZRINITFrame(ZRINIT_Header_ZF0.CANBREAK, 4096);
        //                    SendCommand(frame);
        //                }
        //                else if (LastHeader.type == HeaderType.ZNAK)
        //                { // si ZNACK, alors on affiche une erreur?
        //                    ZModemPositionIndex = LastHeader.endofHeader;
        //                    Console.WriteLine("Sender didn't acknowledge!");
        //                }
        //                else if (LastHeader.type == HeaderType.ZRINIT)
        //                { // si ZRINIT, alors l'envoyeur n'est pas fonctionnel?
        //                    ZModemPositionIndex = LastHeader.endofHeader;
        //                    Console.WriteLine("Sender not functional!");
        //                }
        //                else if (LastHeader.type == HeaderType.ZFILE)
        //                { // si ZFILE, alors on va decrypter la frame pour avoir les infos fichiers
        //                    zFileInfo = new ZFileInfo(LastHeader, msReceivedDatabytes);
        //                    ZModemPositionIndex = zFileInfo.endofframe;

        //                    if (FileInfoEvent != null)
        //                        FileInfoEvent(null, new FileInfoEventArgs(zFileInfo.fileName, zFileInfo.size));
        //                    //Console.WriteLine(zFileInfo.ToString());
        //                    state = TransfertState.Initialized;
        //                }
        //            }
        //        }
        //    }
        //}

        //void portserie_DataReceived_ZDataFrame(object sender, SerialDataReceivedEventArgs e)
        //{
        //    lock (msReceivedDatabytes)
        //    {
        //        lock (portserie)
        //        {
        //            while (portserie.BytesToRead > 0)
        //            {
        //                msReceivedDatabytes.WriteByte((Byte)portserie.ReadByte());
        //            }
        //        }

        //        if (state == TransfertState.Initialized)
        //        {
        //            //Recherche d'un éventuel header
        //            int nextHeaderPos = SearchTextInBytes(msReceivedDatabytes, HexCommonHeader, lastpos);
        //            if (nextHeaderPos < 0)
        //                nextHeaderPos = SearchTextInBytes(msReceivedDatabytes, BinCommonHeader, lastpos);


        //            if ((nextHeaderPos >= 0) && (ZModemSession))
        //            {
        //                LastHeader = new Header(msReceivedDatabytes, nextHeaderPos);
        //                //si le header n'est pas valide, alors on envoi un ZNACK
        //                if (!LastHeader.valid)
        //                {
        //                    if (portserie.BytesToRead > 0) // si le header n'est pas complet, on boucle 
        //                        return;

        //                    ZModemPositionIndex = LastHeader.endofHeader;
        //                    string frame = BuildZNACKFrame();
        //                    SendCommand(frame);
        //                    Thread.Sleep(50);
        //                }
        //                else
        //                {
        //                    if (LastHeader.type == HeaderType.ZDATA)
        //                    {
        //                        if (((int)LastHeader.arg1 + 256 * (int)LastHeader.arg2) != msDownloadedFile.Position)
        //                        {
        //                            state = TransfertState.Error;
        //                            CleanReceived();
        //                        }

        //                        state = TransfertState.Transfering;
        //                        zDataLastIndexofFrame = (int)msReceivedDatabytes.Position;
        //                        return;
        //                        //portserie_DataReceived_ZDataFrame(sender, e);
        //                    }
        //                    else if (LastHeader.type == HeaderType.ZRQINIT)
        //                    { // si ZRQINIT, alors on envoi un ZRINIT
        //                        ZModemPositionIndex = LastHeader.endofHeader;
        //                        string frame = BuildZRINITFrame(ZRINIT_Header_ZF0.CANBREAK, 4096);
        //                        SendCommand(frame);
        //                    }
        //                    else
        //                    {
        //                        //Issue, we should only have a ZData frame here
        //                        throw new Exception("Error, LastHeader: " + LastHeader.type.ToString());
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (portserie.BytesToRead == 0)
        //                    state = TransfertState.Error;
        //            }
        //        }
        //        else if (state == TransfertState.Transfering)
        //        {
        //            if (msReceivedDatabytes.Position == msReceivedDatabytes.Length) // this stupid device didn't received the zack or ZPOS frame?
        //                state = TransfertState.Error;

        //            //Lire les data, calculer le crc et envoyer un break si erreur
        //            if (ExtractDataBinaryFrame(msReceivedDatabytes, msDownloadedFile))
        //            {
        //                // Check received data file
        //                if (msDownloadedFile.Position == zFileInfo.size)
        //                {
        //                    state = TransfertState.Ended;
        //                }
        //                else if (msDownloadedFile.Position > zFileInfo.size)
        //                    throw new Exception("Issue during file transmission! Bug to be solved");
        //            }
        //            else
        //            {
        //                Console.WriteLine("CRC error, resuming!");
        //                portserie.DiscardOutBuffer();
        //                SendCommand(AttSeq);
        //                Thread.Sleep(50);
        //                LastHeader = null;
        //                portserie.DiscardInBuffer();
        //                lastpos = 0;
        //                zDataLastIndexofFrame = 0;
        //                msReceivedDatabytes = new MemoryStream();
        //                state = TransfertState.Error;

        //                //string frame = BuildZRPOSFrame((int)msDownloadedFile.Position);
        //                //WriteCmd(frame);
        //            }
        //        }
        //        else if (state == TransfertState.Ended)
        //        {
        //            //Recherche d'un éventuel header
        //            int nextHeaderPos = SearchTextInBytes(msReceivedDatabytes, HexCommonHeader, lastpos);
        //            if (nextHeaderPos < 0)
        //                nextHeaderPos = SearchTextInBytes(msReceivedDatabytes, BinCommonHeader, lastpos);

        //            if (LastHeader.type == HeaderType.ZEOF)
        //            { // si ZEOF, alors le fichier est envoyé complètement?
        //                int lengthOfFile = 0;
        //                for (int i = 0; i < lbf.Count; i++)
        //                    lengthOfFile += lbf[i].data.Length;

        //                if (lengthOfFile != LastHeader.GetArgValue())
        //                    return;
        //                else
        //                {
        //                    ZModemPositionIndex = LastHeader.endofHeader;
        //                    string frame = BuildZRINITFrame(ZRINIT_Header_ZF0.CANBREAK , 4096);
        //                    SendCommand(frame);
        //                }
        //            }
        //            else if (LastHeader.type == HeaderType.ZFIN)
        //            { // si ZFIN, alors on va fermer la session?
        //                lastpos = LastHeader.endofHeader;
        //                string frame = BuildZFINFrame();
        //                SendCommand(frame);
        //                state = TransfertState.ClosingSession;
        //                return;
        //            }
        //            else
        //            {
        //                // ????
        //            }
        //        }
        //        else if (state == TransfertState.ClosingSession)
        //        {
        //            {
        //                //// Check qu'on recoit bien un OO
        //                int OOindex = SearchTextInBytes(ReceivedBytes, "OO", lastpos);
        //                if (OOindex > 0)
        //                    ZModemSession = false;
        //                else
        //                    SendCommand(EndOfSession);
        //            }
        //        }
        //        else if (state == TransfertState.Error)
        //        {
                    
        //            return;
        //        }
        //    }
        //}

        //private void CleanReceived()
        //{
        //    portserie.DiscardInBuffer();
        //    portserie.DiscardOutBuffer();
        //    ReceivedBytes = new Byte[0];
        //    ZModemPositionIndex = 0;
        //    msReceivedDatabytes = new MemoryStream();
        //}

        //private void CloseZModemSession()
        //{
        //    CleanReceived();

        //    //Send ZRINIT
        //    SendCommand(ZModem.SessionAbortSeq);
        //    SendCommand(ZModem.EndOfSession);

        //    //TODO : check qu'on a bien fermé la session
        //    while (portserie.BytesToRead > 0)
        //    {
        //        msReceivedDatabytes.WriteByte((byte)portserie.ReadByte());
        //    }
        //    Console.WriteLine("\rZModem session Closed");
        //    ZModemSession = false;
        //}

        //private int SearchTextInBytes(Byte[] data, string text, int startPosition)
        //{
        //    if (data.Length - text.Length - startPosition < 0)
        //        return -1;

        //    int index = startPosition;
        //    bool founded = false;
        //    while (index < data.Length - text.Length)
        //    {
        //        for (int i = 0; i < text.Length; i++)
        //        {
        //            if (ReceivedBytes[index + i] != text[i])
        //            {
        //                founded = false;
        //                break;
        //            }
        //            founded = true;
        //        }
        //        if (founded)
        //            return index;
        //        index++;
        //    }

        //    return -1;
        //}

        //private int SearchTextInBytes(MemoryStream ms, string text, int startPosition)
        //{ // return MemoryStream just after the text
        //    if (ms.Length - text.Length - startPosition < 0)
        //        return -1;

        //    int index = startPosition;
        //    bool founded = false;
        //    while (index < ms.Length - text.Length)
        //    {
        //        ms.Seek(index, SeekOrigin.Begin);
        //        for (int i = 0; i < text.Length; i++)
        //        {
        //            if (ms.ReadByte() != (byte)text[i])
        //            {
        //                founded = false;
        //                break;
        //            }
        //            founded = true;
        //        }
        //        if (founded)
        //        {
        //            return index;
        //        }
        //        index++;
        //    }

        //    return -1;
        //}

        //private bool InitZModemSession()
        //{
        //    //ReceiveDataByte();
        //    //if (ZModemSession)
        //    //    return true;
        //    //else
        //    //{
        //    //    Console.WriteLine("ZModem session Not initialized!!");
        //        return false;
        //    //}
        //}

        //private string LastFrame(int lastFrameIndex)
        //{
        //    string lastframe = "";
        //    int pos = lastFrameIndex;
        //    while (pos < ReceivedBytes.Length)
        //    {
        //        lastframe += (char)ReceivedBytes[pos];
        //        pos++;
        //    }
        //    return lastframe;
        //}

        ///// <summary>
        ///// Function to get a file. Make sure you unsuscribe DataReceived events from SerialPort before calling this function!
        ///// </summary>
        ///// <param name="FileName">FileName on sender system</param>
        ///// <param name="timeOutMs">not correctly implemented yet</param>
        ///// <returns>Return a Memory Stream. It's up to you to write it to a file or deserialize it, or whatever...</returns>
        //public MemoryStream ZModemTransfert(string FileName, int timeOutMs)
        //{
        //    msReceivedDatabytes = new MemoryStream();
        //    state = TransfertState.Initializing;
        //    if (!portserie.IsOpen)
        //        portserie.Open();
        //    //CleanReceived();
        //    portserie.DiscardInBuffer();
        //    portserie.DiscardOutBuffer();
        //    lastpos = 0;
        //    //portserie.DataReceived += portserie_DataReceived_InitSession;

        //    DateTime start = DateTime.Now;
        //    //WriteCmd("sz " + FileName, "\n"); //Demarrage de l'envoi
        //    SendCommand("sz " + FileName); //Demarrage de l'envoi

        //    while ((state == TransfertState.Initializing) && ((DateTime.Now - start).TotalMilliseconds < 1000))
        //    {
        //        //Waiting for connection
        //        Thread.Sleep(50);
        //        portserie_DataReceived_InitSession(null, null);
        //    }

        //    //Check That file transfert is initialized
        //    if (state != TransfertState.Initialized)
        //    {
        //        Console.WriteLine("Impossible to initialize session, Aborting session...");
        //        CloseZModemSession();
        //        return null;
        //    }

        //    //Change mode for ZData reception
        //    //portserie.DataReceived -= portserie_DataReceived_InitSession;
        //    //portserie.DataReceived += portserie_DataReceived_ZDataFrame;

        //    msDownloadedFile = new MemoryStream(zFileInfo.size);

        //    //Send ZRPos -> Start file transmittion
        //    while (msDownloadedFile.Position != zFileInfo.size)
        //    {
        //        string frame = BuildZRPOSFrame((int)msDownloadedFile.Position);
        //        SendCommand(frame);

        //        while ((state == TransfertState.Initialized) || (state == TransfertState.Transfering))
        //        {
        //            portserie_DataReceived_ZDataFrame(null, null);
        //            //Waiting for connection
        //            Thread.Sleep(50);
        //        }

        //        if (state == TransfertState.Error)
        //        {
        //            //do something
        //            state = TransfertState.Initialized;
        //        }
        //    }

        //    //portserie.DataReceived -= portserie_DataReceived_ZDataFrame;

        //    Console.WriteLine("\rEnd of transmission.");

        //    CloseZModemSession();
        //    return msDownloadedFile;

        //    //Thread.Sleep(100);
        //    //try
        //    //{
        //    //    //if (!InitZModemSession())
        //    //    //    return null;

        //    //    //if (LastHeader.type != HeaderType.ZFILE)
        //    //    //    return null;

        //    //    //DateTime Start = DateTime.Now;

        //    //    //Send ZRPos -> Demarrage de la transmittion du fichier
        //    //    frame = BuildZRPOSFrame(0);
        //    //    WriteCmd(frame);
        //    //    Thread.Sleep(50);
        //    //    ReceivedBytes = new Byte[0];
        //    //    ReceiveZDataByte();

        //    //    //Check que l'on recoit a bien recu les data
        //    //    int l = 0;
        //    //    for (int i = 0; i < lbf.Count; i++)
        //    //        l += lbf[i].nbvalue;

        //    //    MemoryStream ms = null;

        //    //    if (zFileInfo.size == l)
        //    //    {
        //    //        ms = new MemoryStream(l);
        //    //        for (int i = 0; i < lbf.Count; i++)
        //    //            ms.Write(lbf[i].data, 0, lbf[i].nbvalue);
        //    //    }

        //    //    return ms;

        //    //}
        //    //finally
        //    //{
        //    //        CloseZModemSession();
        //    //}



        //    return null;
        //}
    }
}

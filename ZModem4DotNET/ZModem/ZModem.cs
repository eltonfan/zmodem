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
    public partial class ZModem
    {
        int ZModemPositionIndex = -1;
        bool ZModemSession = false;
        SerialPort portserie;
        Byte[] ReceivedBytes;
        ZFileInfo zFileInfo = null;
        Header LastHeader = null;
        List<BinaryFrame> lbf;

        /// <summary>
        /// Constructor. Make sure that the serial port is accessible...
        /// </summary>
        /// <param name="serialPort">Serial port object</param>
        public ZModem(SerialPort serialPort)
        {
            portserie = serialPort;
            lbf = new List<BinaryFrame>();
        }

        bool WriteCmd(string Command)
        {
            portserie.DiscardOutBuffer();
            portserie.Write(Command + "\r\n");
            Thread.Sleep(100);
            return true;
        }

        bool WriteCmd(string Command, string termination)
        {
            portserie.DiscardOutBuffer();
            portserie.Write(Command + termination);
            Thread.Sleep(100);
            return true;
        }

        private void ReceiveDataByte()
        {
            if ((portserie.BytesToRead == 0) && (ZModemPositionIndex >= ReceivedBytes.Length-1))
                return;

            //Lecture
            int byteIndex = ReceivedBytes.Length;
            int lastpos = ZModemPositionIndex;
            Array.Resize(ref ReceivedBytes, (int)(ReceivedBytes.Length + portserie.BytesToRead));

            while (portserie.BytesToRead != 0)
            {
                if (ReceivedBytes.Length < portserie.BytesToRead + byteIndex)
                    Array.Resize(ref ReceivedBytes, (int)(ReceivedBytes.Length + portserie.BytesToRead));

                ReceivedBytes[byteIndex] = (Byte)portserie.ReadByte();
                byteIndex++;
            }

            //Search for frame start
            if (!ZModemSession)
            {
                if (SearchTextInBytes(ReceivedBytes, "rz\r", lastpos) < 0) // L'envoi n'a pas commencé
                {
                    //Console.WriteLine("Impossible to start ZModem session");
                    ZModemSession = false;
                    WriteCmd(ZModem.EndOfSession, "\r\n");
                    return;
                }
                else
                    ZModemSession = true;
            }

            //Recherche d'un éventuel header
            int nextHeader = SearchTextInBytes(ReceivedBytes, HexCommonHeader, lastpos);
            if (nextHeader < 0)
                nextHeader = SearchTextInBytes(ReceivedBytes, BinCommonHeader, lastpos);

            
            if ((nextHeader > 0) && (ZModemSession))
            {
                LastHeader = new Header(ReceivedBytes, nextHeader);
                //si le header n'est pas valide, alors on envoi un ZNACK
                if (!LastHeader.valid)
                {
                    ZModemPositionIndex = LastHeader.endofHeader;
                    string frame = BuildZNACKFrame();
                    WriteCmd(frame);
                    Thread.Sleep(50);
                }
                else
                {
                    if (LastHeader.type == HeaderType.ZRQINIT)
                    { // si ZRQINIT, alors on envoi un ZRINIT
                        ZModemPositionIndex = LastHeader.endofHeader; 
                        string frame = BuildZRINITFrame(ZRINIT_Header_ZF0.CANBREAK | ZRINIT_Header_ZF0.CANFULLDUPLEX, 512);
                        WriteCmd(frame);
                    }
                    else if (LastHeader.type == HeaderType.ZNAK)
                    { // si ZNACK, alors on affiche une erreur?
                        ZModemPositionIndex = LastHeader.endofHeader; 
                        Console.WriteLine("Sender didn't acknowledge!");
                    }
                    else if (LastHeader.type == HeaderType.ZRINIT)
                    { // si ZRINIT, alors l'envoyeur n'est pas fonctionnel?
                        ZModemPositionIndex = LastHeader.endofHeader; 
                        Console.WriteLine("Sender not functional!");
                    }
                    else if (LastHeader.type == HeaderType.ZFILE)
                    { // si ZFILE, alors on va decrypter la frame pour avoir les infos fichiers
                        zFileInfo = new ZFileInfo(LastHeader, ReceivedBytes);
                        ZModemPositionIndex = zFileInfo.endofframe;
                        //Console.WriteLine(zFileInfo.ToString());
                    }
                    else if (LastHeader.type == HeaderType.ZEOF)
                    { // si ZEOF, alors le fichier est envoyé complètement?
                        int lengthOfFile = 0;
                        for (int i = 0; i < lbf.Count; i++)
                            lengthOfFile += lbf[i].data.Length;

                        if (lengthOfFile != LastHeader.GetArgValue())
                            return;
                        else
                        {
                            ZModemPositionIndex = LastHeader.endofHeader;
                            string frame = BuildZRINITFrame(ZRINIT_Header_ZF0.CANBREAK | ZRINIT_Header_ZF0.CANFULLDUPLEX, 512);
                            WriteCmd(frame);
                        }
                    }
                    else if (LastHeader.type == HeaderType.ZFIN)
                    { // si ZFIN, alors on va fermer la session?
                        ZModemPositionIndex = LastHeader.endofHeader;
                        int bufpos = ReceivedBytes.Length;
                        string frame = BuildZFINFrame();
                        WriteCmd(frame);

                        //// Check qu'on recoit bien un OO
                        while (portserie.BytesToRead != 0)
                        {
                            if (ReceivedBytes.Length < portserie.BytesToRead + byteIndex)
                                Array.Resize(ref ReceivedBytes, (int)(ReceivedBytes.Length + portserie.BytesToRead));

                            ReceivedBytes[byteIndex] = (Byte)portserie.ReadByte();
                            byteIndex++;
                        }

                        int OOindex = SearchTextInBytes(ReceivedBytes, "OO", bufpos);
                        if (OOindex > 0)
                            ZModemSession = false;
                        else
                            WriteCmd(EndOfSession);
                    }
                    else if (LastHeader.type == HeaderType.ZDATA)
                    { // si ZFIN, alors on va fermer la session?
                        //Lire les data, calculer le crc et envoyer un break si erreur
                        if (ExtractBinaryFrame(ReceivedBytes, ref lbf, LastHeader.endofHeader + 1))
                            ZModemPositionIndex = lbf.Last().EndOfFrame;
                        else
                        {
                            Console.WriteLine("CRC error, resuming!");
                            int length = 0;
                            for (int i = 0; i < lbf.Count; i++)
                                length += lbf[i].data.Length;

                            ZModemPositionIndex = ReceivedBytes.Length-1;
                            string frame = BuildZRPOSFrame(length);
                            WriteCmd(frame);
                        }

                    }
                }
            }
            else
                ZModemPositionIndex = ReceivedBytes.Length - 1;

            Thread.Sleep(50);
            ReceiveDataByte();
        }

        private void CleanReceived()
        {
            portserie.DiscardInBuffer();
            portserie.DiscardOutBuffer();
            ReceivedBytes = new Byte[0];
            ZModemPositionIndex = 0;
        }

        private void CloseZModemSession()
        {
            //Send ZRINIT
            string frame = ZModem.EndOfSession;
            WriteCmd(frame, "");
            //TODO : check qu'on a bien fermé la session
            Console.WriteLine("ZModem session Closed");
            ZModemSession = false;
        }

        private int SearchTextInBytes(Byte[] data, string text, int startPosition)
        {
            if (data.Length - text.Length - startPosition < 0)
                return -1;

            int index = startPosition;
            bool founded = false;
            while (index < data.Length - text.Length)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (ReceivedBytes[index + i] != text[i])
                    {
                        founded = false;
                        break;
                    }
                    founded = true;
                }
                if (founded)
                    return index;
                index++;
            }

            return -1;
        }

        private bool InitZModemSession()
        {
            ReceiveDataByte();
            if (ZModemSession)
                return true;
            else
            {
                Console.WriteLine("ZModem session Not initialized!!");
                return false;
            }
        }

        private string LastFrame(int lastFrameIndex)
        {
            string lastframe = "";
            int pos = lastFrameIndex;
            while (pos < ReceivedBytes.Length)
            {
                lastframe += (char)ReceivedBytes[pos];
                pos++;
            }
            return lastframe;
        }

        /// <summary>
        /// Function to get a file. Make sure you unsuscribe DataReceived events from SerialPort before calling this function!
        /// </summary>
        /// <param name="FileName">FileName on sender system</param>
        /// <param name="timeOutMs">not correctly implemented yet</param>
        /// <returns>Return a Memory Stream. It's up to you to write it to a file or deserialize it, or whatever...</returns>
        public MemoryStream ZModemTransfert(string FileName, int timeOutMs)
        {
            if (!portserie.IsOpen)
                portserie.Open();
            CleanReceived();
            WriteCmd("sz " + FileName, "\n"); //Demarrage de l'envoi
            Thread.Sleep(100);
            try
            {
                if (!InitZModemSession())
                    return null;

                if (LastHeader.type != HeaderType.ZFILE)
                    return null;

                DateTime Start = DateTime.Now;

                //Send ZRPos -> Demarrage de la transmittion du fichier
                string frame = BuildZRPOSFrame(0);
                WriteCmd(frame);
                Thread.Sleep(50);
                ReceiveDataByte();

                //Check que l'on recoit a bien recu les data
                int l = 0;
                for (int i = 0; i < lbf.Count; i++)
                    l += lbf[i].nbvalue;

                MemoryStream ms = null;

                if (zFileInfo.size == l)
                {
                    ms = new MemoryStream(l);
                    for (int i = 0; i < lbf.Count; i++)
                        ms.Write(lbf[i].data, 0, lbf[i].nbvalue);
                }

                return ms;

            }
            finally
            {
                if (ZModemSession)
                    CloseZModemSession();
            }



            return null;
        }

        private bool ExtractBinaryFrame(Byte[] data, ref List<BinaryFrame> retour, int startIndex)
        {
            BinaryFrame bf = new BinaryFrame();
            int i = startIndex;
            while (i < data.Length)
            {
                if (data[i] != Convert.ToByte(ControlBytes.ZDLE))
                {
                    bf.Addvalue(data[i]);
                }
                else
                {
                    i++;
                    if ((data[i] == Convert.ToByte(ZDLE_Sequence.ZCRCG)) || (data[i] == Convert.ToByte(ZDLE_Sequence.ZCRCE)))
                    {
                        bf.EndOfFrame = data[i];
                        i++;
                        bf.crc[0] = ReadByte(data, ref i);
                        i++;
                        bf.crc[1] = ReadByte(data, ref i);
                        bf.LastIndexOfFrame = i;


                        if (!bf.CheckCRC())
                            return false;

                        retour.Add(bf);

                        if (bf.EndOfFrame == Convert.ToByte(ZDLE_Sequence.ZCRCE)) // end of frame, get out!
                            return true;

                        bf = new BinaryFrame();
                    }
                    else if ((data[i] == Convert.ToByte(ZDLE_Sequence.ZCRCQ)) || (data[i] == Convert.ToByte(ZDLE_Sequence.ZCRCW)))
                    {
                        bf.EndOfFrame = data[i];
                        i++;
                        bf.crc[0] = ReadByte(data, ref i);
                        i++;
                        bf.crc[1] = ReadByte(data, ref i);
                        bf.LastIndexOfFrame = i;


                        if (!bf.CheckCRC())
                            return false;

                        retour.Add(bf);

                        //Send ZACK
                        int l = 0;
                        for (int j = 0; j < lbf.Count; j++)
                            l += lbf[j].nbvalue;

                        string frame = BuildZACKFrame(l);
                        WriteCmd(frame);

                        if (bf.EndOfFrame == Convert.ToByte(ZDLE_Sequence.ZCRCE)) // end of frame, get out!
                            return true; ;

                        bf = new BinaryFrame();
                    }
                    else
                    {
                        bf.Addvalue(Convert.ToByte((char)(data[i] ^ 64))); // car escaped
                    }
                }
                i++;
            }
            return false;
        }

        Byte ReadByte(Byte[] data, ref int index)
        {
            if (data[index] != Convert.ToByte(ControlBytes.ZDLE))
                return data[index];
            else
            {
                index++;
                return (Convert.ToByte((char)(data[index] ^ 64)));
            }
        }

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
                    pEndOfCommand += ((char)ControlBytes.XON).ToString();
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

        static string pEndOfSession;
        static string EndOfSession
        {
            get
            {
                if (pEndOfSession == null)
                {
                    pEndOfSession = "";
                    for (int i = 0; i < 5; i++)
                        pEndOfSession += ((char)ControlBytes.ZDLE).ToString();
                    pEndOfSession += EndOfCommand;
                }
                return pEndOfSession;
            }
        }

        private static int ComputeHeaderCRC(int type, int arg0, int arg1, int arg2, int arg3)
        {
            //Calcul du crc
            Byte[] b = new Byte[5];
            b[0] = Convert.ToByte((int)type);
            b[1] = Convert.ToByte(arg0);
            b[2] = Convert.ToByte(arg1);
            b[3] = Convert.ToByte(arg2);
            b[4] = Convert.ToByte(arg3);
            int ret = calcrc(b, b.Length);

            return ret;
        }

        private static bool ComputeHexHeaderCRC(ref string HexFrame)
        {
            if (HexFrame.Length != 14)
                return false;
            if (HexFrame[0] != Convert.ToChar(ControlBytes.ZPAD)) //Header Hex
                return false;
            if (HexFrame[1] != Convert.ToChar(ControlBytes.ZPAD)) //Header Hex
                return false;
            if (HexFrame[2] != Convert.ToChar(ControlBytes.ZDLE)) //Header Hex
                return false;
            if (HexFrame[3] != Convert.ToChar(ControlBytes.ZHEX)) //Header Hex
                return false;

            int crc = ComputeHeaderCRC(Convert.ToInt32(HexFrame.Substring(4, 2), 16), Convert.ToInt32(HexFrame.Substring(6, 2), 16),
                Convert.ToInt32(HexFrame.Substring(8, 2), 16), Convert.ToInt32(HexFrame.Substring(10, 2), 16), Convert.ToInt32(HexFrame.Substring(12, 2), 16));

            HexFrame += crc.ToString("x4");
            return true;
        }

        private static int calcrc(Byte[] data, int count)
        {
            int crc = 0;
            int d;
            for (int i = 0; i < count; i++)
            {
                d = data[i];
                crc = crc ^ (d << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (crc << 1) ^ 0x1021;
                    else
                        crc = (crc << 1);
                }
            }
            return (crc & 0xFFFF);
        }

        static string BuildZRPOSFrame(int Position)
        {
            string pos = Position.ToString("x8");
            string frame = HexCommonHeader + ((int)HeaderType.ZRPOS).ToString("x2") + pos.Substring(6, 2) + pos.Substring(4, 2) + pos.Substring(2, 2) + pos.Substring(0, 2);
            ComputeHexHeaderCRC(ref frame);
            frame += EndOfCommand;
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

        static string BuildZACKFrame(int currentPosition)
        {
            string pos = currentPosition.ToString("x8");
            string frame = HexCommonHeader + ((int)HeaderType.ZRPOS).ToString("x2") + pos.Substring(6, 2) + pos.Substring(4, 2) + pos.Substring(2, 2) + pos.Substring(0, 2);
            ComputeHexHeaderCRC(ref frame);
            frame += EndOfCommand;
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

            //Ajout de la fin de trame
            frame += EndOfCommand;
            return frame;
        }

    }
}

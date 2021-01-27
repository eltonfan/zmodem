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
        public bool ZModemSend(string fileName, int timeOutS)
        {
            if (!portserie.IsOpen)
                portserie.Open();

            Queue<byte> inputBuffer = new Queue<byte>();
            string zrqinit = BuildZRQINITFrame();
            SendCommand("rz\r");
            SendCommand(BuildZRQINITFrame());
            //Thread.Sleep(50);
            while (portserie.BytesToRead > 0)
            {
                inputBuffer.Enqueue((byte)portserie.ReadByte());
            }

            Console.WriteLine("Answer to ZRQINIT from device:");
            while (inputBuffer.Count > 0)
                Console.Write((char)inputBuffer.Dequeue());
            Console.WriteLine("");


            byte[] b = null;
            b = BuildZFILEFrame(fileName, 10141);
            for (int i = 0; i < b.Length; i++)
                Console.Write((char)b[i]);
            Console.WriteLine("");
            SendCommand(b);
            //Thread.Sleep(50);
            while (portserie.BytesToRead > 0)
            {
                inputBuffer.Enqueue((byte)portserie.ReadByte());
            }
            Console.WriteLine("Answer to ZFILE from device:");
            while (inputBuffer.Count > 0)
                Console.Write((char)inputBuffer.Dequeue());
            Console.WriteLine("");


            SendCommand(SessionAbortSeq);
            //SendCommand(BuildZFINFrame());
            //Thread.Sleep(500);
            //SendCommand("OO");

            portserie.Close();

              return false;
        }
    }

}
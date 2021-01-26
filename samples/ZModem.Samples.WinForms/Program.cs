using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Windows.Forms;

namespace ZModem_example
{
    class Program
    {
        static int cas = 4;
        
        static void Main(string[] args)
        {
            if (cas == 0)
            {
                //Create a com port object
                SerialPort port = new SerialPort("COM11", 115200, Parity.None, 8, StopBits.One);
                if (port.IsOpen)
                    port.Close();

                //Solve the MS issue happening on some computers
                try
                {
                    SerialPort_Fix.SerialPortFixer.Execute(port.PortName);
                }
                catch (Exception e)
                {
                    Console.WriteLine("SerialPort fixer exception: " + e.Message);
                    Console.ReadKey();
                    return;
                }

                //Initiate a ZModem object 
                ZModem_Protocol.ZModem zmodem = new ZModem_Protocol.ZModem(port);
                MemoryStream ms = null;


                zmodem.FileInfoEvent += new ZModem_Protocol.ZModem.FileInfoHandler(zmodem_FileInfoEvent);
                zmodem.BytesTransferedEvent += new ZModem_Protocol.ZModem.BytesTransferedHandler(zmodem_BytesTransferedEvent);
                zmodem.TransfertStateEvent += new ZModem_Protocol.ZModem.TransfertStateHandler(zmodem_TransfertStateEvent);

                //download file "toto"
                //Make sure you unsuscribe all DAtaReceived event from port before calling following line.

                //for (int i = 0; i < 10; i++)

                //zmodem.ZModemTransfert("Test.jpg", 2000);


            }

            if (cas == 1)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Test_UI());
                return;
            }

        //    if (cas == 2)
        //    {
        //        //Create a com port object
        //        SerialPort port = new SerialPort("COM11", 115200, Parity.None, 8, StopBits.One);
        //        if (port.IsOpen)
        //            port.Close();

        //        //Solve the MS issue happening on some computers
        //        try
        //        {
        //            SerialPort_Fix.SerialPortFixer.Execute(port.PortName);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("SerialPort fixer exception: " + e.Message);
        //            Console.ReadKey();
        //            return;
        //        }

        //        //Initiate a ZModem object 
        //        ZModem_Protocol.ZModem zmodem = new ZModem_Protocol.ZModem(port);
        //        MemoryStream ms = null;

        //        //download file "toto"
        //        //Make sure you unsuscribe all DAtaReceived event from port before calling following line.

        //        //for (int i = 0; i < 10; i++)
        //        ms = zmodem.ZModemTransfert("ILTLog.txt", 10000);

        //        if (ms != null)
        //        {
        //            string fn = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "ILT.txt");
        //            File.WriteAllBytes(fn, ms.ToArray());
        //            //   Console.WriteLine(ms.Length.ToString() + " bytes Received");
        //        }

        //        if (port.IsOpen)
        //            port.Close();

        //    }
            else if (cas == 4)
            {
                //Create a com port object
                SerialPort port = new SerialPort("COM11", 115200, Parity.None, 8, StopBits.One);
                if (port.IsOpen)
                    port.Close();

                //Solve the MS issue happening on some computers
                try
                {
                    SerialPort_Fix.SerialPortFixer.Execute(port.PortName);
                }
                catch (Exception e)
                {
                    Console.WriteLine("SerialPort fixer exception: " + e.Message);
                    Console.ReadKey();
                    return;
                }

                //Initiate a ZModem object 
                ZModem_Protocol.ZModem zmodem = new ZModem_Protocol.ZModem(port);


                zmodem.ZModemSend("test.txt", 10);


            }
            Console.WriteLine("Press a key to Exit...");
            Console.ReadKey();
        }

        static void zmodem_TransfertStateEvent(object sender, ZModem_Protocol.ZModem.TransfertStateEventArgs e)
        {
            Console.WriteLine("Current Status: " + e.state);
        }

        static void zmodem_FileInfoEvent(object sender, ZModem_Protocol.ZModem.FileInfoEventArgs e)
        {
            Console.WriteLine("File Name: " + e.fileName + ", file size: " + e.size);

        }

        static void zmodem_BytesTransferedEvent(object sender, ZModem_Protocol.ZModem.BytesTransferedEventArgs e)
        {
            Console.Write(e.size + " Bytes transfered" +  "\r");
        }
    }
}

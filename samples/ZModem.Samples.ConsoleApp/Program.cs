using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using ZModem_Protocol;

namespace ZModem.Samples.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ZModem Samples");
            var cas = 4;

            var portNames = SerialPort.GetPortNames();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Available ports:");

            for (int i = 0; i < portNames.Length; i++)
            {
                var portName = $"{i}: {portNames[i]}";
                Console.WriteLine(portName);
            }

            Console.Write("Selected port ID: ");
            var selectedID = Console.ReadLine();

            if (!int.TryParse(selectedID, out int portID))
            {
                Console.WriteLine($"Port ID {selectedID} not found. Exiting...");
                Console.ReadKey();
                return;
            }

            SerialPort serialPort = null;
            try
            {
                var portName = portNames[portID];
                var boudRate = 115200;
                var parity = Parity.None;
                var dataBits = 8;
                var stopBits = StopBits.One;

                var createInfo = $"Using {portName} with BoudRate: {boudRate}, Parity: {parity}, Databits: {dataBits}, StopBits: {stopBits}";
                Console.WriteLine(createInfo);

                serialPort = new SerialPort(portName, boudRate, parity, dataBits, stopBits);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
                return;
            }
            //Solve the MS issue happening on some computers
            try
            {
                SerialPort_Fix.SerialPortFixer.Execute(serialPort.PortName);
            }
            catch (Exception e)
            {
                Console.WriteLine("SerialPort fixer exception: " + e.Message);
                Console.ReadKey();
                return;
            }

            if (cas == 0)
            {
                TestReceiveFile(serialPort);
            }
            else if (cas == 2)
            {
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
            }
            else if (cas == 4)
            {
                TestSendFile(serialPort);
            }
            Console.WriteLine("Press a key to Exit...");
            Console.ReadKey();
        }

        static void TestSendFile(SerialPort port)
        {
            //Initiate a ZModem object 
            var zmodem = new ZModem_Protocol.ZModem(port);

            if(zmodem.ZModemSend(@"data\version.txt", 1000))
            {
                Console.WriteLine("Succeeded to send file.");
            }
            else
            {
                Console.WriteLine("Failed to send file.");
            }
        }

        static void TestReceiveFile(SerialPort port)
        {
            //Initiate a ZModem object 
            var zmodem = new ZModem_Protocol.ZModem(port);
            MemoryStream ms = null;

            zmodem.FileInfoEvent += (sender, args) =>
            {
                Console.WriteLine("File Name: " + args.FileName + ", file size: " + args.Size);
            };
            zmodem.BytesTransferedEvent += (sender, args) =>
            {
                Console.Write(args.Size + " Bytes transfered" + "\r");
            };
            zmodem.TransfertStateEvent += (sender, args) =>
            {
                Console.WriteLine("Current Status: " + args.State);
            };

            //download file "toto"
            //Make sure you unsuscribe all DAtaReceived event from port before calling following line.

            //for (int i = 0; i < 10; i++)

            //zmodem.ZModemTransfert("Test.jpg", 2000);


            //backgroundWorker1.RunWorkerAsync();

            ////download file 
            ////Make sure you unsuscribe all DAtaReceived event from port before calling following line.
            ms = zmodem.ZModemTransfert("test.txt", 10);

            if (ms != null)
            {
                var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                File.WriteAllBytes(Path.Combine(basePath, $"Verison-{DateTime.Now:yyyyMMdd-HHmmss-fff}.txt"), ms.ToArray());
            }

            Console.WriteLine("Press a key to Exit...");
            Console.ReadKey();
        }
    }
}

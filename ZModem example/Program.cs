using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace ZModem_example
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a com port object
            SerialPort port = new SerialPort("COM13", 115200, Parity.None, 8, StopBits.One);
            if (port.IsOpen)
                port.Close();

            //Solve the MS issue happening on some computers
            SerialPort_Fix.SerialPortFixer.Execute(port.PortName);

            //Initiate a ZModem object 
            ZModem_Protocol.ZModem zmodem = new ZModem_Protocol.ZModem(port);
            MemoryStream ms = null;
            
            //download file "toto"
            //Make sure you unsuscribe all DAtaReceived event from port before calling following line.
            ms = zmodem.ZModemTransfert("toto", 1000);

            if (port.IsOpen)
                port.Close();
        }
    }
}

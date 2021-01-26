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

        class SessionReceiver
        {
            string receivedbuffer = "";
            byte[] AbortFrame;

            public SessionReceiver()
            {
                receivedbuffer = "";
                AbortFrame = new byte[0];
            }

            public void ReceivedNewByte(byte b, ref bool sessionInit, ref bool sessionClose, ref bool HeaderInProgress, ref bool ErrorInData)
            {
                if (b == Convert.ToByte('*'))
                {
                    HeaderInProgress = true;
                    receivedbuffer = "";
                    return;
                }
                else if (b == 24) 
                {
                    Array.Resize<byte>(ref AbortFrame, AbortFrame.Length + 1);
                    AbortFrame[AbortFrame.Length - 1] = b;
                    return;
                }
                else if ((b == 8) && (AbortFrame.Length >= 8))
                {
                    Array.Resize<byte>(ref AbortFrame, AbortFrame.Length + 1);
                    AbortFrame[AbortFrame.Length - 1] = b;
                    return;
                }
                else if ((b == 0) && (AbortFrame.Length >= 16))
                {
                    Console.WriteLine("Session aborted by sender!");
                    sessionClose = true;
                }
                else
                {
                    AbortFrame = new byte[0];
                }

                receivedbuffer += Convert.ToChar(b);
                if (receivedbuffer.Contains("rz\r")) // echo from session init request
                {
                    sessionInit = true;
                    sessionClose = false;
                    receivedbuffer = "";
                    return;
                }
                if (receivedbuffer.Contains("sz"))
                {
                    return;
                }
                else if (receivedbuffer.Contains("OO"))
                {
                    sessionInit = false;
                    sessionClose = true;
                    receivedbuffer = "";
                    return;
                }
                else if ((receivedbuffer.Length >2)  && (sessionInit))
                {
                    ErrorInData = true;
                    receivedbuffer = "";
                    return;
                    throw new Exception("Unknow case");
                }
            }
        }
    }
}

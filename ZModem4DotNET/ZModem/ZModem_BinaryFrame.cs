using System;

namespace ZModem_Protocol
{
    public partial class ZModem
    {
        /// <summary>
        /// internal class used to decrypt a ZDATA frame
        /// </summary>
        class BinaryFrame
        {
            public Byte[] data;
            public Byte EndOfFrame;
            public Byte[] crc;
            public int nbvalue;
            public bool state = false;
            public int LastIndexOfFrame;

            public BinaryFrame()
            {
                data = new Byte[0];
                nbvalue = 0;
                crc = new Byte[2];
            }

            public bool Addvalue(Byte b)
            {
                if (nbvalue == data.Length)
                    Array.Resize(ref data, nbvalue + 1);

                data[nbvalue] = b;
                nbvalue++;
                return true;
            }

            /// <summary>
            /// Used to check transmission errors
            /// </summary>
            /// <returns>true if frame is valid, false if a transmission error occured</returns>
            public bool CheckCRC()
            {
                Crc16Ccitt crc16 = new Crc16Ccitt(InitialCrcValue.Zeros);
                Byte[] b = new Byte[nbvalue + 1];
                for (int i = 0; i < nbvalue; i++)
                {
                    b[i] = Convert.ToByte(data[i]);
                }
                b[nbvalue] = Convert.ToByte(EndOfFrame);
                Byte[] r = crc16.ComputeChecksumBytes(b);

                if ((r[0] == crc[1]) && (r[1] == crc[0]))
                    state = true;
                else
                    state = false;
                return state;
            }
        }

 
    }
}

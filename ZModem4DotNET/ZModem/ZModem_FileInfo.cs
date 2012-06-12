using System;

namespace ZModem_Protocol
{
    partial class ZModem
    {
        /// <summary>
        /// Internal class used to decode File Information sent during ZModem session
        /// </summary>
        class ZFileInfo
        {
            string fileName = "";
            string fileSize = "";
            ZFILE_Header_ZF0 zF0;
            ZFILE_Header_ZF1 zF1;
            ZFILE_Header_ZF2 zF2;
            ZFILE_Header_ZF3 zF3;
            public int endofframe;
            public int size;

            /// <summary>
            /// Constructor, decode a ZFILE frame to readable information
            /// </summary>
            /// <param name="h">ZFILE header</param>
            /// <param name="data">Received Data</param>
            public ZFileInfo(Header h, Byte[] data)
            {
                zF0 = (ZFILE_Header_ZF0)((int)h.arg1);
                zF1 = (ZFILE_Header_ZF1)((int)h.arg2);
                zF2 = (ZFILE_Header_ZF2)((int)h.arg3);
                zF3 = (ZFILE_Header_ZF3)((int)h.arg4);

                int frameindex = h.endofHeader + 1;
                while (data[frameindex] != 0)
                { // Get Name
                    fileName += Convert.ToChar(data[frameindex]).ToString();
                    frameindex++;
                    if (frameindex > data.Length)
                        return; // la frame n'est pas complète!!
                }

                frameindex++;
                if (data[frameindex] == 0)
                {//End of ZFILE if only the name is transmitted
                    endofframe = frameindex + 1;
                    return;
                }

                while (data[frameindex] != 32)
                { // Get Size
                    fileSize += Convert.ToChar(data[frameindex]).ToString();
                    frameindex++;
                    if (frameindex > data.Length)
                        return; // la frame n'est pas complète!!
                }
                size = Convert.ToInt32(fileSize);

                //get following, but not used
                while (data[frameindex] != 0)
                { // Get Modification date
                    frameindex++;
                }
                //while (data[frameindex] != 32)
                //{ // Get Modification date
                //    frameindex++;
                //}
                //while (data[frameindex] != 32)
                //{ // Get FileMode
                //    frameindex++;
                //}
                //while (data[frameindex] != 32)
                //{ // Get serialNumber
                //    frameindex++;
                //}
                //while (data[frameindex] != 32)
                //{ // Get Number of files
                //    frameindex++;
                //}
                //while (data[frameindex] != 32)
                //{ // Get Number of bytes
                //    frameindex++;
                //}
                //while (data[frameindex] != 32)
                //{ // Get File type Iff
                //    frameindex++;
                //}
                endofframe = frameindex;
            }

            public override string ToString()
            {
                return "File Name :\t" + fileName + "\r\nFile Size :\t" + fileSize;
            }
        }
    }
}
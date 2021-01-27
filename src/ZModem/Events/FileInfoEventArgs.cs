using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace ZModem_Protocol
{
    //Event for FileInfo
    public class FileInfoEventArgs : EventArgs
    {
        public string FileName { get; }
        public int Size { get; }

        public FileInfoEventArgs(string fileName, int size)
        {
            this.FileName = fileName;
            this.Size = size;
        }
    }
}

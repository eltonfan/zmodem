using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace ZModem_Protocol
{
    //Event for size
    public class BytesTransferedEventArgs : EventArgs
    {
        public int Size { get; }

        public BytesTransferedEventArgs(int size)
        {
            this.Size = size;
        }
    }
}

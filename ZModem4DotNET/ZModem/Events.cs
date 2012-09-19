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
        //Event for state
        public delegate void TransfertStateHandler(object sender, TransfertStateEventArgs e);
        public class TransfertStateEventArgs : EventArgs
        {
            public TransfertState state;

            public TransfertStateEventArgs(TransfertState state)
            {
                this.state = state;
            }
        }

        /// <summary>
        /// This event is used to communicate the session state
        /// </summary>
        public event TransfertStateHandler TransfertStateEvent;


        //Event for FileInfo
        public delegate void FileInfoHandler(object sender, FileInfoEventArgs e);
        public class FileInfoEventArgs : EventArgs
        {
            public string fileName;
            public int size;

            public FileInfoEventArgs(string fileName, int size)
            {
                this.fileName = fileName;
                this.size = size;
            }
        }
        /// <summary>
        /// This event is used to communicate the file name and size informations
        /// </summary>
        public event FileInfoHandler FileInfoEvent;


        //Event for size
        public delegate void BytesTransferedHandler(object sender, BytesTransferedEventArgs e);
        public class BytesTransferedEventArgs : EventArgs
        {
            public int size;

            public BytesTransferedEventArgs(int size)
            {
                this.size = size;
            }
        }
        /// <summary>
        /// This event is used to report progress during transfert.
        /// </summary>
        public event BytesTransferedHandler BytesTransferedEvent;


    }
}

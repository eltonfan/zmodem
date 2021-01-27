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
    partial class ZModem : IDisposable
    {
        /// <summary>
        /// This event is used to communicate the session state
        /// </summary>
        public event EventHandler<TransfertStateEventArgs> TransfertStateEvent;

        /// <summary>
        /// This event is used to communicate the file name and size informations
        /// </summary>
        public event EventHandler<FileInfoEventArgs> FileInfoEvent;

        /// <summary>
        /// This event is used to report progress during transfert.
        /// </summary>
        public event EventHandler<BytesTransferedEventArgs> BytesTransferedEvent;
    }
}

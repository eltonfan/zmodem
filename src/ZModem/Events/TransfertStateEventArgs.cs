using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace ZModem_Protocol
{
    //Event for state
    public class TransfertStateEventArgs : EventArgs
    {
        public TransfertState State { get; }

        public TransfertStateEventArgs(TransfertState state)
        {
            this.State = state;
        }
    }
}

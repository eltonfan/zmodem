using System;

namespace ZModem_Protocol
{
    /// <summary>
    /// This enumeration is used to describe the session state.
    /// </summary>
    public enum TransfertState
    {
        /// <summary> Session not initialized yet: closing SW is possible.</summary>
        Initializing,
        /// <summary> Session on-going: closing SW may freeze remote device for 60s.</summary>
        Initialized,
        /// <summary> Session on-going, transfert on-going: closing SW may crash remote device and serial port.</summary>
        Transfering,
        /// <summary> Session on-going, transfert Completed: closing SW may freeze remote device for 60s.</summary>
        Ended,
        /// <summary> Session closed: closing SW is possible.</summary>
        ClosingSession,
        /// <summary> An error occured. Will try to restore session.</summary>
        Error
    }
}
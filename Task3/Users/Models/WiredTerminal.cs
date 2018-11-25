using System;
using Task3.Stations.Enums;
using Task3.Stations.Models;
using Task3.Users.Interfaces;

namespace Task3.Users.Models
{
    public class WiredTerminal : ITerminal
    {
        public event EventHandler<StateEventArgs> TerminalStateChanged;

        public void ConnectToPort()
        {
            if (TerminalStateChanged != null)
            {
                TerminalStateChanged(this, new StateEventArgs(PortState.Connected));
            }
        }

        public void DisconnectFromPort()
        {
            if (TerminalStateChanged != null)
            {
                TerminalStateChanged(this, new StateEventArgs(PortState.Disconnected));
            }
        }

        public void Call(long phoneNumber)
        {
            if (TerminalStateChanged != null)
            {
                TerminalStateChanged(this, new StateEventArgs(PortState.Call, phoneNumber));
            }
        }

        public void Answer()
        {
            if (TerminalStateChanged != null)
            {
                TerminalStateChanged(this, new StateEventArgs(PortState.Answered));
            }
        }

        public void Reject()
        {
            if (TerminalStateChanged != null)
            {
                TerminalStateChanged(this, new StateEventArgs(PortState.Rejected));
            }
        }

        public void Complete()
        {
            if (TerminalStateChanged != null)
            {
                TerminalStateChanged(this, new StateEventArgs(PortState.Completed));
            }
        }

        public int GetAssignedPortsCount()
        {
            return TerminalStateChanged?.GetInvocationList().Length ?? 0;
        }
    }
}
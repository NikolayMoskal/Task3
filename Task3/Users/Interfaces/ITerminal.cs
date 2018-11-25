using System;
using Task3.Stations.Models;

namespace Task3.Users.Interfaces
{
    public interface ITerminal
    {
        event EventHandler<StateEventArgs> TerminalStateChanged;

        void ConnectToPort();
        void DisconnectFromPort();
        void Call(long phoneNumber);
        void Answer();
        void Reject();
        void Complete();
        int GetAssignedPortsCount();
    }
}
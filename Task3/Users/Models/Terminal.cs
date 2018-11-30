using System;
using Task3.Stations.Enums;
using Task3.Stations.Models;

namespace Task3.Users.Models
{
    public class Terminal
    {
        private Port _port;
        
        public event EventHandler<StateEventArgs> TerminalStateChanged;

        public void ConnectToPort(Port port)
        {
            _port = port ?? throw new ArgumentNullException(nameof(port));
            TerminalStateChanged += _port.OnTerminalStateChanged;
            
            _port.Connect();
        }

        public void DisconnectFromPort()
        {
            _port.Disconnect();

            TerminalStateChanged -= _port.OnTerminalStateChanged;
            _port = null;
        }

        public void Call(long phoneNumber = 0)
        {
            if (TerminalStateChanged != null)
            {
                TerminalStateChanged(this, new StateEventArgs(PortState.Call, phoneNumber));
            }
        }

        public void Complete()
        {
            if (TerminalStateChanged != null)
            {
                TerminalStateChanged(this, new StateEventArgs(PortState.Connected));
            }
        }
    }
}
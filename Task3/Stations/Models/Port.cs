using System;
using System.Text.RegularExpressions;
using Task3.Stations.Enums;
using Task3.Users.Models;

namespace Task3.Stations.Models
{
    public class Port
    {
        public long PhoneNumber { get; }
        public PortState State { get; private set; }

        public event EventHandler<StateEventArgs> PortStateChanged;
        
        public Port(long phoneNumber)
        {

            if (!new Regex(@"\d{6}").IsMatch(phoneNumber.ToString()))
            {
                throw new ArgumentException($"Phone number is incorrect. Example: 123456.");
            }

            PhoneNumber = phoneNumber;
            State = PortState.Disconnected;
        }

        public void Connect()
        {
            State = PortState.Connected;
        }

        public void Disconnect()
        {
            State = PortState.Disconnected;
        }

        public void OnTerminalStateChanged(object sender, StateEventArgs args)
        {
            if (!(sender is Terminal) || args == null)
            {
                return;
            }
            // разница состояний порта - до события и после
            // поднята трубка = Connected(1) -> Call(2) = 2-1 = 1 > 0
            // положена трубка = Call(2) -> Connected(1) = 1-2 = -1 < 0
            var arg = new StateEventArgs(args.State, args.PhoneNumber, (args.State - State) > 0);
            State = args.State;
            if (PortStateChanged != null)
            {
                PortStateChanged(this, arg);
            }
        }
    }
}
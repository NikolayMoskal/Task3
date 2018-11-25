using System;
using System.Text.RegularExpressions;
using Task3.Stations.Enums;
using Task3.Users.Interfaces;

namespace Task3.Stations.Models
{
    public class Port
    {
        public long PhoneNumber { get; }
        public PortState State { get; set; } // ??? не придумал, как сделать по-другому

        public event EventHandler<StateEventArgs> PortStateChanged;
        
        public Port(long phoneNumber)
        {

            if (!new Regex(@"\d{6}").IsMatch(phoneNumber.ToString()))
            {
                throw new ArgumentException($"Phone number is incorrect. Example: 440101.");
            }

            PhoneNumber = phoneNumber;
            State = PortState.Disconnected;
        }

        public void OnTerminalStateChanged(object sender, StateEventArgs args)
        {
            if (!(sender is ITerminal) || args == null)
            {
                return;
            }
            
            State = args.State;
            
            if (PortStateChanged != null)
            {
                PortStateChanged(this,
                    args.State == PortState.Completed
                        ? new CallCompletedEventArgs(PhoneNumber)
                        : args);
            }
        }
    }
}
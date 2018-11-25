using System;
using Task3.Stations.Enums;

namespace Task3.Stations.Models
{
    public class StateEventArgs : EventArgs
    {
        public PortState State { get; }
        public long PhoneNumber { get; }
        
        public StateEventArgs(PortState state)
        {
            State = state;
        }

        public StateEventArgs(PortState state, long phoneNumber) : this(state)
        {
            PhoneNumber = phoneNumber;
        }
    }
}
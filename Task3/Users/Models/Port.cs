using Task3.Users.Enums;

namespace Task3.Users.Models
{
    public class Port
    {
        public int Number { get; }
        public PortState State { get; private set; }

        public Port(int number)
        {
            Number = number;
            State = PortState.Disconnected;
        }

        public bool Connect()
        {
            if (State == PortState.Connected)
            {
                return false;
            }

            State = PortState.Connected;
            return true;
        }

        public bool Disconnect()
        {
            if (State == PortState.Disconnected)
            {
                return false;
            }

            State = PortState.Disconnected;
            return true;
        }
    }
}
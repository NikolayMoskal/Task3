namespace Task3.Users.Models
{
    public class SubscriberInfo
    {
        public int PhoneNumber { get; }
        public Port Port { get; }

        public SubscriberInfo(int phoneNumber, int port)
        {
            PhoneNumber = phoneNumber;
            Port = new Port(port);
        }
    }
}
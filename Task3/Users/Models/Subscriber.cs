using System;
using Task3.Stations;

namespace Task3.Users.Models
{
    public class Subscriber
    {
        private SubscriberInfo _info;
        public string Name { get; }

        public event Func<Subscriber, SubscriberInfo> OnGetNumber;
        public event Action<int> OnPortConnect;
        public event Action<int> OnPortDisconnect;

        public Subscriber(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"Invalid subscriber name: {name}");
            }
            
            Name = name;
        }

        public void Subscribe(PhoneStation station)
        {
            if (station == null)
            {
                throw new ArgumentNullException(nameof(station));
            }
            _info = OnGetNumber?.Invoke(this);
        }

        public void ConnectToPort()
        {
            OnPortConnect?.Invoke(_info.Port.Number);
        }

        public void DisconnectFromPort()
        {
            OnPortDisconnect?.Invoke(_info.Port.Number);
        }
    }
}
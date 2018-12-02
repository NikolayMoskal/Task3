using System;
using System.Collections.Generic;
using NLog;
using Task3.Billings;
using Task3.Stations.Models;

namespace Task3.Users.Models
{
    public class Subscriber
    {
        private readonly List<Terminal> _terminals;
        private Terminal _activeTerminal;
        public long PhoneNumber { get; private set; }
        public string Name { get; }
        
        public Subscriber(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"Invalid subscriber name: {name}");
            }

            Name = name;
            _terminals = new List<Terminal>(0);
        }

        public void Subscribe(PhoneStation station, TariffPlan plan)
        {
            if (station == null)
            {
                throw new ArgumentNullException(nameof(station));
            }

            if (plan == null)
            {
                throw new ArgumentNullException(nameof(plan));
            }

            PhoneNumber = station.AddSubscriber(this, plan);
        }

        public void AddTerminal(Terminal terminal)
        {
            if (terminal == null)
            {
                throw new ArgumentNullException(nameof(terminal));
            }

            _terminals.Add(terminal);
        }

        public void RemoveTerminal(Terminal terminal)
        {
            var result = _terminals.Find(x => x == terminal);
            if (result != null)
            {
                _terminals.Remove(terminal);
            }
        }

        public void PickUpPhone(Terminal terminal, long phoneNumber = 0)
        {
            var phone = _terminals.Find(x => x == terminal);
            if (phone == null)
            {
                throw new ArgumentNullException(nameof(terminal));
            }
            
            phone.Call(phoneNumber);
            _activeTerminal = phone;
        }

        public void HangUp()
        {
            if (_activeTerminal != null)
            {
                _activeTerminal.Complete();
                _activeTerminal = null;
            }
        }
    }
}
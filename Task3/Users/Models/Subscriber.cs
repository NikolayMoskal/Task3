using System;
using System.Collections.Generic;
using Task3.Billings;
using Task3.Stations.Models;

namespace Task3.Users.Models
{
    public class Subscriber
    {
        private readonly List<Terminal> _terminals;
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
    }
}
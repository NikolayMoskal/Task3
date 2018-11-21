using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using Task3.Users.Models;

namespace Task3.Stations
{
    public class PhoneStation
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly int _prefix;
        private readonly Dictionary<SubscriberInfo, Subscriber> _subscribers;

        public Dictionary<SubscriberInfo, Subscriber> Subscribers =>
            new Dictionary<SubscriberInfo, Subscriber>(_subscribers);

        public PhoneStation(int prefix)
        {
            if (!new Regex(@"\d{2}").IsMatch(prefix.ToString()))
            {
                throw new ArgumentException($"Phone prefix is incorrect: {prefix}");
            }

            _prefix = prefix;
            _subscribers = new Dictionary<SubscriberInfo, Subscriber>(0);
        }

        private int GenerateNumber(int prefix, int digits)
        {
            var random = new Random();
            int number;
            var from = Convert.ToInt32(prefix * Math.Pow(10, digits));
            var to = Convert.ToInt32(from + Math.Pow(10, digits - 1));
            do
            {
                number = random.Next(from, to);
            } while (_subscribers.Any(x => x.Key.PhoneNumber == number));

            return number;
        }

        private int GeneratePort()
        {
            var random = new Random();
            int port;
            do
            {
                port = random.Next(1000, 10000);
            } while (_subscribers.Any(x => x.Key.Port.Number == port));

            return port;
        }

        public SubscriberInfo AddSubscriber(Subscriber subscriber)
        {
            var number = GenerateNumber(_prefix, 4);
            var port = GeneratePort();
            var subscriberInfo = new SubscriberInfo(number, port);
            _subscribers.Add(subscriberInfo, subscriber);

            return subscriberInfo;
        }

        public void ConnectPort(int portNumber)
        {
            var port = _subscribers
                .First(x => x.Key.Port.Number == portNumber)
                .Key.Port;
            if (port != null)
            {
                Logger.Info(port.Connect()
                    ? $"Port {portNumber} is connected."
                    : $"Port {portNumber} is already connected.");
            }
        }

        public void DisconnectPort(int portNumber)
        {
            var port = _subscribers
                .First(x => x.Key.Port.Number == portNumber)
                .Key.Port;
            if (port != null)
            {
                Logger.Info(port.Disconnect()
                    ? $"Port {portNumber} is disconnected."
                    : $"Port {portNumber} is already disconnected.");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using Task3.Billings;
using Task3.Stations.Enums;
using Task3.Users.Models;

namespace Task3.Stations.Models
{
    public class PhoneStation
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly int _prefix;
        private readonly List<Entry> _subscribers;
        private readonly List<Connection> _connectionPool;

        public event EventHandler<CallCompletedEventArgs> CallCompleted;

        public PhoneStation(int prefix)
        {
            if (!new Regex(@"\d{2}").IsMatch(prefix.ToString()))
            {
                throw new ArgumentException($"Phone prefix is incorrect: {prefix}");
            }

            _prefix = prefix;
            _subscribers = new List<Entry>(0);
            _connectionPool = new List<Connection>(0);
        }

        public void AddBillingSystem(BillingSystem system)
        {
            if (system != null)
            {
                CallCompleted += system.OnCallCompleted;
            }
        }

        public void RemoveBillingSystem(BillingSystem system)
        {
            if (system != null)
            {
                CallCompleted -= system.OnCallCompleted;
            }
        }

        private long GenerateNumber(int prefix, int digits)
        {
            var random = new Random();
            int number;
            var from = Convert.ToInt32(prefix * Math.Pow(10, digits));
            var to = Convert.ToInt32(from + Math.Pow(10, digits - 1));
            do
            {
                number = random.Next(from, to);
            } while (_subscribers.Any(x => x.Subscriber.PhoneNumber == number));

            return number;
        }

        public long AddSubscriber(Subscriber subscriber, TariffPlan plan)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }
            if (plan == null)
            {
                throw new ArgumentNullException(nameof(plan));
            }

            var number = GenerateNumber(_prefix, 4);
            var port = new Port(number);
            port.PortStateChanged += OnPortStateChanged;
            _subscribers.Add(new Entry(port, subscriber, plan));

            return number;
        }

        public void DeleteSubscriber(Subscriber subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }

            var entry = _subscribers.FirstOrDefault(x => x.Subscriber.PhoneNumber == subscriber.PhoneNumber);
            if (entry != null)
            {
                entry.Port.PortStateChanged -= OnPortStateChanged;
                _subscribers.Remove(entry);
            }
        }

        public Port SelectPort(long phoneNumber)
        {
            return _subscribers
                .FirstOrDefault(x => x.Port.PhoneNumber == phoneNumber)
                ?.Port;
        }

        private Port GetConnectionSender(Port port)
        {
            return _connectionPool.Find(x => x.Receiver == port).Sender;
        }

        private void RemoveConnection(Port sender, Port receiver)
        {
            var conn = _connectionPool.Find(x => x.Sender == sender && x.Receiver == receiver);
            if (conn != null)
            {
                OnConnectionClosing(conn);
                _connectionPool.Remove(conn);
            }
        }

        private void CreateConnection(Port sender, Port receiver)
        {
            var senderIsLocked = _connectionPool.Any(x => x.Sender == sender || x.Receiver == sender);
            var receiverIsLocked = _connectionPool.Any(x => x.Sender == receiver || x.Receiver == receiver);
            if (!senderIsLocked && !receiverIsLocked)
            {
                var conn = new Connection(sender, receiver);
                _connectionPool.Add(conn);
            }
            else
            {
                Logger.Warn($"The line is busy. Try later.");
            }
        }

        private void ManageConnection(Port sender, Port receiver, bool isPickedUpPhone, bool isAnswered)
        {
            if (!isPickedUpPhone) // если трубка не поднята или была опущена, то ищем и закрываем соединение
            {
                RemoveConnection(sender, receiver);
                RemoveConnection(receiver, sender);
                return;
            }

            if (sender.State == PortState.Call && !isAnswered) // попытка инициации нового соединения
            {
                CreateConnection(sender, receiver);
            }
            
            if (receiver.State == PortState.Disconnected)
            {
                Logger.Info($"The subscriber {receiver.PhoneNumber} is unreachable. Try it later.");
            }
        }

        private void OnPortStateChanged(object sender, StateEventArgs args)
        {
            if (!(sender is Port port))
            {
                return;
            }
            
            var senderPort = SelectPort(port.PhoneNumber);
            if (senderPort == null)
            {
                Logger.Warn($"This number {port.PhoneNumber} is not in service.");
                return;
            }
            
            var receiverPort = args.PhoneNumber != 0 ? SelectPort(args.PhoneNumber) : GetConnectionSender(senderPort);
            if (receiverPort == null)
            {
                Logger.Warn($"This number {args.PhoneNumber} is not in service.");
                return;
            }
            
            ManageConnection(senderPort, receiverPort, args.IsPickedUpPhone, args.PhoneNumber == 0);
        }

        private void OnConnectionClosing(object sender)
        {
            if (!(sender is Connection conn))
            {
                return;
            }
            
            if (CallCompleted != null)
            {
                var senderTariff = _subscribers
                    .First(x => x.Port.PhoneNumber == conn.Sender.PhoneNumber)
                    .TariffPlan;
                CallCompleted(this, 
                    new CallCompletedEventArgs(conn.Sender.PhoneNumber, conn.Receiver.PhoneNumber, senderTariff));
            }
        }

        private class Entry
        {
            public Port Port { get; }
            public Subscriber Subscriber { get; }
            public TariffPlan TariffPlan { get; }
            public long LinkedNumber { get; set; }

            public Entry(Port port, Subscriber subscriber, TariffPlan tariffPlan)
            {
                Port = port ?? throw new ArgumentNullException(nameof(port));
                Subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
                TariffPlan = tariffPlan ?? throw new ArgumentNullException(nameof(tariffPlan));
            }
        }

        private class Connection
        {
            public Port Sender { get; }
            public Port Receiver { get; }

            public Connection(Port sender, Port receiver)
            {
                Sender = sender ?? throw new ArgumentNullException(nameof(sender));
                Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            }
        }
    }
}
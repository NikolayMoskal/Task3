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

        public event EventHandler<CallCompletedEventArgs> CallCompleted;

        public PhoneStation(int prefix)
        {
            if (!new Regex(@"\d{2}").IsMatch(prefix.ToString()))
            {
                throw new ArgumentException($"Phone prefix is incorrect: {prefix}");
            }

            _prefix = prefix;
            _subscribers = new List<Entry>(0);
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
            Helper.AssignPortToStation(port, this);
            _subscribers.Add(new Entry(port, subscriber, plan));

            return number;
        }

        public Port SelectPort(long phoneNumber)
        {
            return _subscribers
                .First(x => x.Subscriber.PhoneNumber == phoneNumber)
                .Port;
        }

        public void OnPortStateChanged(object sender, StateEventArgs args)
        {
            if (!(sender is Port port))
            {
                return;
            }

            // ищем свой порт в списке станции
            var ownEntry = _subscribers.FirstOrDefault(x => x.Port.PhoneNumber == port.PhoneNumber);
            if (ownEntry == null)
            {
                return;
            }
            
            Logger.Info($"The subscriber {port.PhoneNumber: ##-##-##} is {args.State}");

            switch (ownEntry.Port.State)
            {
                case PortState.Disconnected:
                    throw new Exception($"The port by number {ownEntry.Port.PhoneNumber} is disconnected. Connect it first.");
                case PortState.Call:
                {
                    var linkedEntry = _subscribers.FirstOrDefault(x => x.Port.PhoneNumber == args.PhoneNumber);
                    if (linkedEntry != null)
                    {
                        if (linkedEntry.Port.State == PortState.Disconnected)
                        {
                            throw new Exception(
                                $"The subscriber {linkedEntry.Port.PhoneNumber} is unreachable. Try it later.");
                        }

                        linkedEntry.Port.State = args.State;
                        linkedEntry.LinkedNumber = args.PhoneNumber;
                    }

                    break;
                }
                case PortState.Completed when CallCompleted != null:
                {
                    if (!(args is CallCompletedEventArgs arg))
                    {
                        throw new ArgumentNullException(nameof(arg));
                    }

                    CallCompleted(this, new CallCompletedEventArgs(arg.PhoneNumber, ownEntry.LinkedNumber, ownEntry.TariffPlan));
                    _subscribers.First(x => x.Port.PhoneNumber == ownEntry.LinkedNumber).Port.State = PortState.Connected;
                    ownEntry.Port.State = PortState.Connected;
                    break;
                }
            }
        }

        public int GetAssignedBillingsCount()
        {
            return CallCompleted?.GetInvocationList().Length ?? 0;
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
    }
}
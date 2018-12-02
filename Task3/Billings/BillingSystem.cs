using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Task3.Stations.Models;
using Task3.Users.Models;

namespace Task3.Billings
{
    public class BillingSystem
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); 
        private readonly List<BillingSystemEntry> _entries;

        public BillingSystem()
        {
            _entries = new List<BillingSystemEntry>(0);
        }

        public void OnCallCompleted(object sender, CallCompletedEventArgs args)
        {
            if (!(sender is PhoneStation) || args == null)
            {
                return;
            }

            Logger.Info($"The call is completed. Duration: {args.CallDuration}s.");
            
            var price = args.Plan.Calculate(args.CallDuration);
            _entries.Add(new BillingSystemEntry(args.SenderNumber, args.ReceiverNumber, args.CallDuration, price));
        }

        public List<BillingSystemEntry> GenerateReport(Subscriber subscriber, int days)
        {
            var now = DateTime.Now;
            return _entries
                .Where(x => x.Sender == subscriber.PhoneNumber)
                .Where(x => (now - x.CallDate).Days <= days)
                .ToList();
        }
    }
}
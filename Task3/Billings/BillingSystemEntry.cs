using System;

namespace Task3.Billings
{
    public class BillingSystemEntry
    {
        public DateTime CallDate { get; }
        public long Sender { get; }
        public long Receiver { get; }
        public int CallDuration { get; }
        public double Price { get; }
        
        public BillingSystemEntry(long sender, long receiver, int callDuration, double price)
        {
            CallDate = DateTime.Now;
            Sender = sender;
            Receiver = receiver;
            CallDuration = callDuration;
            Price = price;
        }
    }
}
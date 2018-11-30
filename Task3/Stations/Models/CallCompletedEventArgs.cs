using System;
using Task3.Billings;
using Task3.Stations.Enums;

namespace Task3.Stations.Models
{
    public class CallCompletedEventArgs : EventArgs
    {
        public long SenderNumber { get; }
        public long ReceiverNumber { get; }
        public int CallDuration { get; }
        public TariffPlan Plan { get; }
        
        public CallCompletedEventArgs(long senderNumber, long receiverNumber, TariffPlan plan)
        {
            SenderNumber = senderNumber;
            ReceiverNumber = receiverNumber;
            CallDuration = new Random().Next(1, 3601);
            Plan = plan ?? throw new ArgumentNullException(nameof(plan));
        }
    }
}
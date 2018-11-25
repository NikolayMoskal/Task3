using System;
using Task3.Billings;
using Task3.Stations.Enums;

namespace Task3.Stations.Models
{
    public class CallCompletedEventArgs : StateEventArgs
    {
        public long LinkedNumber { get; }
        public int CallDuration { get; }
        public TariffPlan Plan { get; }

        public CallCompletedEventArgs(long phoneNumber, long linkedNumber = 0) : base(PortState.Completed, phoneNumber)
        {
            LinkedNumber = linkedNumber;
            CallDuration = new Random().Next(1, 3601);
        }
        
        public CallCompletedEventArgs(long phoneNumber, long linkedNumber, TariffPlan plan) 
            : this(phoneNumber, linkedNumber)
        {
            Plan = plan ?? throw new ArgumentNullException(nameof(plan));
        }
    }
}
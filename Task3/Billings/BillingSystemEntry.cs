namespace Task3.Billings
{
    public class BillingSystemEntry
    {
        public long OutgoingNumber { get; }
        public long PhoneNumber { get; }
        public int CallDuration { get; }
        public double Price { get; }
        
        public BillingSystemEntry(long outgoingNumber, long phoneNumber, int callDuration, double price)
        {
            OutgoingNumber = outgoingNumber;
            PhoneNumber = phoneNumber;
            CallDuration = callDuration;
            Price = price;
        }
    }
}
using System;
using NLog;
using Task3.Stations;
using Task3.Users.Models;

namespace Task3
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var station = new PhoneStation(44);
            var subscriber = new Subscriber("Nikolay");

            subscriber.OnGetNumber += station.AddSubscriber;
            subscriber.OnPortConnect += station.ConnectPort;
            subscriber.OnPortDisconnect += station.DisconnectPort;
            
            subscriber.Subscribe(station);
            subscriber.ConnectToPort();
        }
    }
}
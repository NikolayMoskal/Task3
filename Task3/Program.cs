using System;
using NLog;
using Task3.Billings;
using Task3.Stations.Models;
using Task3.Users.Models;

namespace Task3
{
    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public static void Main(string[] args)
        {
            try
            {
                var station = new PhoneStation(44);
                station.AddBillingSystem(new BillingSystem());

                var subscriber = new Subscriber("PersonA");
                subscriber.Subscribe(station, new TariffPlan("SomeTariff"));
                var terminal = new Terminal();
                var port = station.SelectPort(subscriber.PhoneNumber);
                subscriber.AddTerminal(terminal);

                var subscriber2 = new Subscriber("PersonB");
                subscriber2.Subscribe(station, new TariffPlan("SomeTariff"));
                var terminal2 = new Terminal();
                var port2 = station.SelectPort(subscriber2.PhoneNumber);
                subscriber2.AddTerminal(terminal2);
                
                var subscriber3 = new Subscriber("PersonC");
                subscriber3.Subscribe(station, new TariffPlan("SomeTariff"));
                var terminal3 = new Terminal();
                var port3 = station.SelectPort(subscriber3.PhoneNumber);
                subscriber3.AddTerminal(terminal3);

                terminal.ConnectToPort(port);
                terminal2.ConnectToPort(port2);
                terminal3.ConnectToPort(port3);
                
                subscriber.PickUpPhone(terminal, subscriber2.PhoneNumber);
                subscriber2.PickUpPhone(terminal2);
                subscriber3.PickUpPhone(terminal3, subscriber2.PhoneNumber);
                
                subscriber2.HangUp();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}
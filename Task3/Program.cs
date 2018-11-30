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

                terminal.ConnectToPort(port);
                terminal2.ConnectToPort(port2);
                
                terminal.Call(subscriber2.PhoneNumber);
                terminal2.Call();
                
                terminal2.Complete();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}
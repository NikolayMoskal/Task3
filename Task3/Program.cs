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
                var billing = new BillingSystem();
                Helper.AssignBillingSystem(station, billing);

                var subscriber = new Subscriber("PersonA");
                subscriber.Subscribe(station, new TariffPlan("SomeTariff"));
                var terminal = new WiredTerminal();
                var port = station.SelectPort(subscriber.PhoneNumber);
                subscriber.AddTerminal(terminal);
                Helper.AssignTerminalToPort(terminal, port);

                var subscriber2 = new Subscriber("PersonB");
                subscriber2.Subscribe(station, new TariffPlan("SomeTariff"));
                var terminal2 = new WiredTerminal();
                var port2 = station.SelectPort(subscriber2.PhoneNumber);
                subscriber2.AddTerminal(terminal2);
                Helper.AssignTerminalToPort(terminal2, port2);

                terminal.ConnectToPort();
                terminal2.ConnectToPort();
                
                terminal.Call(subscriber2.PhoneNumber);
                terminal2.Answer();

                terminal2.Complete();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}
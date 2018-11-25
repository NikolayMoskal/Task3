using System;
using Task3.Billings;
using Task3.Users.Interfaces;

namespace Task3.Stations.Models
{
    public static class Helper
    {
        public static void AssignPortToStation(Port port, PhoneStation station)
        {
            if (port == null)
            {
                throw new ArgumentNullException(nameof(port));
            }

            if (station == null)
            {
                throw new ArgumentNullException(nameof(station));
            }

            port.PortStateChanged += station.OnPortStateChanged;
        }

        public static void AssignTerminalToPort(ITerminal terminal, Port port)
        {
            if (terminal == null)
            {
                throw new ArgumentNullException(nameof(terminal));
            }

            if (port == null)
            {
                throw new ArgumentNullException(nameof(port));
            }

            if (terminal.GetAssignedPortsCount() != 0)
            {
                throw new Exception($"This terminal is already assigned to port of phone number #{port.PhoneNumber}");
            }

            terminal.TerminalStateChanged += port.OnTerminalStateChanged;
        }

        public static void AssignBillingSystem(PhoneStation station, BillingSystem system)
        {
            if (station == null)
            {
                throw new ArgumentNullException(nameof(station));
            }

            if (system == null)
            {
                throw new ArgumentNullException(nameof(system));
            }

            if (station.GetAssignedBillingsCount() != 0)
            {
                throw new Exception($"This phone station has already assigned billing systems");
            }

            station.CallCompleted += system.OnCallCompleted;
        }
    }
}
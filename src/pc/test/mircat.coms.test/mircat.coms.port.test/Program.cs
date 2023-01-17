using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace psc.mircat.coms.port.test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(" ------------------------------------");
                Console.WriteLine(" PSC test of MIRcat VCP communication");
                Console.WriteLine(" ------------------------------------");

                using (var mircat = new MIRcat())
                {
                    mircat.PortName = "COM5";
                    mircat.BaudRate = 921600;
                    mircat.Parity = System.IO.Ports.Parity.None;
                    mircat.DataBits = 8;
                    mircat.StopBits = System.IO.Ports.StopBits.One;
                    mircat.Handshake = System.IO.Ports.Handshake.RequestToSend;

                    Console.WriteLine("\n[Information] port configuration: ");
                    Console.WriteLine($"    port:      {mircat.PortName}");
                    Console.WriteLine($"    baud:      {mircat.BaudRate}");
                    Console.WriteLine($"    data bits: {mircat.DataBits}");
                    Console.WriteLine($"    parity:    {mircat.Parity}");
                    Console.WriteLine($"    stop bits: {mircat.StopBits}");
                    Console.WriteLine($"    handshake: {mircat.Handshake}");

                    Console.WriteLine("\n[Information] initiating connection...");
                    mircat.Connect();

                    Console.WriteLine($"\n[Information] connection {(mircat.IsConnected ? "successful" : "** FAIL **")}");


                    Console.WriteLine($"\n[Information] sending '{MIRcat.Command.GetLightInformation}' command...");
                    mircat.GetLightInformation();


                    Console.WriteLine("\n[Information] disconnecting...");
                    mircat.Disconnect();

                    Console.WriteLine($"\n[Information] disconnection {(!mircat.IsConnected ? "successful" : "** FAIL **")}");
                }

                Console.WriteLine("\n[Information] Exiting normally...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("\nPress any key to close...");
            Console.ReadKey();
        }
    }
}

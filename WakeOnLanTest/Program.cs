using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace WakeOnLanTest
{
    class Program
    {
        private const int PORT = 9;
        static void Main(string[] args)
        {
            IPAddress targetBroadcast;
            if (args.Length == 0)
            {
                Console.WriteLine("What IP address would you like the captured packages to be relayed to?");
                Console.WriteLine();
                Console.Write("=> ");
                targetBroadcast = IPAddress.Parse(Console.ReadLine());
                Console.Clear();
            }
            else
                targetBroadcast = IPAddress.Parse(args[0]);
            Console.Title = $"Relaying all special packets to {targetBroadcast}";

            Console.WriteLine($"Binding to UDP port {PORT}...");
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var wolHelper = new WOLHelper(PORT);
            var ipEndpoint = new IPEndPoint(IPAddress.Any, PORT);
            socket.Bind(ipEndpoint);
            var endpoint = (EndPoint)ipEndpoint;
            Console.WriteLine("Ready to receive...");

            while (true)
            {
                try
                {
                    var data = new byte[256];
                    socket.ReceiveFrom(data, ref endpoint);

                    var localIps = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Select(i => i.ToString());
                    if (localIps.Contains(endpoint.ToString().Split(':')[0]))
                        continue;

                    Console.WriteLine($"Captured special packet from '{endpoint}' with target '{BitConverter.ToString(data, 6, 6)}'.");
                    wolHelper.Send(targetBroadcast, data, 3);
                    Console.WriteLine($"Packet was relayed succesfully.");
                    Console.WriteLine();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Encountered an error: {ex.Message}");
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }
    }
}

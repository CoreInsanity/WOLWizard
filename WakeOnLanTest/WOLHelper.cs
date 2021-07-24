using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WakeOnLanTest
{
    class WOLHelper
    {
        private UdpClient Client { get; }
        private int Port { get; }
        public WOLHelper(int port)
        {
            Client = new UdpClient();
            Port = port;
        }
        public void Send(IPAddress broadcastAddress, byte[] receivedBytes, int sendCount)
        {
            var magicPacket = Enumerable
                        .Repeat(byte.MaxValue, 6)
                        .Concat(Enumerable.Repeat(receivedBytes.ToList().GetRange(6, 12), 16).SelectMany(mac => mac))
                        .ToArray();
            for (int i = 0; i < sendCount; i++)
                Client.Send(magicPacket, magicPacket.Length, new IPEndPoint(broadcastAddress, Port));
        }
    }
}

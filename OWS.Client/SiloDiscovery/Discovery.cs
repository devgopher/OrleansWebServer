using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using IPAddressCollection = System.Net.IPAddressCollection;

namespace OWS.Client.SiloDiscovery
{
    public static class Discovery
    {
        public static List<string> ScanAddresses(int port)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var result = new List<string>();

            foreach (var iface in interfaces)
            {
                if (iface.NetworkInterfaceType == NetworkInterfaceType.Loopback) 
                    continue;

                Console.WriteLine(iface.Description);
                var unicastIpInfoCol = iface.GetIPProperties().UnicastAddresses;
                foreach (var addr in unicastIpInfoCol)
                {
                    if (addr.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (!addr.Address.ToString().StartsWith("192.168"))
                        continue;

                    Console.WriteLine("\tIP Address is {0}", addr.Address);
                    Console.WriteLine("\tSubnet Mask is {0}", addr.IPv4Mask);

                    var usableAddresses = GetUsableAddresses(addr);

                    Task.WaitAny(usableAddresses.Select(addr => Task.Run(() =>
                    {
                        if (Probe(addr, port)) 
                            result.Add($"{addr}");
                    })).ToArray());
                }
            }

            return result;
        }

        public static IPAddressCollection GetUsableAddresses(UnicastIPAddressInformation addr)
            => IPNetwork.Parse(addr.Address, addr.IPv4Mask).ListIPAddress(FilterEnum.Usable);


        public static bool Probe(IPAddress address, int port)
        {
            using var scan = new TcpClient();
            try
            {
                scan.SendTimeout = 10;
                scan.ReceiveTimeout = 10;
                scan.Connect(address, port);
            }
            catch
            {
                Console.WriteLine($"\t Probing {address}:{port}: false");
                return false;
            }

            Console.WriteLine($"\t Probing {address}:{port}: true");
            return true;
        }

        public static bool Probe(string address, int port)
            => Probe(IPAddress.Parse(address), port);
    }
}

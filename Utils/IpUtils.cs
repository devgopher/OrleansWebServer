using NetTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Utils
{
    public static class IpUtils
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine($"{nameof(GetLocalIPAddress)}() : {ip.ToString()}");
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static IPAddress IpAddress()
            => IPAddress.Parse(GetLocalIPAddress());

        public static IEnumerable<IPAddress> GetGatewayAddresses()
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            return adapters?.Select(a => a.GetIPProperties())
                .SelectMany(a => a.GatewayAddresses)
                .Select(r => r.Address);
        }

        public static IEnumerable<IPAddress> GetGatewayAddresses(NetworkInterface adapter)
            => adapter?.GetIPProperties().GatewayAddresses.Select(r => r.Address);

        public static bool Ping(IPAddress address, int timeout = 500)
        {
            var ping = new Ping();
            var rep = ping.Send(address, timeout);
            return rep.Status == IPStatus.Success;
        }



        private static int GetMaskLength(IPAddress address)
        {
            string a = "";
            address.ToString().Split('.').ToList().ForEach(x => a += Convert.ToInt32(x, 2).ToString());
            return a.Replace("0", "").Length;
        }

        public static IPAddress[] ScanLocalIps()
        {
            var results = new List<IPAddress>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var gateways = GetGatewayAddresses();
                foreach (var gw in gateways)
                {
                    var range = new IPAddressRange(gw, 24/*GetMaskLength(ni.GetIPProperties().UnicastAddresses.First().IPv4Mask)*/);
                    results.AddRange(range.AsEnumerable().Where(_ => Ping(_, 50)));
                }
            }

            return results.ToArray();
        }
    }
}

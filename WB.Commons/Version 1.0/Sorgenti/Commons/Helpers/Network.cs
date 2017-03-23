using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.IO;

namespace WB.Commons.Helpers
{
    public static class Network
    {
        /// <summary>
        /// Ritorna una lista degli ip presenti nella LAN
        /// </summary>
        /// <returns>Lista degli IP nella LAN</returns>
        public static IEnumerable<string> GetIps()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("net", "view");
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            Process proc = Process.Start(startInfo);
            StreamReader sr = proc.StandardOutput;
            var machines = GetMachines(sr.ReadToEnd());
            foreach (string machine in machines)
            {
                var ip = GetIp(machine);
                if (!string.IsNullOrEmpty(ip))
                    yield return ip;
            }
        }

        /// <summary>
        /// Ritorna una lista delle macchine sulla rete
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<string> GetMachines(string str)
        {
            string line = str.Substring(str.IndexOf("\\"));
            var machines = new List<string>();
            while (line.IndexOf("\\") != -1)
            {
                machines.Add(line.Substring(line.IndexOf("\\"),
                    line.IndexOf(" ", line.IndexOf("\\")) - line.IndexOf("\\")).Replace("\\", String.Empty));
                line = line.Substring(line.IndexOf(" ", line.IndexOf("\\") + 1));
            }
            return machines;
        }
        
        public static string GetIp(string server)
        {
            try
            {
                //IPHostEntry heserver = Dns.Resolve(server);
                IPHostEntry heserver = Dns.GetHostEntry(server);
                var found = heserver.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                return found != null? found.ToString() : string.Empty;
            }
            catch (SocketException ex)
            {

                Console.WriteLine(ex.Message + " Şu Serverda : " + server);
                return "";
            }
        }

        public static IPAddress GetLocalIPAddress()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            return IPAddress.None;
        }


        public static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bli.Common
{
    public class ConvertHelper
    {
        public static int Ipv4ToInt(IPAddress ipAddress)
        {
            byte[] ipAddressBytes = ipAddress.GetAddressBytes();
            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Array.Reverse(ipAddressBytes); 
            }

            return  BitConverter.ToInt32(ipAddressBytes, 0);
        }
        public static int Ipv4ToInt(string ipStr)
        {
            var ipAddress = IPAddress.Parse(ipStr);
            byte[] ipAddressBytes = ipAddress.GetAddressBytes();
            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Array.Reverse(ipAddressBytes);
            }

            return BitConverter.ToInt32(ipAddressBytes, 0);
        }
    }
}

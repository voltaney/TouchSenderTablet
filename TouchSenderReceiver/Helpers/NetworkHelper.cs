using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace TouchSenderReceiver.Helpers;

public class NetworkHelper
{
    /// <summary>
    /// Get all local IPv4 addresses of the specified network interface type.
    /// ref: https://stackoverflow.com/questions/6803073/get-local-ip-address
    /// </summary>
    /// <param name="networkInterfaceType"></param>
    /// <returns></returns>
    public static IList<string> GetAllLocalIPv4ByNIC(NetworkInterfaceType networkInterfaceType = NetworkInterfaceType.Ethernet)
    {
        List<string> ipAddrList = [];
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.NetworkInterfaceType == networkInterfaceType && item.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddrList.Add(ip.Address.ToString());
                    }
                }
            }
        }
        return ipAddrList;
    }

    /// <summary>
    /// Get all local IPv4 addresses of the current machine.
    /// </summary>
    public static IEnumerable<string> GetAllLocalIPv4()
    {
        string hostname = Dns.GetHostName();

        IPAddress[] adrList = Dns.GetHostAddresses(hostname);
        foreach (IPAddress address in adrList)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                yield return address.ToString();
            }
        }
    }
}

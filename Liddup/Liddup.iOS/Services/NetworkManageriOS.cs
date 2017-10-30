using System.Net.NetworkInformation;
using System.Net.Sockets;
using Couchbase.Lite;
using Couchbase.Lite.Listener.Tcp;
using Liddup.iOS.Services;
using Liddup.Services;
using NetworkExtension;
using Xamarin.Forms;

[assembly: Dependency(typeof(NetworkManageriOS))]
namespace Liddup.iOS.Services
{
    internal class NetworkManageriOS : INetworkManager
    {
        public void Start(Manager manager, ushort port)
        {
            var listener = new CouchbaseLiteTcpListener(manager, port);
            listener.Start();
        }

        public string GetIPAddress()
        {
            var ipAddress = "";

            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                    netInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                    if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        ipAddress = addrInfo.Address.ToString();
            }
            
            return ipAddress;
        }

        public string GetEncryptedIPAddress(string ip)
        {
            return null;
        }

        public string GetDecryptedIPAddress(string ip)
        {
            return null;
        }

        public void SetHotSpot(bool on)
        {
            var hotspotNetwork = new NEHotspotNetwork();
            
            hotspotNetwork.SetPassword("password");
            hotspotNetwork.Init();
            System.Diagnostics.Debug.WriteLine(hotspotNetwork.Ssid);
        }
    }
}
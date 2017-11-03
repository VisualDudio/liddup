﻿using System.Net.NetworkInformation;
using System.Net.Sockets;
using Couchbase.Lite;
using Couchbase.Lite.Listener.Tcp;
using Liddup.iOS.Services;
using Liddup.Services;
using NetworkExtension;
using Xamarin.Forms;
using System.Text;
using System;

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
            var ipComponents = ip.Split('.');
            var builder = new StringBuilder();
            var shift = new Random().Next(0, 100);

            foreach (var component in ipComponents)
            {
                //var convertedComponent = GetBaseConversion(int.Parse(component),
                //    Enumerable.Range('A', 26).Select(x => (char)x).ToArray());
                var convertedComponent = Convert.ToInt32(component).ToString("X");
                if (convertedComponent.Length == 1)
                    convertedComponent = "0" + convertedComponent;
                builder.Append(convertedComponent);
            }

            return builder.ToString();
        }

        public string GetDecryptedIPAddress(string ip)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < ip.Length; i += 2)
            {
                var ipComponent = Convert.ToInt32(ip.Substring(i, 2), 16);
                //var ipComponent = GetBaseConversion(int.Parse(ip.Substring(i, 2)),
                //    new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                builder.Append(ipComponent + ".");
            }
            return builder.Remove(builder.ToString().LastIndexOf('.'), 1).ToString();
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
using System;
using System.Net;
using System.Text;
using Android.Content;
using Android.Net.Wifi;
using Liddup.Droid.Services;
using Liddup.Services;
using Xamarin.Forms;
using Array = System.Array;

[assembly: Dependency(typeof(NetworkManagerAndroid))]
namespace Liddup.Droid.Services
{
    public class NetworkManagerAndroid : INetworkManager
    {
        public NetworkManagerAndroid() { }

        public string GetIPAddress()
        {
            var adresses = Dns.GetHostAddresses(Dns.GetHostName());

            return adresses[0]?.ToString();
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

        private static string GetBaseConversion(int value, char[] baseChars)
        {
            var i = 32;
            var buffer = new char[i];
            var targetBase = baseChars.Length;

            do
            {
                buffer[--i] = baseChars[value % targetBase];
                value = value / targetBase;
            }
            while (value > 0);

            var result = new char[32 - i];
            Array.Copy(buffer, i, result, 0, 32 - i);

            return new string(result);
        }

        public void SetHotSpot(bool on)
        {
            var wifiManager = (WifiManager)Forms.Context.GetSystemService(Context.WifiService);
            var wmMethods = wifiManager.Class.GetDeclaredMethods();
            var wifiConfiguration = new WifiConfiguration();
            var enabled = false;

            foreach (var method in wmMethods)
            {
                if (method.Name.Equals("isWifiApEnabled"))
                    enabled = (bool)method.Invoke(wifiManager);

                if (!method.Name.Equals("getWifiApConfiguration")) continue;
                try
                {
                    wifiConfiguration = (WifiConfiguration) method.Invoke(wifiManager, null);
                    Random random = new Random();

                    wifiConfiguration.Ssid = "Wifi Name " + random.Next(0, 10) + random.Next(0, 10) + random.Next(0, 10) + random.Next(0, 10);
                    wifiConfiguration.PreSharedKey = "Password" + random.Next(0, 10) + random.Next(0, 10);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if (on && !enabled)
            {
                foreach (var method in wmMethods)
                {
                    if (!method.Name.Equals("setWifiApEnabled")) continue;
                    try
                    {
                        method.Invoke(wifiManager, wifiConfiguration, true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    break;
                }
            }
            else if (!on && enabled)
            {
                foreach (var method in wmMethods)
                {
                    if (!method.Name.Equals("setWifiApEnabled")) continue;
                    try
                    {
                        method.Invoke(wifiManager, wifiConfiguration, false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }
    }
}
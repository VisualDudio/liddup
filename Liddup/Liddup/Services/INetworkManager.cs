using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Couchbase.Lite;

namespace Liddup.Services
{
    public interface INetworkManager
    {
        string GetIPAddress();
        string GetEncryptedIPAddress(string ip);
        string GetDecryptedIPAddress(string ip);
        void SetHotSpot(bool on);
    }
}

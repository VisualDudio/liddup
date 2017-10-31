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

namespace Liddup.Services
{
    public interface ISpotifyApi
    {
        string AccessToken { get; set; }
        bool IsLoggedIn { get; }

        void Login();
        void PlayTrack(string uri);
    }
}
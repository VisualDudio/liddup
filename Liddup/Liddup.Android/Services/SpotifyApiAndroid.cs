using Android.App;
using Android.Content;
using Android.Net;
using Com.Spotify.Sdk.Android.Authentication;
using Com.Spotify.Sdk.Android.Player;
using Liddup.Droid.Delegates;
using Liddup.Droid.Services;
using Liddup.Services;
using Xamarin.Forms;
using Error = Com.Spotify.Sdk.Android.Player.Error;

[assembly: Dependency(typeof(SpotifyApiAndroid))]
namespace Liddup.Droid.Services
{
    internal class SpotifyApiAndroid : Java.Lang.Object, ISpotifyApi, IPlayerNotificationCallback, IConnectionStateCallback
    {
        private const string ClientId = "969187cf9a3c48879a4c8e7376435aa3";
        private const string RedirectUri = "testschema://callback";
        private const int RequestCode = 1337;
        private PlaybackState _currentPlaybackState;
        private SpotifyPlayer _spotifyPlayer;
        private Metadata _metadata;
        private readonly OperationCallbackDelegate _operationCallbackDelegate = new OperationCallbackDelegate(() => LogStatus("Success!"), error => LogStatus("Error!"));

        public string AccessToken { get; set; }

        public bool IsLoggedIn => _spotifyPlayer != null && _spotifyPlayer.IsLoggedIn;

        public SpotifyApiAndroid()
        {
            if ((Activity)Forms.Context is MainActivity activity) activity.Destroy += HandleDestroy;
        }

        private void HandleDestroy(object sender, DestroyEventArgs e)
        {
            Spotify.DestroyPlayer(this);
        }

        public void Login()
        {
            string[] scopes = { "user-library-read", "user-read-private", "playlist-read", "playlist-read-private", "playlist-read-collaborative", "streaming" };
            var request = new AuthenticationRequest.Builder(ClientId, AuthenticationResponse.Type.Token, RedirectUri).SetScopes(scopes).Build();

            var activity = (Activity)Forms.Context as MainActivity;

            if (activity != null) activity.ActivityResult += HandleActivityResult;

            AuthenticationClient.OpenLoginActivity(activity, RequestCode, request);
        }

        public void InitPlayer(AuthenticationResponse response)
        {
            if (_spotifyPlayer == null)
            {
                AccessToken = response.AccessToken;

                var playerConfig = new Config(Forms.Context, response.AccessToken, ClientId);

                _spotifyPlayer = Spotify.GetPlayer(playerConfig, this, new InitializationObserverDelegate(p =>
                {
                    p.SetConnectivityStatus(_operationCallbackDelegate, GetNetworkConnectivity(Forms.Context));
                    p.AddNotificationCallback(this);
                    p.AddConnectionStateCallback(this);
                    p.Login(response.AccessToken);
                }, throwable => LogStatus(throwable.ToString())));
            }
            else
                _spotifyPlayer.Login(response.AccessToken);
        }

        private void HandleActivityResult(object sender, ActivityResultEventArgs e)
        {
            if (e.RequestCode != RequestCode) return;
            var response = AuthenticationClient.GetResponse((int)e.ResultCode, e.Data);
            if (response?.ResponseType == AuthenticationResponse.Type.Token)
                InitPlayer(response);
        }

        public void OnConnectionMessage(string message)
        {
            LogStatus("Incoming connection message: " + message);
        }

        public void OnLoggedIn()
        {
            LogStatus("Logged in");
        }

        public void OnLoggedOut()
        {
            LogStatus("Logged out");
        }

        public void OnLoginFailed(Error error)
        {
            LogStatus("Login failed! Error: " + error);
        }

        public void OnPlaybackError(Error error)
        {
            LogStatus("Playback error! Error: " + error);
        }

        public void OnPlaybackEvent(PlayerEvent p0)
        {
            _currentPlaybackState = _spotifyPlayer.PlaybackState;
            _metadata = _spotifyPlayer.Metadata;
        }

        public void OnTemporaryError()
        {
            LogStatus("Temporary error occurred");
        }

        public void PlayTrack(string uri)
        {
            _spotifyPlayer.PlayUri(_operationCallbackDelegate, uri, 0, 0);
        }

        private static void LogStatus(string status)
        {
            System.Diagnostics.Debug.WriteLine(status);
        }

        private static Connectivity GetNetworkConnectivity(Context context)
        {
            var connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            var activeNetwork = connectivityManager.ActiveNetworkInfo;
            if (activeNetwork != null && activeNetwork.IsConnected)
                return Connectivity.FromNetworkType((int)activeNetwork.Type);

            return Connectivity.Offline;
        }
    }
}
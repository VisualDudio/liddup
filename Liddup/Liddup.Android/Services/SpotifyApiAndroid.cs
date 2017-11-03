using Android.App;
using Android.Content;
using Android.Net;
using Com.Spotify.Sdk.Android.Authentication;
using Com.Spotify.Sdk.Android.Player;
using Liddup.Droid.Delegates;
using Liddup.Droid.Services;
using Liddup.Services;
using Liddup.Constants;
using Xamarin.Forms;
using Error = Com.Spotify.Sdk.Android.Player.Error;

[assembly: Dependency(typeof(SpotifyApiAndroid))]
namespace Liddup.Droid.Services
{
    internal class SpotifyApiAndroid : Java.Lang.Object, ISpotifyApi, IPlayerNotificationCallback, IConnectionStateCallback
    {
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
            var request = new AuthenticationRequest.Builder(ApiConstants.SpotifyClientId, AuthenticationResponse.Type.Token, ApiConstants.SpotifyRedirectUri).SetScopes(scopes).Build();

            var activity = (Activity)Forms.Context as MainActivity;

            if (activity != null) activity.ActivityResultDelegate += HandleActivityResultDelegate;

            AuthenticationClient.OpenLoginActivity(activity, ApiConstants.SpotifyRequestCode, request);
        }

        private void InitPlayer(string accessToken)
        {
            if (_spotifyPlayer == null)
            {
                AccessToken = accessToken;

                var playerConfig = new Config(Forms.Context, accessToken, ApiConstants.SpotifyClientId);

                _spotifyPlayer = Spotify.GetPlayer(playerConfig, this, new InitializationObserverDelegate(p =>
                {
                    p.SetConnectivityStatus(_operationCallbackDelegate, GetNetworkConnectivity(Forms.Context));
                    p.AddNotificationCallback(this);
                    p.AddConnectionStateCallback(this);
                    p.Login(accessToken);
                }, throwable => LogStatus(throwable.ToString())));
            }
            else
                _spotifyPlayer.Login(accessToken);
        }

        private void HandleActivityResultDelegate(object sender, ActivityResultEventArgs e)
        {
            if (e.RequestCode != ApiConstants.SpotifyRequestCode) return;
            var response = AuthenticationClient.GetResponse((int)e.ResultCode, e.Data);
            if (response?.ResponseType == AuthenticationResponse.Type.Token)
                InitPlayer(response?.AccessToken);
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

        public void OnPlaybackEvent(PlayerEvent e)
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
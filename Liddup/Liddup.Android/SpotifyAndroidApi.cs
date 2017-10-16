using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Liddup.Droid;

using Com.Spotify.Sdk.Android.Authentication;
using Com.Spotify.Sdk.Android.Player;
using Liddup.Droid.Delegates;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Error = Com.Spotify.Sdk.Android.Player.Error;

[assembly: Dependency(typeof(SpotifyAndroidApi))]
//TODO: Implement OnResume() and other MainActivity-invoked events
namespace Liddup.Droid
{
    class SpotifyAndroidApi : Java.Lang.Object, ISpotifyApi, IPlayerNotificationCallback, IConnectionStateCallback
    {
        public const string ClientId = "969187cf9a3c48879a4c8e7376435aa3";
        public const string RedirectUri = "testschema://callback";
        public int RequestCode = 1337;
        private PlaybackState _currentPlaybackState;
        private SpotifyPlayer _spotifyPlayer;
        private Metadata _metadata;
        private readonly OperationCallbackDelegate _operationCallbackDelegate = new OperationCallbackDelegate(() => LogStatus("Success!"), error => LogStatus("Error!"));

        public SpotifyAndroidApi()
        {
            if ((Activity)Forms.Context is MainActivity activity) activity.Destroy += HandleDestroy;
        }

        private void HandleDestroy(object sender, DestroyEventArgs e)
        {
            Spotify.DestroyPlayer(this);
        }

        public string AccessToken { get; set; }

        public bool IsLoggedIn => _spotifyPlayer != null && _spotifyPlayer.IsLoggedIn;

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
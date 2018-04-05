using System;
using Foundation;
using Liddup.Constants;
using Liddup.iOS.Delegates;
using Liddup.iOS.Services;
using Liddup.Services;
using SafariServices;
using SpotifyAuthenticationBindingiOS;
using SpotifyPlaybackBindingiOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SpotifyApiiOS))]
namespace Liddup.iOS.Services
{
    internal class SpotifyApiiOS : SPTAudioStreamingDelegate, ISPTAudioStreamingDelegate, IUIApplicationDelegate, ISpotifyApi
    {
        private SPTAuth _auth;
        private SPTAudioStreamingController _spotifyPlayer;
        private UIViewController _authViewController;
        private const string ClientId = ApiConstants.SpotifyClientId;
        private readonly NSUrl _redirectUrl = new NSUrl(ApiConstants.SpotifyRedirectUri);

        public string AccessToken { get; set; }

        public bool IsLoggedIn => _spotifyPlayer != null && _spotifyPlayer.LoggedIn;

        public SpotifyApiiOS() { }

        public void Login()
        {
            InitializeSpotify();
        }

        private void InitializeSpotify()
        {
            var scopes = new[]
            {
                SpotifyConstants.SPTAuthStreamingScope,
                SpotifyConstants.SPTAuthPlaylistReadPrivateScope,
                SpotifyConstants.SPTAuthUserLibraryReadScope,
                SpotifyConstants.SPTAuthUserReadPrivateScope
            };

            _auth = SPTAuth.DefaultInstance;
            _spotifyPlayer = SPTAudioStreamingController.SharedInstance();

            _auth.ClientID = ClientId;
            _auth.RequestedScopes = scopes;
            _auth.RedirectURL = _redirectUrl;

            _spotifyPlayer.Delegate = this;

            NSError error = null;

            try
            {
                _spotifyPlayer.StartWithClientId(ClientId, out error);
            }
            catch
            {
                Console.WriteLine(error);
                throw;
            }

            StartAuthenticationFlow();
        }

        private void StartAuthenticationFlow()
        {
            if (_auth.Session != null)
            {
                if (_auth.Session.IsValid)
                {
                    AccessToken = _auth.Session.AccessToken;
                    _spotifyPlayer.LoginWithAccessToken(_auth.Session.AccessToken);
                }
            }
            else
            {
                ((AppDelegate)UIApplication.SharedApplication.Delegate).OpenUrlDelegate += HandleOpenUrl;

                if (SPTAuth.SupportsApplicationAuthentication)
                    UIApplication.SharedApplication.OpenUrl(_auth.SpotifyAppAuthenticationURL);
                else
                {
                    _authViewController = new SFSafariViewController(_auth.SpotifyWebAuthenticationURL);
                    UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(_authViewController, true, null);
                }
            }
        }

        private void HandleOpenUrl(object sender, OpenUrlEventArgs e)
        {
            if (!_auth.CanHandleURL(e.Url)) return;

            _auth.HandleAuthCallbackWithTriggeredAuthURL(e.Url, (error, session) =>
            {
                if (error != null && session.IsValid)
                {
                    _auth.Session = session;
                    _spotifyPlayer.LoginWithAccessToken(_auth.Session.AccessToken);
                }
            });
        }

        public void PlayTrack(string uri)
        {
            _spotifyPlayer.PlaySpotifyURI(uri, 0, 0, null);
        }

        public void PauseTrack()
        {

        }

        public void ResumeTrack()
        {

        }

        public void Rewind()
        {
            _spotifyPlayer.SkipPrevious(null);
        }

        public void PlayPause()
        {
            _spotifyPlayer.SetIsPlaying(!_spotifyPlayer.PlaybackState.IsPlaying, null);
        }

        public void FastForward()
        {
            _spotifyPlayer.SkipNext(null);
        }

        public void SeekValueChanged()
        {

        }

        public void Logout()
        {
            _spotifyPlayer?.Logout();
        }

        public new void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
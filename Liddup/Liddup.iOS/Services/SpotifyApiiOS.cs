using System;
using Foundation;
using Liddup.Constants;
using Liddup.iOS.Delegates;
using Liddup.iOS.Services;
using Liddup.Services;
using SafariServices;
using SpotifyBindingiOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SpotifyApiiOS))]
namespace Liddup.iOS.Services
{
    internal class SpotifyApiiOS : SPTAudioStreamingDelegate, ISPTAudioStreamingDelegate, IUIApplicationDelegate, ISpotifyApi
    {
        private readonly SPTAuth _auth = SPTAuth.DefaultInstance;
        private SPTAudioStreamingController _spotifyPlayer = SPTAudioStreamingController.SharedInstance();
        private UIViewController _authViewController;
        private const string ClientId = ApiConstants.SpotifyClientId;
        private readonly NSUrl _redirectUrl = new NSUrl(ApiConstants.SpotifyRedirectUri);

        public string AccessToken { get; set; }

        public bool IsLoggedIn => _spotifyPlayer != null && _spotifyPlayer.LoggedIn;

        public SpotifyApiiOS()
        {

        }

        public void Login()
        {
            InitializeSpotify();
        }

        private void InitializeSpotify()
        {
            var scopes = new[]
            {
                Scopes.Streaming,
                Scopes.PlaylistReadPrivate,
                Scopes.UserLibraryRead,
                Scopes.UserReadPrivate
            };

            _spotifyPlayer = SPTAudioStreamingController.SharedInstance();
            _auth.ClientID = ClientId;
            _auth.RequestedScopes = scopes;
            _auth.RedirectURL = _redirectUrl;

            _spotifyPlayer.Delegate = (SPTAudioStreamingDelegate)UIApplication.SharedApplication.Delegate;

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
            if (_auth.Session.IsValid)
            {
                AccessToken = _auth.Session.AccessToken;
                _spotifyPlayer.LoginWithAccessToken(_auth.Session.AccessToken);
            }
            else
            {
                var authUrl = _auth.LoginURL;
                _authViewController = new SFSafariViewController(authUrl);

                ((AppDelegate)UIApplication.SharedApplication.Delegate).OpenUrlDelegate += HandleOpenUrl;

                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(_authViewController, true, null);
            }
        }

        private void HandleOpenUrl(object sender, OpenUrlEventArgs e)
        {
            if (!_auth.CanHandleURL(e.Url)) return;

            _authViewController.PresentingViewController.DismissViewController(true, null);
            _authViewController = null;

            _auth.HandleAuthCallbackWithTriggeredAuthURL(e.Url, (error, session) =>
            {
                if (session.IsValid)
                    _spotifyPlayer.LoginWithAccessToken(_auth.Session.AccessToken);
            });
        }

        public void PlayTrack(string uri)
        {
            _spotifyPlayer.PlaySpotifyURI(uri, 0, 0, null);
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
            
        }
    }
}
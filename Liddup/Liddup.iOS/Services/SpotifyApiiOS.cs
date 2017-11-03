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
        private SPTAuth _auth = SPTAuth.DefaultInstance;
        private const string ClientId = ApiConstants.SpotifyClientId;
        private readonly NSUrl _redirectUrl = new NSUrl(ApiConstants.SpotifyRedirectUri);

        public string AccessToken { get; set; }

        public bool IsLoggedIn => _spotifyPlayer != null && _spotifyPlayer.LoggedIn;

        public SpotifyApiiOS()
        {
            var result = new NSString[strings.Count];
            for (var i = 0; i < strings.Count; i++)
                result[i] = new NSString(strings[i]);

            return result;
        }

        private static NSString ConvertStringToNsString(string str)
        {
            return new NSString(str);    
        }

        public void Login()
        {
            if (SPTAuth.SupportsApplicationAuthentication)
            {
                var url = _auth.LoginURL;
                UIApplication.SharedApplication.OpenUrl(url);
            }
            else
            {
                
            }

            if (_auth.Session == null) return;
            var auth = SPTAuth.DefaultInstance;
            auth.RenewSession(auth.Session, (error, session) =>
            {
                auth.Session = session;
            });
        }

        public void InitializeSpotify()
        {
            var scopes = new []
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

                
            }
        }

        private static void StartLoginFlow()
        {
            
        }

        public bool OpenUrl(NSUrl url)
        {
            var auth = SPTAuth.DefaultInstance;
            SPTAuthCallback authCallback = (error, session) =>
            {
                auth.Session = session;
            };

            if (!auth.CanHandleURL(url)) return false;

            auth.HandleAuthCallbackWithTriggeredAuthURL(url, authCallback);
            
            return true;
        }

            _authViewController.PresentingViewController.DismissViewController(true, null);
            _authViewController = null;

            _auth.HandleAuthCallbackWithTriggeredAuthURL(e.Url, (error, session) =>
            {
                
            });
        }

        public void AuthWithURL(NSUrl url)
        {
            
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

        public void HandleNewSession()
        {
            SPTAuth auth = SPTAuth.DefaultInstance;

            if (_spotifyPlayer == null)
            {
                _spotifyPlayer = SPTAudioStreamingController.SharedInstance();
                NSError error = null;
                if (_spotifyPlayer.StartWithClientId(auth.ClientID, null, true, out error))
                {
                    //_spotifyPlayer.Delegate = this;
                    //_spotifyPlayer.PlaybackDelegate = this;
                    //_spotifyPlayer.DiskCache = NSObject.Alloc(1024 * 1024 * 64);
                    _spotifyPlayer.LoginWithAccessToken(auth.Session.AccessToken);

                }
                else
                {
                    _spotifyPlayer = null;

                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
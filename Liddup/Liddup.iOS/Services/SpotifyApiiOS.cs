﻿using System;
using System.Collections.Generic;
using Foundation;
using Liddup.Constants;
using Liddup.iOS.Services;
using Liddup.Services;
using SpotifyBindingiOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SpotifyApiiOS))]
namespace Liddup.iOS.Services
{
    internal class SpotifyApiiOS : SPTAudioStreamingDelegate, ISpotifyApi
    {
        private SPTAuth _auth = SPTAuth.DefaultInstance;
        private const string ClientId = ApiConstants.SpotifyClientId;
        private readonly NSUrl _redirectUrl = new NSUrl(ApiConstants.SpotifyRedirectUri);
        private NSUrl _tokenSwapUrl;
        private NSUrl _tokenRefreshUrl;
        private string _sessionUserDefaultsKey;

        private UIViewController _authViewController;

        public string AccessToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsLoggedIn => throw new NotImplementedException();

        private SPTAudioStreamingController _spotifyPlayer;

        private static NSString[] ConvertStringsToNsStrings(IReadOnlyList<string> strings)
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
            SPTAuth.DefaultInstance.ClientID = ClientId;
            SPTAuth.DefaultInstance.RequestedScopes = scopes;
            SPTAuth.DefaultInstance.RedirectURL = _redirectUrl;
            SPTAuth.DefaultInstance.TokenSwapURL = _tokenSwapUrl;
            SPTAuth.DefaultInstance.TokenRefreshURL = _tokenRefreshUrl;
            SPTAuth.DefaultInstance.SessionUserDefaultsKey = _sessionUserDefaultsKey;

            _spotifyPlayer.Delegate = this;

            NSError error = null;

            try
            {
                SPTAudioStreamingController.SharedInstance().StartWithClientId(ClientId, out error);
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
            if (SPTAuth.DefaultInstance.Session.IsValid)
                StartLoginFlow();
            else
            {
                NSUrl authUrl = SPTAuth.DefaultInstance.LoginURL;

                
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

        public void RenewTokenAndShowPlayer()
        {
            _auth = SPTAuth.DefaultInstance;

            _auth.RenewSession(_auth.Session, (error, session) =>
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
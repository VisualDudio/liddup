using System;
using Com.Spotify.Sdk.Android.Player;
using Java.Lang;

namespace Liddup.Droid
{
    internal class InitializationObserverDelegate : Java.Lang.Object, SpotifyPlayer.IInitializationObserver
    {
        private readonly Action<SpotifyPlayer> _onInitialized;
        private readonly Action<Throwable> _onError;

        public InitializationObserverDelegate(Action<SpotifyPlayer> onInitialized, Action<Throwable> onError)
        {
            _onInitialized = onInitialized;
            _onError = onError;
        }

        public void OnError(Throwable error)
        {
            _onError(error);
        }

        public void OnInitialized(SpotifyPlayer player)
        {
            _onInitialized(player);
        }
    }
}
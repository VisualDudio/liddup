using System;
using Com.Spotify.Sdk.Android.Player;

namespace Liddup.Droid
{
    internal class OperationCallbackDelegate : Java.Lang.Object, IPlayerOperationCallback
    {
        private readonly Action _onSuccess;
        private readonly Action<Error> _onError;

        public OperationCallbackDelegate(Action onSuccess, Action<Error> onError)
        {
            _onSuccess = onSuccess;
            _onError = onError;
        }

        public void OnError(Error error)
        {
            _onError(error);
        }

        public void OnSuccess()
        {
            _onSuccess();
        }
    }
}
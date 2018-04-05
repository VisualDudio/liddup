using Liddup.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace Liddup.PageModels
{
    class SpotifyUnavailablePageModel : BasePageModel
    {
        ICommand _connectToSpotifyCommand;
        public ICommand ConnectToSpotifyCommand => _connectToSpotifyCommand ?? (_connectToSpotifyCommand = new Command(() => ConnectToSpotify()));

        private void ConnectToSpotify()
        {
            if (!DependencyService.Get<ISpotifyApi>().IsLoggedIn)
                DependencyService.Get<ISpotifyApi>().Login();

            MessagingCenter.Send(this as BasePageModel, "NavigateToSpotifyLibrary");
        }
    }
}

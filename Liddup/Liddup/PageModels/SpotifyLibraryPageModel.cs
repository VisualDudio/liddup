using PropertyChanged;
using System.Windows.Input;
using Xamarin.Forms;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class SpotifyLibraryPageModel : BasePageModel
    {
        ICommand _navigateToSongsPageCommand;
        public ICommand NavigateToSongsPageCommand => _navigateToSongsPageCommand ?? (_navigateToSongsPageCommand = new Command(() => NavigateToSongsPage()));

        ICommand _navigateToPlaylistsPageCommand;
        public ICommand NavigateToPlaylistsPageCommand => _navigateToPlaylistsPageCommand ?? (_navigateToPlaylistsPageCommand = new Command(() => NavigateToPlaylistsPage()));

        private void NavigateToPlaylistsPage()
        {
            MessagingCenter.Send(this as BasePageModel, "NavigateToPlaylists");
        }

        private void NavigateToSongsPage()
        {
            MessagingCenter.Send(this as BasePageModel, "NavigateToSongs");
        }
    }
}

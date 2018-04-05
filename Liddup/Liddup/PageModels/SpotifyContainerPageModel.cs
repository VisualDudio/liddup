using Xamarin.Forms;
using Liddup.Pages;
using System.Windows.Input;
using PropertyChanged;
using Liddup.Models;
using Liddup.TemplateSelectors;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class SpotifyContainerPageModel : AddSongsPageModel
    {
        [AlsoNotifyFor(nameof(CurrentTemplate))]
        public DataTemplate CurrentTemplate { get; set; }

        public SpotifyContainerPageModel()
        {
            var spotifyTemplateSelector = new SpotifyAuthenticationDataTemplateSelector()
            {
                AuthenticatedTemplate = new DataTemplate(() => new SpotifyLibraryPage()),
                NotAuthenticatedTemplate = new DataTemplate(() => new SpotifyUnavailablePage())
            };

            CurrentTemplate = spotifyTemplateSelector.SelectTemplate(User, null);
            MessagingCenter.Subscribe<BasePageModel, Playlist>(this, "NavigateToPlaylistSongs", NavigateToPlaylistPage);
            MessagingCenter.Subscribe<BasePageModel>(this, "NavigateToPlaylists", NavigateToPlaylistsPage);
            MessagingCenter.Subscribe<BasePageModel>(this, "NavigateToSongs", NavigateToSongsPage);
            MessagingCenter.Subscribe<BasePageModel>(this, "NavigateToSpotifyLibrary", NavigateToSpotifyLibraryPage);
        }

        private void NavigateToPlaylistPage(BasePageModel sender, Playlist playlist)
        {
            CurrentTemplate = new DataTemplate(() => new PlaylistPage(playlist));
        }

        private void NavigateToPlaylistsPage(BasePageModel sender)
        {
            CurrentTemplate = new DataTemplate(() => new PlaylistsPage());
        }

        private void NavigateToSpotifyLibraryPage(BasePageModel sender)
        {
            if (!User.IsSpotifyAuthenticated)
                User.IsSpotifyAuthenticated = true;
            CurrentTemplate = new DataTemplate(() => new SpotifyLibraryPage());
        }

        private void NavigateToSongsPage(BasePageModel sender)
        {
            CurrentTemplate = new DataTemplate(() => new SongsPage());
        }
    }
}
using System;
using Liddup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Liddup.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MusicServicesPage : ContentPage
    {
        public MusicServicesPage()
        {
            InitializeComponent();
        }

        private async void ConnectSpotifyButton_OnClicked(object sender, EventArgs e)
        {
            if (!DependencyService.Get<ISpotifyApi>().IsLoggedIn)
                DependencyService.Get<ISpotifyApi>().Login();

            await Navigation.PushAsync(new UserLibraryPage());
        }

        private async void LibraryButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LibrarySongsPage());
        }

        private void ConnectDeezerButton_OnClicked(object sender, EventArgs e)
        {

        }
    }
}
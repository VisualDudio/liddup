using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liddup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using PCLStorage;

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
            try
            {
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async void ConnectGooglePlayMusicButton_OnClicked(object sender, EventArgs e)
        {

        }
    }
}
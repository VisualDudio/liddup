using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Liddup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MusicServicesPage : ContentPage
    {
        public MusicServicesPage()
        {
            InitializeComponent();
        }

        private void ConnectSpotifyButton_OnClicked(object sender, EventArgs e)
        {
            if (!DependencyService.Get<ISpotifyApi>().IsLoggedIn)
                DependencyService.Get<ISpotifyApi>().Login();

            Navigation.PushAsync(new UserLibraryPage());
        }
    }
}
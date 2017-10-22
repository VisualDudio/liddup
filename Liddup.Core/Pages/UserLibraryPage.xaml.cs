using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Liddup.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserLibraryPage : ContentPage
    {
        public UserLibraryPage()
        {
            InitializeComponent();
        }

        private async void PlaylistsButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserPlaylistsPage());
        }

        private async void SongsButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserSongsPage());
        }
    }
}
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
    public partial class UserLibraryPage : ContentPage
    {
        public UserLibraryPage()
        {
            InitializeComponent();
        }

        private void PlaylistsButton_OnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserPlaylistsPage());
        }

        private void SongsButton_OnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UserSongsPage());
        }
    }
}
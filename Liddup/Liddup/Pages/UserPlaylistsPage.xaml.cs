using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Liddup.Models;
using Liddup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;

namespace Liddup.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPlaylistsPage : ContentPage
    {
        private CancellationTokenSource _tokenSource;

        public UserPlaylistsPage()
        {
            InitializeComponent();

            UpdateUI();
        }

        private async void UpdateUI()
        {
            using (_tokenSource = new CancellationTokenSource())
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(TimeSpan.FromSeconds(1), _tokenSource.Token);

                    UserPlaylists.ItemsSource = await SpotifyApiManager.GetUserPlaylistsAsync(_tokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void UserPlaylists_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var selectedPlaylist = (SimplePlaylist) e.SelectedItem;
                selectedPlaylist.Owner.Id = Uri.EscapeDataString(selectedPlaylist.Owner.Id);
                Navigation.PushAsync(new UserPlaylistSongsPage(selectedPlaylist.Owner.Id, selectedPlaylist.Id));
            }

            ((ListView)sender).SelectedItem = null;
        }
    }
}
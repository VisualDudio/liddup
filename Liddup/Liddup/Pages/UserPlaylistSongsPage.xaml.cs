using System;
using System.Threading;
using System.Threading.Tasks;
using Liddup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Liddup.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPlaylistSongsPage : ISongProvider
    {
        private static CancellationTokenSource _tokenSource;

        public UserPlaylistSongsPage(string profileId, string playlistId)
        {
            InitializeComponent();

            UpdateUI(profileId, playlistId);
        }

        

        private async void UpdateUI(string profileId, string playlistId)
        {
            using (_tokenSource = new CancellationTokenSource())
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(TimeSpan.FromSeconds(1), _tokenSource.Token);

                    UserPlaylistSongs.ItemsSource = await SpotifyApiManager.GetUserPlaylistSongsAsync(profileId, playlistId, _tokenSource.Token);
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

        private void UserPlaylistSongs_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SpotifyApiManager.AddSongToMasterPlaylist(e.SelectedItem, this);
        }
    }
}
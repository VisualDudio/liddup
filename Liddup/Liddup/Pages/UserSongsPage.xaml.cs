using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Liddup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;

namespace Liddup.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserSongsPage : ISongProvider
    {
        private static CancellationTokenSource _tokenSource;

        public UserSongsPage()
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

                    UserSongs.ItemsSource = await SpotifyApiManager.GetSavedTracks(_tokenSource.Token);
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

        private void UserSongs_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SpotifyApiManager.AddSongToMasterPlaylist(e.SelectedItem, this);
        }
    }
}
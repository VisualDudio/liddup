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
        private SpotifyWebAPI m_spotify;
        private CancellationTokenSource m_tokenSource;

        public UserPlaylistsPage()
        {
            InitializeComponent();

            var accessToken = DependencyService.Get<ISpotifyApi>().AccessToken;

            InitApi(accessToken);

            var profile = m_spotify.GetPrivateProfile();
            profile.Id = Uri.EscapeDataString(profile.Id);

            UpdateUI(profile);
        }

        private async void UpdateUI(PrivateProfile profile)
        {
            using (m_tokenSource = new CancellationTokenSource())
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(TimeSpan.FromSeconds(1), m_tokenSource.Token); // buffer

                    UserPlaylists.ItemsSource = await GetUserPlaylistsAsync(profile, m_tokenSource.Token);
                }
                catch (TaskCanceledException) // if the operation is cancelled, do nothing
                {
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public async Task<List<SimplePlaylist>> GetUserPlaylistsAsync(PrivateProfile profile, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var playlists = await m_spotify.GetUserPlaylistsAsync(profile.Id);
            token.ThrowIfCancellationRequested();
            var list = playlists.Items.ToList();
            
            while (playlists.Next != null)
            {
                token.ThrowIfCancellationRequested();
                playlists = await m_spotify.GetUserPlaylistsAsync(profile.Id, 20, playlists.Offset + playlists.Limit);
                token.ThrowIfCancellationRequested();
                list.AddRange(playlists.Items);
            }

            return list;
        }

        public void InitApi(string accessToken)
        {
            m_spotify = new SpotifyWebAPI
            {
                UseAuth = true,
                TokenType = "Bearer",
                AccessToken = accessToken
            };
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
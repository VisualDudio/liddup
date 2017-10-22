using System;
using System.Collections.Generic;
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
    public partial class UserPlaylistSongsPage : ContentPage
    {
        private SpotifyWebAPI m_spotify;
        private static CancellationTokenSource m_tokenSource;

        public UserPlaylistSongsPage(string profileId, string playlistId)
        {
            InitializeComponent();

            var accessToken = DependencyService.Get<ISpotifyApi>().AccessToken;

            InitApi(accessToken);

            UpdateUI(profileId, playlistId);
        }

        private async void UpdateUI(string profileId, string playlistId)
        {
            using (m_tokenSource = new CancellationTokenSource())
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(TimeSpan.FromSeconds(1), m_tokenSource.Token); // buffer

                    UserPlaylistSongs.ItemsSource =
                        await GetUserPlaylistSongsAsync(profileId, playlistId, m_tokenSource.Token);
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

        private async Task<List<FullTrack>> GetUserPlaylistSongsAsync(string profileId, string playlistId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var savedTracks = await m_spotify.GetPlaylistTracksAsync(profileId, playlistId);
            token.ThrowIfCancellationRequested();
            var list = savedTracks.Items.Select(track => track.Track).ToList();

            while (savedTracks.Next != null)
            {
                token.ThrowIfCancellationRequested();
                savedTracks = await m_spotify.GetPlaylistTracksAsync(profileId, playlistId, "", 20, savedTracks.Offset + savedTracks.Limit);
                token.ThrowIfCancellationRequested();
                list.AddRange(savedTracks.Items.Select(track => track.Track));
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

        private void UserPlaylistSongs_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            AddSong((FullTrack) e.SelectedItem);
        }

        private void AddSong(FullTrack track)
        {
            var song = new Song
            {
                Uri = track.Uri,
                SongSource = "Spotify",
                Title = track.Name,
                Votes = 0
            };

            try
            {
                MessagingCenter.Send(this, "AddSong", song);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}
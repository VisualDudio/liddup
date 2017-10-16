using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;

namespace Liddup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserSongsPage : ContentPage
    {
        private SpotifyWebAPI m_spotify;
        private static CancellationTokenSource m_tokenSource;

        public UserSongsPage()
        {
            InitializeComponent();

            var accessToken = DependencyService.Get<ISpotifyApi>().AccessToken;

            InitApi(accessToken);
            UpdateUI();
        }

        private async void UpdateUI()
        {
            using (m_tokenSource = new CancellationTokenSource())
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(TimeSpan.FromSeconds(1), m_tokenSource.Token); // buffer

                    UserSongs.ItemsSource = await GetSavedTracks(m_tokenSource.Token);
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

        private async Task<List<FullTrack>> GetSavedTracks(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var savedTracks = await m_spotify.GetSavedTracksAsync();
            token.ThrowIfCancellationRequested();
            var list = savedTracks.Items.Select(track => track.Track).ToList();

            while (savedTracks.Next != null)
            {
                token.ThrowIfCancellationRequested();
                savedTracks = await m_spotify.GetSavedTracksAsync(20, savedTracks.Offset + savedTracks.Limit);
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
    }
}
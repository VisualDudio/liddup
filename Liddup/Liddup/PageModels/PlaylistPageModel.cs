using Liddup.Models;
using System.Collections.ObjectModel;
using PropertyChanged;
using System.Windows.Input;
using Xamarin.Forms;
using System;
using Liddup.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class PlaylistPageModel : BasePageModel, ISongProvider
    {
        private static CancellationTokenSource _tokenSource;

        public List<Song> Songs { get; set; }
        public Playlist SelectedPlaylist { get; set; }
        public bool IsBusy { get; set; }

        ICommand _navigateToPlaylistsPageCommand;
        public ICommand NavigateToPlaylistsPageCommand => _navigateToPlaylistsPageCommand ?? (_navigateToPlaylistsPageCommand = new Command(() => NavigateToPlaylistsPage()));

        ICommand _addSongToQueueCommand;
        public ICommand AddSongToQueueCommand => _addSongToQueueCommand ?? (_addSongToQueueCommand = new Command<Song>(s => AddSongToQueue(s)));

        public PlaylistPageModel(Playlist playlist)
        {
            SelectedPlaylist = playlist;
            UpdateUI(SelectedPlaylist.OwnerId, SelectedPlaylist.Id);
        }

        private async void UpdateUI(string profileId, string playlistId)
        {
            using (_tokenSource = new CancellationTokenSource())
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(TimeSpan.FromSeconds(1), _tokenSource.Token);
                    var _songs = await SpotifyApiManager.GetUserPlaylistSongsAsync(profileId, playlistId, _tokenSource.Token);
                    Songs = _songs.Select(x => new Song { Title = x.Name, Uri = x.Uri, Artist = x.Artists[0].Name, ImageUrl = x.Album.Images[0].Url, DurationInSeconds = x.DurationMs / 1000, Source = "Spotify"}).ToList();
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

        private void AddSongToQueue(Song s)
        {
            SpotifyApiManager.AddSongToQueue(s, this);
        }

        private void NavigateToPlaylistsPage()
        {
            MessagingCenter.Send(this as BasePageModel, "NavigateToPlaylists");
        }
    }
}

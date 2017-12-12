using Xamarin.Forms;
using Liddup.Models;
using System.Windows.Input;
using PropertyChanged;
using System;
using System.Threading;
using System.Linq;
using Liddup.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class PlaylistsPageModel : BasePageModel, ISongProvider
    {
        private CancellationTokenSource _tokenSource;

        public List<Playlist> Playlists { get; set; }
        public bool IsBusy { get; set; }

        ICommand _navigateToSpotifyLibraryCommand;
        public ICommand NavigateToSpotifyLibraryCommand => _navigateToSpotifyLibraryCommand ?? (_navigateToSpotifyLibraryCommand = new Command(() => NavigateToSpotifyLibrary()));

     
        ICommand _navigateToPlaylistSongsCommand;
        public ICommand NavigateToPlaylistSongsCommand => _navigateToPlaylistSongsCommand ?? (_navigateToPlaylistSongsCommand = new Command<Playlist>(p => NavigateToPlaylistSongs(p)));

        public PlaylistsPageModel()
        {
            UpdateUI();
        }

        public void NavigateToPlaylistSongs(Playlist playlist)
        {
            MessagingCenter.Send(this as BasePageModel, "NavigateToPlaylistSongs", playlist);
        }

        private void NavigateToSpotifyLibrary()
        {
            MessagingCenter.Send(this as BasePageModel, "NavigateToSpotifyLibrary");
        }

        private async void UpdateUI()
        {
            using (_tokenSource = new CancellationTokenSource())
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(TimeSpan.FromSeconds(1), _tokenSource.Token);

                    var _playlists = await SpotifyApiManager.GetUserPlaylistsAsync(_tokenSource.Token);
                    Playlists = _playlists.Select(x => new Playlist { Name = x.Name, ImageUrl = x.Images[0].Url, NumberOfTracks = x.Tracks.Total, OwnerId = Uri.EscapeDataString(x.Owner.Id), Id = Uri.EscapeDataString(x.Id) }).ToList();
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

        //private void UserPlaylists_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    if (e.SelectedItem != null)
        //    {
        //        var selectedPlaylist = (SimplePlaylist)e.SelectedItem;
        //        selectedPlaylist.Owner.Id = Uri.EscapeDataString(selectedPlaylist.Owner.Id);
        //    }

        //    ((ListView)sender).SelectedItem = null;
        //}
    }
}

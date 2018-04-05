using Liddup.Models;
using Liddup.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using PropertyChanged;
using System.Collections.Generic;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class SongsPageModel : BasePageModel, ISongProvider
    {
        private static CancellationTokenSource _tokenSource;

        public List<Song> Songs { get; private set; }
        public bool IsBusy { get; set; }

        ICommand _addSongToQueueCommand;
        public ICommand AddSongToQueueCommand => _addSongToQueueCommand ?? (_addSongToQueueCommand = new Command<Song>(s => AddSongToQueue(s)));

        public SongsPageModel()
        {

        }

        private async void UpdateUI()
        {
            using (_tokenSource = new CancellationTokenSource())
            {
                try
                {
                    IsBusy = true;
                    await Task.Delay(TimeSpan.FromSeconds(1), _tokenSource.Token);

                    var _songs = await SpotifyApiManager.GetSavedTracks(_tokenSource.Token);
                    Songs = _songs.Select(x => new Song { Title = x.Name, Uri = x.Uri, Artist = x.Artists[0].Name, ImageUrl = x.Album.Images[0].Url, DurationInSeconds = x.DurationMs / 1000, Source = "Spotify" }).ToList();
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

        private void AddSongToQueue(Song song)
        {
            song.AddToQueueCommand.Execute(null);

            SpotifyApiManager.AddSongToQueue(song, this);
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Liddup.Models;
using Liddup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Liddup.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPlaylistPage : ContentPage
    {
        private static ObservableCollection<Song> _songs = new ObservableCollection<Song>();

        public MasterPlaylistPage(string replicationIP, bool isHost = false)
        {
            InitializeComponent();

            SongManager.Host = replicationIP;
            RoomCodeLabel.Text = DependencyService.Get<INetworkManager>().GetEncryptedIPAddress(replicationIP);
            SongManager.StartListener();

            StartReplications();

            if (!isHost)
                _songs = SongManager.GetSongs();

            MasterPlaylist.ItemsSource = _songs;

            MessagingCenter.Subscribe<ISongProvider, Song>(this, "AddSong", SubscribeToSongAdditions);
        }

        private static void SubscribeToSongAdditions(object sender, Song song)
        {
            if (_songs.FirstOrDefault(s => s.Uri.Equals(song.Uri)) == null)
            {
                SongManager.SaveSong(song);
                _songs.Add(song);
            }
            else
            {
                //TODO: Notify user that the song already exists via MessagingCenter
            }
        }

        private async void AddSongsButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MusicServicesPage());
        }
        private void StartReplications()
        {
            SongManager.StartReplications(async (sender, e) =>
            {
                var changes = e.Changes;

                foreach (var change in changes)
                {
                    var song = SongManager.GetSong(change.DocumentId);

                    var indexOfExistingSong = _songs.IndexOf(_songs.FirstOrDefault(s => s.Id == song.Id));
                    if (indexOfExistingSong < 0)
                    {
                        if (song.SongSource.Equals("Library"))
                        {
                            song.Contents = SongManager.GetSongContents(song);
                            song.Uri = await FileSystemManager.WriteFileAsync(song.Contents, song.Id);
                        }
                        _songs.Add(song);
                    }
                    else
                    {
                        _songs[indexOfExistingSong].Votes = song.Votes;
                        _songs = new ObservableCollection<Song>(_songs.OrderByDescending(s => s.Votes));
                        MasterPlaylist.ItemsSource = _songs;
                    }
                }
            });
        }

        private void VoteButton_OnClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var song = button?.CommandParameter as Song;
            song.Votes++;
            SongManager.SaveSong(song);
        }

        private void DeleteButton_OnClicked(object sender, EventArgs e)
        {
            // This button is just for debugging, the database should get automatically deleted once the app closes or the user joins/hosts another playlist
            SongManager.DeleteDatabases();
        }

        private void MasterPlaylist_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            PlaySong(e.SelectedItem as Song);
        }

        private static async void PlaySong(Song song)
        {
            switch (song.SongSource)
            {
                case "Spotify":
                    DependencyService.Get<ISpotifyApi>().PlayTrack(song.Uri);
                    break;
                case "Library":
                    var mediaFile = new MediaFile
                    {
                        Url = "file://" + song.Uri,
                        Metadata = new MediaFileMetadata {AlbumArt = song.AlbumArt},
                        Type = MediaFileType.Audio,
                        Availability = ResourceAvailability.Local
                    };
                    await CrossMediaManager.Current.Play(mediaFile);
                    
                    break;
                default:
                    break;
            }
        }
    }
}
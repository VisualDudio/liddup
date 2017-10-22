using System;
using System.Collections.ObjectModel;
using System.Linq;
using Liddup.Models;
using Liddup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Liddup.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPlaylistPage : ContentPage
    {
        public MasterPlaylistPage(string replicationIP)
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine(replicationIP);
            SongManager.Host = replicationIP;
            RoomCodeLabel.Text = DependencyService.Get<INetworkManager>().GetEncryptedIPAddress(replicationIP);
            SongManager.StartListener();

            StartReplications();

            var songs = SongManager.GetSongs();
            MasterPlaylist.ItemsSource = songs;

            SongManager.UpdateUI((sender, e) =>
            {
                var changes = e.Changes;

                foreach (var change in changes)
                {
                    var song = SongManager.GetSong(change.DocumentId);
                    var indexOfExistingSong = songs.IndexOf(songs.FirstOrDefault(s => s.Id == song.Id));
                    if (indexOfExistingSong < 0)
                        songs.Add(song);
                    else
                    {
                        songs[indexOfExistingSong].Votes = song.Votes;
                        songs = new ObservableCollection<Song>(songs.OrderByDescending(s => s.Votes));
                        MasterPlaylist.ItemsSource = songs;
                    }
                }
            });

            MessagingCenter.Subscribe<UserPlaylistSongsPage, Song>(this, "AddSong", (sender, song) =>
            {
                if (songs.FirstOrDefault(s => s.Uri.Equals(song.Uri)) == null)
                {
                    SongManager.SaveSong(song);
                    songs.Add(song);
                }
                else
                {
                    //TODO: Notify user that the song already exists via MessagingCenter
                }
            });
        }

        private async void AddSongsButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MusicServicesPage());
        }
        private static void StartReplications()
        {
            SongManager.StartReplications((sender, e) => { });
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
            SongManager.DeleteDatabase();
        }
    }
}
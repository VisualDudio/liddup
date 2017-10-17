using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liddup.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Liddup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPlaylistPage : ContentPage
    {
        private ObservableCollection<Song> _songs = new ObservableCollection<Song>();

        public MasterPlaylistPage()
        {
            InitializeComponent();
            SongManager.InitManager();
            SongManager.StartListener();
            SongManager.StartReplications(() =>
            {
                _songs = SongManager.GetSongs();
                MasterPlaylist.ItemsSource = _songs;
            });

            MessagingCenter.Subscribe<UserPlaylistSongsPage, Song>(this, "AddSong", (sender, song) =>
            {
                _songs.Add(song);
                SongManager.SaveSong(song);
                MasterPlaylist.ItemsSource = _songs;
            });
        }

        private async void AddSongsButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MusicServicesPage());
        }
    }
}
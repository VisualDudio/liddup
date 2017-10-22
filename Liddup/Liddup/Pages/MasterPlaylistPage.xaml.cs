using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liddup.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using Zeroconf;

namespace Liddup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPlaylistPage : ContentPage
    {
        private ObservableCollection<Song> _songs = new ObservableCollection<Song>();
        
        public MasterPlaylistPage(string replicationIP)
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine(replicationIP);
            SongManager.Host = replicationIP;
            RoomCodeLabel.Text = DependencyService.Get<INetworkManager>().GetEncryptedIPAddress(replicationIP);
            SongManager.StartListener();

            StartReplications();

            MasterPlaylist.ItemsSource = _songs;

            SongManager.UpdateUI((sender, e) =>
            {
                var changes = e.Changes;

                foreach (var change in changes)
                    _songs.Add(SongManager.GetSong(change.DocumentId));

                MasterPlaylist.ItemsSource = _songs;
            });

            MessagingCenter.Subscribe<UserPlaylistSongsPage, Song>(this, "AddSong", (sender, song) =>
            {
                SongManager.SaveSong(song);
                _songs = SongManager.GetSongs();
            });
        }

        private async void AddSongsButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MusicServicesPage());
        }

        private void StartSyncButton_OnClicked(object sender, EventArgs e)
        {
            StartReplications();
            DependencyService.Get<INetworkManager>().SetHotSpot(true);
        }

        private void StartReplications()
        {
            SongManager.StartReplications((sender, e) =>
            {
                _songs = SongManager.GetSongs();
                MasterPlaylist.ItemsSource = _songs;
            });
        }

        //private static async void Browse()
        //{
        //    //await Task.Run(async () =>
        //    //{

        //    //});

        //    var responses = await ZeroconfResolver.BrowseDomainsAsync();
        //    var builder = new StringBuilder();
        //    foreach (var service in responses)
        //    {
        //        builder.Append(service.Key + Environment.NewLine);

        //        foreach (var host in service)
        //            builder.Append("\tIP: " + host + Environment.NewLine);
        //    }

        //    System.Diagnostics.Debug.WriteLine(builder.ToString());
        //}

        //static async void Resolve(Label output)
        //{
        //    //await Task.Run(async () =>
        //    //{

        //    //});


        //    var domains = await ZeroconfResolver.BrowseDomainsAsync();

        //    var responses = await ZeroconfResolver.ResolveAsync(domains.Select(g => g.Key));


        //    foreach (var resp in responses)
        //    {
        //        output.Text += resp + Environment.NewLine;
        //    }
        //}
        private void VoteButton_OnClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var song = button?.CommandParameter as Song;
            song.Votes++;
            SongManager.SaveSong(song);
        }

        private void DeleteButton_OnClicked(object sender, EventArgs e)
        {
            SongManager.DeleteDatabase();
        }
    }
}
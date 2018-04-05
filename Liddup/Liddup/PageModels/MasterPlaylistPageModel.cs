using Liddup.Models;
using Liddup.Services;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.Implementations;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class MasterPlaylistPageModel : AppContainerPageModel
    {
        public ObservableCollection<Song> Songs { get; private set; } = new ObservableCollection<Song>();
        public Song CurrentSong { get; private set; }
        public string CurrentPlayIcon { get; private set; } = "resource://Liddup.Resources.play_button.svg";
        public int Ticks { get; set; }

        ICommand _navigateToHomePageCommand;
        public ICommand NavigateToHomePageCommand => _navigateToHomePageCommand ?? (_navigateToHomePageCommand = new Command(() => NavigateToHomePage()));

        ICommand _navigateToAddSongsPageCommand;
        public ICommand NavigateToAddSongsPageCommand => _navigateToAddSongsPageCommand ?? (_navigateToAddSongsPageCommand = new Command(() => NavigateToAddSongsPage()));

        ICommand _toggleRepeatSongVoteCommand;
        public ICommand ToggleRepeatSongVoteCommand => _toggleRepeatSongVoteCommand ?? (_toggleRepeatSongVoteCommand = new Command(() => ToggleRepeatSongVote()));

        ICommand _playPauseSongCommand;
        public ICommand PlayPauseSongCommand => _playPauseSongCommand ?? (_playPauseSongCommand = new Command(async () => await PlayPauseSong()));

        ICommand _toggleSkipSongVoteCommand;
        public ICommand ToggleSkipSongVoteCommand => _toggleSkipSongVoteCommand ?? (_toggleSkipSongVoteCommand = new Command(() => ToggleSkipSongVote()));

        public MasterPlaylistPageModel()
        {
            DatabaseManager.Host = DependencyService.Get<INetworkManager>().GetDecryptedIPAddress(Room.Code);
            DatabaseManager.StartListener();
            StartReplications();

            if (!User.IsHost)
                Songs = new ObservableCollection<Song>(DatabaseManager.GetSongs().OrderByDescending(s => s.Votes));

            MessagingCenter.Subscribe<Song, Song>(this, "AddSong", AddSong);
            MessagingCenter.Subscribe<Song, Song>(this, "UpdateSong", UpdateSong);
        }

        private void ToggleSkipSongVote()
        {
            if (CurrentSong == null) return;

            CurrentSong.IsSkipVoted = !CurrentSong.IsSkipVoted;

            if (CurrentSong.IsRepeatVoted)
            {
                CurrentSong.IsRepeatVoted = false;
                CurrentSong.RepeatVotes--;
            }

            if (CurrentSong.IsSkipVoted)
                CurrentSong.SkipVotes++;
            else
                CurrentSong.SkipVotes--;
        }

        private async Task PlayPauseSong()
        {
            if (CurrentSong == null) return;

            CurrentSong.IsPlaying = !CurrentSong.IsPlaying;
            if (CurrentSong.IsPlaying)
            {
                CurrentPlayIcon = "resource://Liddup.Resources.pause_button.svg";
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (CurrentSong == null) return false;
                    Ticks++;

                    if (Ticks == CurrentSong.DurationInSeconds)
                        PlayNextSong();

                    return Ticks <= CurrentSong.DurationInSeconds && CurrentSong.IsPlaying;
                });
                await PlaySong();
            }
            else
            {
                CurrentPlayIcon = "resource://Liddup.Resources.play_button.svg";
                await PauseSong();
            }
        }

        private async void PlayNextSong()
        {
            CurrentSong.IsPlaying = false;
            await PauseSong();
            DatabaseManager.DeleteSong(CurrentSong);
            if (Songs.FirstOrDefault() == null)
                return;
            CurrentSong = Songs.FirstOrDefault();
            Ticks = 0;
            CurrentSong.IsPlaying = true;
            Songs.RemoveAt(Songs.Count - 1);
            await PlaySong();
        }

        private void ToggleRepeatSongVote()
        {
            if (CurrentSong == null) return;

            CurrentSong.IsRepeatVoted = !CurrentSong.IsRepeatVoted;

            if (CurrentSong.IsSkipVoted)
            {
                CurrentSong.IsSkipVoted = false;
                CurrentSong.SkipVotes--;
            }

            if (CurrentSong.IsRepeatVoted)
                CurrentSong.RepeatVotes++;
            else
                CurrentSong.RepeatVotes--;
        }

        private void NavigateToHomePage()
        {
            MessagingCenter.Send(this as FreshMvvm.FreshBasePageModel, "PositionChanged", 0);
        }

        private void NavigateToAddSongsPage()
        {
            MessagingCenter.Send(this as FreshMvvm.FreshBasePageModel, "PositionChanged", 2);
        }

        private async Task PlaySong()
        {
            switch (CurrentSong.Source)
            {
                case "Spotify":
                    SpotifyApiManager.PlayTrack(CurrentSong.Uri);
                    break;
                case "Library":
                    var mediaFile = new MediaFile
                    {
                        Url = "file://" + CurrentSong.Uri,
                        Metadata = new MediaFileMetadata { AlbumArt = CurrentSong.ImageUrl },
                        Type = MediaFileType.Audio,
                        Availability = ResourceAvailability.Local
                    };
                    await CrossMediaManager.Current.Play(mediaFile);
                    break;
                default:
                    break;
            }
        }

        private async Task PauseSong()
        {
            if (CurrentSong.Source.Equals("Spotify"))
                SpotifyApiManager.PauseTrack();
            else if (CurrentSong.Source.Equals("Library"))
                await CrossMediaManager.Current.Pause();
        }

        private async Task ResumeSong()
        {
            if (CurrentSong.Source.Equals("Spotify"))
                SpotifyApiManager.ResumeTrack();
            else if (CurrentSong.Source.Equals("Library"))
                await CrossMediaManager.Current.Play();
        }

        private void UpdateSong(object sender, Song song)
        {
            DatabaseManager.SaveSong(song);
        }

        private void StartReplications()
        {
            DatabaseManager.StartReplications(async (sender, e) =>
            {
                var changes = e.Changes.ToList();

                foreach (var change in changes)
                {
                    if (change.IsDeletion) continue;
                    if (Songs.Count == 0 && CurrentSong != null) continue;
                    var song = DatabaseManager.GetSong(change.DocumentId);
                    var indexOfExistingSong = Songs.IndexOf(Songs.FirstOrDefault(s => s.Id == song.Id));

                    if (indexOfExistingSong < 0)
                    {
                        if (song.Source.Equals("Library"))
                        {
                            song.Contents = DatabaseManager.GetSongContents(song);
                            song.Uri = await FileSystemManager.WriteFileAsync(song.Contents, song.Id);
                        }
                        if (song.Id != CurrentSong.Id)
                            Songs.Add(song);
                    }
                    else
                    {
                        Songs[indexOfExistingSong].Votes = song.Votes;
                        Songs = new ObservableCollection<Song>(Songs.OrderByDescending(s => s.Votes));
                    }
                }
            });
        }

        private void AddSong(object sender, Song song)
        {
            if (Songs?.FirstOrDefault(s => s.Uri.Equals(song.Uri)) != null) return;

            if (CurrentSong == null)
                CurrentSong = song;
            else
                Songs.Add(song);

            DatabaseManager.SaveSong(song);
        }
    }
}
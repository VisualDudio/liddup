using PropertyChanged;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Liddup.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Song
    {
        public string Title { get; set; } = "Unavailable";
        public string Artist { get; set; } = "Unavailable";
        public string HeartIcon { get; set; } = "resource://Liddup.Resources.heart_hollow.svg";
        public int Votes { get; set; }
        public int SkipVotes { get; set; }
        public int RepeatVotes { get; set; }
        public int DurationInSeconds { get; set; } = 1;
        public string ImageUrl { get; set; }
        public bool IsInQueue { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsVoted { get; set; }
        public bool IsSkipVoted { get; set; }
        public bool IsRepeatVoted { get; set; }
        public string Id { get; set; }
        public string Uri { get; set; }
        public string Source { get; set; }
        public byte[] Contents { get; set; }

        ICommand _toggleVoteCommand;
        public ICommand ToggleVoteCommand => _toggleVoteCommand ?? (_toggleVoteCommand = new Command(() => ToggleVote()));

        ICommand _addToQueueCommand;
        public ICommand AddToQueueCommand => _addToQueueCommand ?? (_addToQueueCommand = new Command(() => AddToQueue()));

        public void ToggleVote()
        {
            IsVoted = !IsVoted;
            if (IsVoted)
            {
                HeartIcon = "resource://Liddup.Resources.heart_filled.svg";
                Votes++;
            }
            else
            {
                HeartIcon = "resource://Liddup.Resources.heart_hollow.svg";
                Votes--;
            }
            MessagingCenter.Send(this, "UpdateSong", this);
        }

        private void AddToQueue()
        {
            IsInQueue = true;
            ToggleVote();
            MessagingCenter.Send(this, "AddSong", this);
        }

        public Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"title", Title },
                {"uri", Uri },
                {"source", Source },
                {"votes", Votes },
                {"isPlaying", IsPlaying },
                {"skipVotes", SkipVotes }
            };

            return dictionary;
        }
    }
}
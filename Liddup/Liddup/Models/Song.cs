using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Liddup.Models
{
    public class Song : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _votes;
        private bool _isPlaying;
        private int _skips;
        private bool _didVote;

        public string Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
        public string Source { get; set; }
        public object AlbumArt { get; set; }
        public byte[] Contents { get; set; }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying == value)
                    return;
                _isPlaying = value;

                OnPropertyChanged();
            }
        }

        public int Votes
        {
            get => _votes;
            set
            {
                if (_votes == value)
                    return;
                _votes = value;

                OnPropertyChanged();
            }
        }

        public int Skips
        {
            get => _skips;
            set
            {
                if (_skips == value)
                    return;
                _skips = value;

                OnPropertyChanged();
            }
        }

        public bool DidVote
        {
            get => _didVote;
            set
            {
                if (_didVote == value)
                    return;
                _didVote = value;

                OnPropertyChanged();
            }
        }

        public Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"title", Title },
                {"uri", Uri },
                {"source", Source },
                {"votes", _votes },
                {"isPlaying", _isPlaying },
                {"skips", _skips }
            };

            return dictionary;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

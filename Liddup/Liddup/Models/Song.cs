using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Liddup.Annotations;

namespace Liddup.Models
{
    public class Song : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _votes;

        public Song() : this("", "") { }

        public Song(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Uri { get; set; }
        public string SongSource { get; set; }

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

        public Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"title", Title },
                {"description", Description },
                {"uri", Uri },
                {"songsource", SongSource },
                {"votes", _votes }
            };

            return dictionary;
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

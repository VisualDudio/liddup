using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liddup.Models
{
    public class Song
    {
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
        public int Votes { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"title", Title },
                {"description", Description },
                {"uri", Uri },
                {"songsource", SongSource },
                {"votes", Votes }
            };

            return dictionary;
        }
    }
}

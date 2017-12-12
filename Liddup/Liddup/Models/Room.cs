using System.Collections.Generic;

namespace Liddup.Models
{
    public class Room
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool ExplicitSongsAllowed { get; set; }
        public int SongRequestLimit { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"code", Code },
                {"name", Name },
                {"explicitSongsAllowed", ExplicitSongsAllowed },
                {"songRequestLimit", SongRequestLimit },
            };

            return dictionary;
        }
    }
}
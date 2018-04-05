using Liddup.Models;
using System.Collections.ObjectModel;

namespace Liddup.Models
{
    public class Playlist
    {
        public ObservableCollection<Song> Songs { get; set; }
        public string Name { get; set; }
        public int NumberOfTracks { get; set; }
        public string ImageUrl { get; set; }
        public string OwnerId { get; set; }
        public string Id { get; set; }
    }
}

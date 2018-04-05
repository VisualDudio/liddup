using DLToolkit.Forms.Controls;
using Liddup.Models;
using PropertyChanged;
using SpotifyAPI.Web.Models;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class SearchResultsPageModel : BasePageModel
    {
        public List<Song> SearchResults { get; private set; }
        
        public SearchResultsPageModel()
        {
            MessagingCenter.Subscribe<AddSongsPageModel, List<FullTrack>>(this, "Search", UpdateUI);
        }

        private void UpdateUI(object sender, List<FullTrack> searchResults)
        {
            SearchResults = searchResults.Select(x => new Song { Title = x.Name, Uri = x.Uri, Artist = x.Artists[0].Name, ImageUrl = x.Album.Images[0].Url, DurationInSeconds = x.DurationMs / 1000, Source = "Spotify" }).ToList();
        }
    }
}

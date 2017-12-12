using Xamarin.Forms;
using System.Collections.ObjectModel;
using Liddup.TemplateSelectors;
using Liddup.Pages;
using System.Windows.Input;
using Liddup.Services;
using PropertyChanged;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class AddSongsPageModel : AppContainerPageModel
    {
        private static CancellationTokenSource _tokenSource;

        public new ObservableCollection<DataTemplate> Pages { get; private set; }
        public bool IsSpotifyTabActive { get; private set; } = true;
        public bool IsLocalFilesTabActive { get; private set; } = false;
        public bool IsSearchBoxVisible { get; private set; } = false;

        ICommand _switchtoSpotifyTabCommand;
        public ICommand SwitchtoSpotifyTabCommand => _switchtoSpotifyTabCommand ?? (_switchtoSpotifyTabCommand = new Command(() => SwitchToSpotifyTab()));

        ICommand _switchtoLocalFilesTabCommand;
        public ICommand SwitchtoLocalFilesTabCommand => _switchtoLocalFilesTabCommand ?? (_switchtoLocalFilesTabCommand = new Command(() => SwitchToLocalFilesTab()));

        ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new Command<string>(async s => await Search(s)));

        public bool IsBusy { get; private set; }

        public AddSongsPageModel()
        {
            Pages = new ObservableCollection<DataTemplate>
            {
                new DataTemplate(() => new SpotifyContainerPage()),
                new DataTemplate(() => new LocalLibraryPage())
            };
        }

        private void SwitchToSpotifyTab()
        {
            IsSpotifyTabActive = true;
            IsLocalFilesTabActive = false;
        }

        private void SwitchToLocalFilesTab()
        {
            IsSpotifyTabActive = false;
            IsLocalFilesTabActive = true;
        }
        
        private async Task Search(string query)
        {
            if (User.IsSpotifyAuthenticated && !string.IsNullOrEmpty(query))
            {
                using (_tokenSource = new CancellationTokenSource())
                {
                    try
                    {
                        IsBusy = true;
                        await Task.Delay(TimeSpan.FromSeconds(1), _tokenSource.Token);
                        var searchResults = await SpotifyApiManager.GetSearchResults(query, _tokenSource.Token);
                        MessagingCenter.Send(this, "Search", searchResults);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }
            }
            
            //if (string.IsNullOrWhiteSpace(query))
            //{
            //    IsSearchBoxVisible = false;
            //}
            //else
            //{
            //    IsSearchBoxVisible = true;
            //}
        }
    }
}

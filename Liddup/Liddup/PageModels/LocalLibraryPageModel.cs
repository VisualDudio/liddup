using Liddup.Models;
using Liddup.Services;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;
using PropertyChanged;
using System.Threading.Tasks;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class LocalLibraryPageModel : BasePageModel, ISongProvider
    {
        public IList<Song> Songs { get; private set; }
        private IList<IFile> _files;

        ICommand _addLocalFileToQueueCommand;
        public ICommand AddLocalFileToQueueCommand => _addLocalFileToQueueCommand ?? (_addLocalFileToQueueCommand = new Command<Song>(async s => await AddLocalFileToQueue(s)));

        public LocalLibraryPageModel()
        {
            LoadFiles();
        }

        private async void LoadFiles()
        {
            try
            {
                var path = DependencyService.Get<Services.IFileSystem>().GetMusicDirectory();
                var rootFolder = await FileSystem.Current.GetFolderFromPathAsync(path);
                _files = await rootFolder.GetFilesAsync();
                Songs = _files.Select(x => new Song { Title = x.Name, Uri = x.Path, Source = "Library" }).ToList();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async Task AddLocalFileToQueue(Song song)
        {
            var stream = await _files.FirstOrDefault(x => x.Name.Equals(song.Title))?.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);
            byte[] contents;

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                contents = memoryStream.ToArray();
                stream.Close();
            }

            song.Contents = contents;
            song.AddToQueueCommand.Execute(null);
        }
    }
}
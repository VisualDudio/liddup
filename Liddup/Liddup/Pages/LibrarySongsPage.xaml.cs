using System;
using System.IO;
using Liddup.Models;
using Liddup.Services;
using PCLStorage;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FileAccess = PCLStorage.FileAccess;

namespace Liddup.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LibrarySongsPage : ISongProvider
	{
		public LibrarySongsPage()
		{
			InitializeComponent();

		    LoadFiles();
		}

	    private async void LoadFiles()
	    {
	        try
	        {
	            var path = DependencyService.Get<Services.IFileSystem>().GetMusicDirectory();
	            var rootFolder = await FileSystem.Current.GetFolderFromPathAsync(path);
	            var files = await rootFolder.GetFilesAsync();

	            UserSongs.ItemsSource = files;
	        }
	        catch (Exception exception)
	        {
	            Console.WriteLine(exception);
	            throw;
	        }
        }

	    private async void UserSongs_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
	    {
	        var file = e.SelectedItem as IFile;
            var filePath = new Uri(file?.Path).AbsolutePath;
	        var stream = await file?.OpenAsync(FileAccess.ReadAndWrite);
	        byte[] contents;

            using (var memoryStream = new MemoryStream())
	        {
	            stream.CopyTo(memoryStream);
	            contents = memoryStream.ToArray();
	            stream.Close();
            }

            var song = new Song
	        {
                Title = file?.Name,
                Uri = filePath,
                Source = "Library",
                Votes = 0,
                Contents = contents
            };

            MessagingCenter.Send(this as ISongProvider, "AddSong", song);
        }
	}
}
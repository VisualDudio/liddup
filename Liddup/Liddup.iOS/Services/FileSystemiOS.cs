using Liddup.iOS.Services;
using Liddup.Services;
using MediaPlayer;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileSystemiOS))]

namespace Liddup.iOS.Services
{
    class FileSystemiOS : IFileSystem
    {
        public string GetMusicDirectory()
        {
            var query = new MPMediaQuery();
            var result = query.Items;
            
            return "";
        }

        public void DeleteDirectoryContents(string path)
        {
            
        }
    }
}
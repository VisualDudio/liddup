using Liddup.Droid;
using Liddup.Services;
using Xamarin.Forms;
using Environment = Android.OS.Environment;

[assembly: Dependency(typeof(FileSystemAndroid))]

namespace Liddup.Droid
{
    public class FileSystemAndroid : IFileSystem
    {
        public string GetMusicDirectory()
        {
            return Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMusic).AbsolutePath;
            //return Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads).AbsolutePath;
        }
    }
}
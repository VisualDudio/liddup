using System.Threading.Tasks;
using PCLStorage;

namespace Liddup.Services
{
    public static class FileSystemManager
    {
        public static async Task<string> WriteFileAsync(byte[] content, string songId)
        {
            var rootFolder = FileSystem.Current.LocalStorage;

            var fileName = string.Concat("Song_", songId, ".mp3");

            IFolder subFolder;
            var subFolderExists = await rootFolder.CheckExistsAsync("LiddupResources");

            if (subFolderExists == ExistenceCheckResult.NotFound)
                subFolder = await rootFolder.CreateFolderAsync("LiddupResources", CreationCollisionOption.ReplaceExisting);
            else
                subFolder = await rootFolder.GetFolderAsync("LiddupResources");

            var fileExists = await subFolder.CheckExistsAsync(fileName);

            if (fileExists == ExistenceCheckResult.NotFound)
            {
                var file = await subFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                var stream = await file.OpenAsync(FileAccess.ReadAndWrite);

                stream.Write(content, 0, content.Length);

                return file.Path;
            }

            var filePath = subFolder.Path + "/" + fileName;

            return filePath;
        }
    }
}

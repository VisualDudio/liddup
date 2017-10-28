using System;
using System.Collections.Generic;
using System.Text;

namespace Liddup.Services
{
    public interface IFileSystem
    {
        string GetMusicDirectory();
        void DeleteDirectoryContents(string path);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Liddup.iOS;
using Liddup.Services;
using UIKit;
using Xamarin.Forms;

using MediaPlayer;

[assembly: Dependency(typeof(FileSystemiOS))]

namespace Liddup.iOS
{
    class FileSystemiOS : IFileSystem
    {
        public string GetMusicDirectory()
        {
            var query = new MPMediaQuery();
            var result = query.Items;
            
            return "";
        }
    }
}
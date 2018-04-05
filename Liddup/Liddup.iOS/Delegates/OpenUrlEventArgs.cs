using Foundation;
using UIKit;

namespace Liddup.iOS.Delegates
{
    public class OpenUrlEventArgs
    {
        public UIApplication App { get; set; }
        public NSUrl Url { get; set; }
        public NSDictionary Options { get; set; }
    }
}
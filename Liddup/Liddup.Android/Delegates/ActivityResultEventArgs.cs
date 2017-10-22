using Android.App;
using Android.Content;

namespace Liddup.Droid
{
    public class ActivityResultEventArgs
    {
        public int RequestCode { get; set; }
        public Result ResultCode { get; set; }
        public Intent Data { get; set; }
    }
}
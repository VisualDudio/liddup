using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Liddup.Droid.Delegates;

namespace Liddup.Droid
{
    [Activity(Label = "Liddup", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public event EventHandler<ActivityResultEventArgs> ActivityResult = delegate { };
        public event EventHandler<DestroyEventArgs> Destroy = delegate { };

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
         
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);
            ActivityResult(this, new ActivityResultEventArgs
            {
                RequestCode = requestCode,
                ResultCode = resultCode,
                Data = intent
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(this, new DestroyEventArgs());
        }
    }
}


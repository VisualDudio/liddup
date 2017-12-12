using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Liddup.Droid.Delegates;
using Liddup.Droid.Services;
using Xamarin.Forms;
using CarouselView.FormsPlugin.Android;
using FFImageLoading.Forms.Droid;
using FFImageLoading.Svg.Forms;

namespace Liddup.Droid
{
    [Activity(Label = "Liddup", Icon = "@drawable/icon", Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public event EventHandler<ActivityResultEventArgs> ActivityResultDelegate = delegate { };
        public event EventHandler<DestroyEventArgs> Destroy = delegate { };

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            DependencyService.Register<NetworkManagerAndroid>();

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            CachedImageRenderer.Init();
            CarouselViewRenderer.Init();
            var ignore = typeof(SvgCachedImage);

            LoadApplication(new App());
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);
            ActivityResultDelegate(this, new ActivityResultEventArgs
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


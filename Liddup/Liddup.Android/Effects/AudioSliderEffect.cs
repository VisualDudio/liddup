using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Liddup.Droid.Effects;
using Xamarin.Forms;

[assembly: ResolutionGroupName("Liddup")]
[assembly: ExportEffect(typeof(AudioSliderEffect), "AudioSliderEffect")]
namespace Liddup.Droid.Effects
{
    class AudioSliderEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var seekBar = (SeekBar)Control;
            seekBar.ProgressDrawable.SetColorFilter(new PorterDuffColorFilter(Xamarin.Forms.Color.White.ToAndroid(), PorterDuff.Mode.SrcIn));
            seekBar.Thumb.SetColorFilter(new PorterDuffColorFilter(Xamarin.Forms.Color.White.ToAndroid(), PorterDuff.Mode.SrcIn));
            
        }

        protected override void OnDetached()
        {
            // Use this method if you wish to reset the control to original state
        }
    }
}
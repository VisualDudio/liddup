using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Liddup.Controls;
using Liddup.Droid.Renderers;

[assembly: ExportRenderer(typeof(Entry), typeof(UnderlinedEntryRenderer))]
namespace Liddup.Droid.Renderers
{
    class UnderlinedEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
    
            if (Control == null || Element == null || e.OldElement != null) return;

            var element = (UnderlinedEntry)Element;
            Control.Background.SetColorFilter(element.BorderColor.ToAndroid(), PorterDuff.Mode.SrcAtop);
            Control.LetterSpacing = element.LetterSpacing;

            UpdateLayout();
        }
    }
}
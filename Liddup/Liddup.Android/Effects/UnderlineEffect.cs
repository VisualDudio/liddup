using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Liddup.Droid.Effects;
using Xamarin.Forms;
using System;

[assembly: ExportEffect(typeof(UnderlineEffect), "UnderlineEffect")]
namespace Liddup.Droid.Effects
{
    class UnderlineEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            SetUnderline(true);
        }

        protected override void OnDetached()
        {
            SetUnderline(false);
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == Label.TextProperty.PropertyName || args.PropertyName == Label.FormattedTextProperty.PropertyName)
                SetUnderline(true);
        }

        private void SetUnderline(bool underlined)
        {
            try
            {
                var textView = (TextView)Control;
                if (underlined)
                    textView.PaintFlags |= PaintFlags.UnderlineText;
                else
                    textView.PaintFlags &= ~PaintFlags.UnderlineText;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot underline Label. Error: ", ex.Message);
            }
        }
    }
}
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using System.ComponentModel;
using CoreAnimation;
using Foundation;
using Liddup.iOS.Renderers;
using Liddup.Controls;

[assembly: ExportRenderer(typeof(UnderlinedEntry), typeof(UnderlinedEntryRenderer))]
namespace Liddup.iOS.Renderers
{
    public class UnderlinedEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderStyle = UITextBorderStyle.None;

                var view = (Element as UnderlinedEntry);
                if (view != null)
                {
                    DrawBorder(view);
                    SetFontSize(view);
                    SetPlaceholderTextColor(view);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var view = (UnderlinedEntry)Element;

            if (e.PropertyName.Equals(view.BorderColor))
                DrawBorder(view);
            if (e.PropertyName.Equals(view.FontSize))
                SetFontSize(view);
            if (e.PropertyName.Equals(view.PlaceholderColor))
                SetPlaceholderTextColor(view);
        }

        void DrawBorder(UnderlinedEntry view)
        {
            var borderLayer = new CALayer
            {
                MasksToBounds = true,
                Frame = new CoreGraphics.CGRect(0f, Frame.Height / 2, Frame.Width, 1f),
                BorderColor = view.BorderColor.ToCGColor(),
                BorderWidth = 1.0f
            };

            Control.Layer.AddSublayer(borderLayer);
            Control.BorderStyle = UITextBorderStyle.None;
        }

        void SetFontSize(UnderlinedEntry view)
        {
            if (view.FontSize != Font.Default.FontSize)
                Control.Font = UIFont.SystemFontOfSize((System.nfloat)view.FontSize);
            else if (view.FontSize == Font.Default.FontSize)
                Control.Font = UIFont.SystemFontOfSize(17f);
        }

        void SetPlaceholderTextColor(UnderlinedEntry view)
        {
            if (string.IsNullOrEmpty(view.Placeholder) == false && view.PlaceholderColor != Color.Default)
            {
                var placeholderString = new NSAttributedString(view.Placeholder,
                                            new UIStringAttributes { ForegroundColor = view.PlaceholderColor.ToUIColor() });
                Control.AttributedPlaceholder = placeholderString;
            }
        }
    }
}
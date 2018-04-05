using FFImageLoading.Svg.Forms;
using FFImageLoading.Transformations;
using Xamarin.Forms;

namespace Liddup.Controls
{
    public class ToggleIcon : SvgCachedImage
    {
        public static readonly BindableProperty IsTintedProperty = BindableProperty.Create(nameof(IsTinted), typeof(bool), typeof(ToggleIcon), false, propertyChanged: IsTintedChanged);
        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(string), typeof(ToggleIcon), "#ff2d2d", propertyChanged: TintColorChanged);

        public string TintColor
        {
            get { return (string)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }
        public bool IsTinted
        {
            get { return (bool)GetValue(IsTintedProperty); }
            set { SetValue(IsTintedProperty, value); }
        }

        private void Tint()
        {
            Transformations = new System.Collections.Generic.List<FFImageLoading.Work.ITransformation>()
            {
                new TintTransformation(TintColor)
                {
                    EnableSolidColor = true
                }
            };
        }

        private static void IsTintedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var icon = bindable as ToggleIcon;

            if ((bool)newValue)
                icon.Tint();
            else
                icon.Transformations = new System.Collections.Generic.List<FFImageLoading.Work.ITransformation>();
        }

        private static void TintColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var icon = bindable as ToggleIcon;
            icon.TintColor = (string)newValue;
        }
    }
}

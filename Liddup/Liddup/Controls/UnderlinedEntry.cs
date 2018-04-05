using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Liddup.Controls
{
    public class UnderlinedEntry : Entry
    {
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create("BorderColor", typeof(Color), typeof(Entry), Color.White);
        public static readonly BindableProperty LetterSpacingProperty = BindableProperty.Create("LetterSpacing", typeof(float), typeof(Entry), 0.0f);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public float LetterSpacing
        {
            get { return (float)GetValue(LetterSpacingProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
    }
}

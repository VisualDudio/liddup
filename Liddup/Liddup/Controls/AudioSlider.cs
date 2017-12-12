using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Liddup.Controls
{
    public class AudioSlider : Slider
    {
        public static readonly BindableProperty HasThumbProperty =
               BindableProperty.Create(nameof(HasThumb), typeof(bool), typeof(AudioSlider), true);

        public bool HasThumb
        {
            get { return (bool)GetValue(HasThumbProperty); }
            set { SetValue(HasThumbProperty, value); }
        }
    }
}

using System;
using System.Globalization;
using Xamarin.Forms;

namespace Liddup.Converters
{
    public class SecondsToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                var seconds = TimeSpan.FromSeconds((int)value);

                return $"{seconds.ToString(@"mm\:ss")}";
            }

            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

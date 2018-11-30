using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContextMenuSample.Converters
{
    internal class IsMutedToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isMuted)
            {
                var result = isMuted ? Color.Gray : Color.Green;
                return result;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ContextMenuSample.Converters
{
    internal class IsMutedToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isMuted)
            {
                var result = isMuted ? "Move to unmute" : "Move to mute";
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
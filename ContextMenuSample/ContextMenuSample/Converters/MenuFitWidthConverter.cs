using System;
using System.Globalization;
using Xamarin.Forms;
namespace ContextMenuSample.Converters
{
    public class MenuFitWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => ((double)value) - double.Parse(parameter.ToString());

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

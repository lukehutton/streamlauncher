using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace StreamLauncher.Wpf.Infrastructure.Converters
{
    public class ConvertImagePathForFeedToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : new BitmapImage(new Uri(value.ToString(), UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
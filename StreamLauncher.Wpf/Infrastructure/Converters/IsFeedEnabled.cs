using System;
using System.Globalization;
using System.Windows.Data;

namespace StreamLauncher.Wpf.Infrastructure.Converters
{
    public class IsFeedEnabled : IMultiValueConverter
    {      
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0] != null && System.Convert.ToBoolean(values[0]) && // isplaying
                   values[1] != null;                                          // text on feed button not empty
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
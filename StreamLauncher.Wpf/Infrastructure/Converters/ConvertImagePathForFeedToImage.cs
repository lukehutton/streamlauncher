﻿using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace StreamLauncher.Wpf.Infrastructure.Converters
{
    public class ConvertImagePathForTeamToImage : IValueConverter
    {
        private readonly string[] _resourceNames = GetResourceNames();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resourceName = Uri.EscapeUriString(value.ToString().Remove(0, 3).ToLower());
            return !_resourceNames.Contains(resourceName)
                ? new BitmapImage(new Uri(@"../Images/Teams/Not Available.png", UriKind.RelativeOrAbsolute))
                : new BitmapImage(new Uri(value.ToString(), UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string[] GetResourceNames()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resName = assembly.GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName))
            {
                using (var reader = new ResourceReader(stream))
                {
                    return reader.Cast<DictionaryEntry>().Select(entry =>
                        (string) entry.Key).ToArray();
                }
            }
        }
    }
}
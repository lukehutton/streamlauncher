using System;
using System.Text;

namespace StreamLauncher.Util
{
    public static class StringExtensions
    {
        public static string Fmt(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static T ParseEnum<T>(this string value)
        {
            return (T) Enum.Parse(typeof (T), value, true);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string MaxStrLen(this string value, int length)
        {
            if (value.Length > length)
                return value.Substring(0, length) + "..";
            return value;
        }
        public static string Repeat(this string value, int times)
        {
            if (!string.IsNullOrEmpty(value))
            {
                StringBuilder builder = new StringBuilder(value.Length * times);

                for (int i = 0; i < times; i++) builder.Append(value);

                return builder.ToString();
            }

            return string.Empty;
        }
    }
}
using System;

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

        public static string MaxStrLen(this string value, int length)
        {
            if (value.Length > length)
                return value.Substring(0, length) + "..";
            return value;
        }
    }
}
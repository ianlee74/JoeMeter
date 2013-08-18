using System;

namespace MicroWebServer.Json
{
    public static class JsonDateTime
    {
        public static DateTime Basis { get {return new DateTime(1970, 1, 1);} }
        /// <summary>
        /// The ASP.NET Ajax team made up their own time date format for JSON strings, and it's
        /// explained in this article: http://msdn.microsoft.com/en-us/library/bb299886.aspx
        /// Converts a DateTime to the ASP.NET Ajax JSON format.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToASPNetAjax(this DateTime dt)
        {
            string value = ((dt-Basis).Ticks/10000).ToString();

            return @"\/Date(" + value + @")\/";
        }

        /// <summary>
        /// Converts an ASP.NET Ajaz JSON string to DateTime
        /// </summary>
        /// <param name="ajax"></param>
        /// <returns></returns>
        public static DateTime FromASPNetAjax(this string ajax)
        {
            var parts1 = ajax.Split(new[] { '(', ')' });
            var parts2 = parts1[1].Split(new[] {'+', '-'});

            long ticks = Convert.ToInt64(parts2[0])*10000;
            int offset = 0;
            if (parts2.Length>1) offset = Convert.ToInt32(parts2[1])/100;

            // Create a Utc DateTime based on the tick count
            var dt = Basis.Add(new TimeSpan(ticks)).AddHours(offset);// new DateTime(ticks, DateTimeKind.Local);

            return dt;
        }
    }
}

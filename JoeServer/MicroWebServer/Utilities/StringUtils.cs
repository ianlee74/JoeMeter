using System.Text;

namespace MicroWebServer
{
    /// <summary>
    /// StringUtils class.
    /// </summary>
    public static class StringUtils
    {
        private static readonly Decoder decoder = Encoding.UTF8.GetDecoder();

        /// <summary>
        /// Convert a byte array to UTF8 string.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <param name="offset">The offset in the byte array for the first character to convert.</param>
        /// <param name="count">The number of bytes starting from offset to convert.</param>
        /// <returns>The converted string.</returns>
        public static string ByteArrayToString(byte[] bytes, int offset, int count)
        {
            if (count > 0)
            {
                int bytesUsed, charsUsed;
                bool completed;
                var chars = new char[count];
                decoder.Convert(bytes, offset, count, chars, 0, count, true, out bytesUsed, out charsUsed, out completed);
                return new string(chars);
            }
            return "";
        }

        /// <summary>
        /// Checks if a string starts with the given part.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <param name="part">The part to check for.</param>
        /// <returns>True when input starts with the given part.</returns>
        public static bool StartsWith(string input, string part)
        {
            if (input.Length < part.Length)
                return false;

            for (int i = 0; i < part.Length; i++)
                if (input[i] != part[i])
                    return false;

            return true;
        }
    }
}

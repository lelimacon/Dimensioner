namespace Dimensioner.Utils
{
    internal static class StringUtils
    {
        /// <summary>
        ///     Tries to convert the specified input to an integer.
        /// </summary>
        /// <param name="input">The input number as a string.</param>
        /// <param name="defaultValue">The default value if the conversion failed.</param>
        /// <returns>The int, or default value if fail.</returns>
        public static int ToInt(this string input, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(input))
                return defaultValue;
            bool success = int.TryParse(input, out int res);
            if (!success)
                return defaultValue;
            return res;
        }
    }
}

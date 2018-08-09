using System;
using System.Diagnostics;
using System.Text;

namespace Dimensioner.Utils
{
    internal static class StringUtils
    {
        private readonly static Random Rand;
        private readonly static Object RandLock;

        static StringUtils()
        {
            int seed = Process.GetCurrentProcess().Id ^ (int)DateTime.Now.Ticks;
            Rand = new Random(seed);
            RandLock = new Object();
        }

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

        /// <summary>
        ///     Generates a random string with uppercase letters of the given size.
        /// </summary>
        /// <param name="size">The size of the generated string.</param>
        /// <returns>A 'unique' instance of a string.</returns>
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                double rand;
                lock (RandLock)
                    rand = Rand.NextDouble();
                char c = Convert.ToChar((int)(Math.Floor(26 * rand + 65)));
                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}

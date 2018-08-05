using System;

namespace Dimensioner.Utils
{
    public static class EnumUtils
    {
        /// <summary>
        ///     Tries to parse the specified value (ignores case).a
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum (infered by default value).</typeparam>
        /// <param name="enumDefault">The default value.</param>
        /// <param name="value">The value to be parsed.</param>
        /// <returns>The enum value, or enumDefault if not found.</returns>
        public static TEnum TryParse<TEnum>(this TEnum enumDefault, string value)
            where TEnum : struct
        {
            var success = Enum.TryParse(value, true, out TEnum result);
            return success ? result : enumDefault;
        }
    }
}

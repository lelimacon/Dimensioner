using System;
using System.Collections.Generic;

namespace Dimensioner.Utils
{
    public static class SwitchUtils
    {
        /// <summary>
        ///     A switch using a dictionary of actions.
        ///     Silent fallback when the key is not found.
        /// </summary>
        public static void Switch<T>(this T key, Dictionary<T, Action> values)
        {
            if (values.ContainsKey(key))
                values[key]();
        }

        /// <summary>
        ///     A switch that returns the value of the first occurence of the key.
        /// </summary>
        /// <return>The value of the dictionary entry, or default value if not found.</return>
        public static TRet Switch<T, TRet>(this T key, Dictionary<T, TRet> values, TRet defaultValue)
        {
            if (values.ContainsKey(key))
                return values[key];
            return defaultValue;
        }

        /// <summary>
        ///     A switch that returns the value of the first occurence of the key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <return>The value of the dictionary entry, or default value if not found.</return>
        public static TRet Switch<T, TRet>(this T key, Dictionary<T, TRet> values)
        {
            if (values.ContainsKey(key))
                return values[key];
            return default(TRet);
        }

        /// <summary>
        ///     A switch that returns the result of the function's application.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <return>The result of the application of the function, or default value if not found.</return>
        public static TRet Switch<T, TRet>(this T key, Dictionary<T, Func<TRet>> values)
        {
            if (values.ContainsKey(key))
                return values[key]();
            return default(TRet);
        }

        /// <summary>
        ///     An exhaustive switch.
        ///     All possibilities must be declared in dictionary.
        /// </summary>
        public static void SwitchEx<T>(this T key, Dictionary<T, Action> values)
        {
            if (!values.ContainsKey(key))
                throw new IndexOutOfRangeException($"Key not found in switch: {key}");
            values[key]();
        }
    }
}

using System;
using System.Diagnostics;

namespace Hamwic.Cif.Core
{
    /// <summary>
    /// Provides standard argument validation methods
    /// </summary>
    public static class Argument
    {
        [DebuggerStepThrough]
        public static void IsNull(object value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(InitName(name));
        }

        [DebuggerStepThrough]
        public static void IsNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("The string is null or empty.",
                    InitName(name));
        }

        [DebuggerStepThrough]
        private static string InitName(string name)
        {
            return string.IsNullOrEmpty(name) ? "UNKNOWN" : name;
        }

        public static T IsOfType<T>(object value, string name) where T : class
        {
            if (value == null)
                return default(T);

            var typedParam = value as T;
            if (typedParam == null)
                throw new ArgumentException(string.Format("Expected object of type {0} but was {1}",
                        typeof (T).Name,
                        value.GetType().Name),
                    name);

            return typedParam;
        }
    }
}
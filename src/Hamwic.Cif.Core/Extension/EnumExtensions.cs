using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Hamwic.Cif.Core.Attributes;
using Hamwic.Cif.Core.CoreEntities;

namespace Hamwic.Cif.Core.Extension
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> Parse<T>(IEnumerable<string> values) where T : struct, IConvertible
        {
            return values.Select(Parse<T>);
        }

        public static object Parse(Type enumType, string value, bool ignoreCase = true)
        {
            Argument.IsNullOrEmpty(value, nameof(value));

            value = value.Trim();

            if (!enumType.IsEnum)
                throw new ArgumentException("T must be an enum.");

            return Enum.Parse(enumType, value, ignoreCase);
        }

        public static T Parse<T>(string value) where T : struct, IConvertible
        {
            return Parse<T>(value, true);
        }

        public static T Parse<T>(string value, bool ignoreCase) where T : struct, IConvertible
        {
            Argument.IsNullOrEmpty(value, nameof(value));

            value = value.Trim();

            if (!typeof (T).IsEnum)
                throw new ArgumentException("T must be an enum.");

            return (T) Enum.Parse(typeof (T), value, ignoreCase);
        }

        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return (((int) (object) type & (int) (object) value) == (int) (object) value);
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return (int) (object) type == (int) (object) value;
            }
            catch
            {
                return false;
            }
        }

        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T) (object) ((int) (object) type | (int) (object) value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"Could not append value from enumerated type '{typeof (T).Name}'", ex);
            }
        }

        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T) (object) ((int) (object) type & ~(int) (object) value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException
                    ($"Could not remove value from enumerated type '{typeof (T).Name}'", ex);
            }
        }

        public static IEnumerable<T> GetAllSelectedItems<T>(this T value) where T : struct
        {
            var valueAsInt = Convert.ToInt32(value, CultureInfo.InvariantCulture);

            foreach (var item in Enum.GetValues(typeof (T)))
            {
                var itemAsInt = Convert.ToInt32(item, CultureInfo.InvariantCulture);

                if (itemAsInt == (valueAsInt & itemAsInt))
                    yield return (T) item;
            }
        }
        
        public static IEnumerable<T> GetAllVisibleSelectedItems<T>(this T value) where T : struct
        {
            var valueAsInt = Convert.ToInt32(value, CultureInfo.InvariantCulture);

            var enumeration = typeof (T);
            foreach (var enumName in Enum.GetNames(enumeration))
            {
                var member = enumeration.GetField(enumName);
                if (member.HasAttribute<OmitFromReflectedListAttribute>())
                    continue;

                var itemAsInt = (int) Enum.Parse(enumeration, enumName);

                if (itemAsInt == (valueAsInt & itemAsInt))
                    yield return (T) Enum.ToObject(enumeration, itemAsInt);
            }
        }

        public static IEnumerable<Enum> GetFlagValues(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (Convert.ToInt32(value, CultureInfo.InvariantCulture) != 0 && input.HasFlag(value))
                    yield return value;
        }

        public static T SetAllFlags<T>(T baseValue, IEnumerable<string> flags) where T : struct, IConvertible
        {
            var intBaseValue = (int) (object) baseValue;
            var result = intBaseValue;
            result = flags.Aggregate(result, (current, flag) => current | (int) (object) Parse<T>(flag));

            // if we're anything but the default, remove the default
            if (result != intBaseValue)
                result = result & ~intBaseValue;

            return (T) Enum.ToObject(typeof (T), result);
        }

        public static T SetAllFlags<T>(T baseValue, IEnumerable<int> flags) where T : struct, IConvertible
        {
            var intBaseValue = (int) (object) baseValue;
            var result = intBaseValue;
            result = flags.Aggregate(result, (current, flag) => current | (int) (object) flag);

            // if we're anything but the default, remove the default
            if (result != intBaseValue)
                result = result & ~intBaseValue;

            return (T) Enum.ToObject(typeof (T), result);
        }
        
        /// <summary>
        ///     Converts an enum into a string keyed dictionary with
        ///     a friendly descriptive name where possible
        /// </summary>
        public static IDictionary<string, string> AsStringKeyedDictionary(this Type enumeration)
        {
            var results = new Dictionary<string, string>();
            AsKeyedDictionaryImpl(enumeration, results.Add);
            return results;
        }

        public static IDictionary<string, Tuple<string, int>> AsOrderedStringKeyedDictionary(this Type enumeration)
        {
            var results = new Dictionary<string, Tuple<string, int>>();
            AsSortedKeyedDictionaryImpl(enumeration, results.Add);
            return results;
        }

        public static IDictionary<int, string> AsIntKeyedDictionary(this Type enumeration)
        {
            var results = new Dictionary<int, string>();
            AsKeyedDictionaryImpl(enumeration, (key, value) => results.Add((int) Enum.Parse(enumeration, key), value));
            return results;
        }
        
        private static void AsKeyedDictionaryImpl(this Type enumeration, Action<string, string> addMethod)
        {
            if (!enumeration.IsEnum)
                throw new ArgumentException("This method can only be called for Enum types", nameof(enumeration));

            foreach (var enumName in Enum.GetNames(enumeration))
            {
                var member = enumeration.GetField(enumName);
                if (member.HasAttribute<OmitFromReflectedListAttribute>())
                    continue;

                addMethod(enumName,
                    member.HasAttribute<DescriptionAttribute>()
                        ? member.GetAttribute<DescriptionAttribute>().Description
                        : member.Name.SplitOnCapitalLetter());
            }
        }

        private static void AsSortedKeyedDictionaryImpl(this Type enumeration,
            Action<string, Tuple<string, int>> addMethod)
        {
            if (!enumeration.IsEnum)
                throw new ArgumentException("This method can only be called for Enum types", nameof(enumeration));

            var sortOrderPostion = 0;
            foreach (var enumName in Enum.GetNames(enumeration))
            {
                var member = enumeration.GetField(enumName);
                if (member.HasAttribute<OmitFromReflectedListAttribute>())
                    continue;

                addMethod(enumName,
                    new Tuple<string, int>(member.HasAttribute<DescriptionAttribute>()
                            ? member.GetAttribute<DescriptionAttribute>().Description
                            : member.Name.SplitOnCapitalLetter(),
                        member.HasAttribute<SortOrderAttribute>()
                            ? member.GetAttribute<SortOrderAttribute>().Position
                            : sortOrderPostion));
                sortOrderPostion++;
            }
        }
    }
}
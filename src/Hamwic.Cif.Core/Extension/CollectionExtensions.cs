using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Hamwic.Core.Extension
{
    /// <summary>
    ///     Contains some useful extensions for working with collections.
    /// </summary>
    public static class CollectionExtensions
    {

        private class OrderByInfo
        {
            public string PropertyName { get; set; }
            public SortDirection Direction { get; set; }
            public bool Initial { get; set; }
        }

        private enum SortDirection
        {
            Ascending = 0,
            Descending = 1
        }

        /// <summary>
        ///     Each extension that enumerates over all items in an <see cref="IEnumerable{T}" /> and executes
        ///     an action.
        /// </summary>
        /// <typeparam name="T">The type that this extension is applicable for.</typeparam>
        /// <param name="collection">The enumerable instance that this extension operates on.</param>
        /// <param name="action">The action executed for each item in the enumerable.</param>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            var enumerable = collection as T[] ?? collection.ToArray();
            foreach (var item in enumerable)
                action(item);

            return enumerable;
        }

        public static IEnumerable<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> other)
        {
            foreach (var item in other)
                collection.Add(item);

            return collection;
        }

        public static IEnumerable<Tuple<string, string>> ToTuples(this NameValueCollection collection)
        {
            return collection.AllKeys.Select(item => new Tuple<string, string>(item, collection[item]));
        }

        public static bool ContainsAsString(this IEnumerable<string> collection, object value)
        {
            var stringValue = (value ?? "") as string;
            return collection.Any(x => string.Equals(x, stringValue, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Each extension that enumerates over all items in an <see cref="IEnumerator{T}" /> and executes
        ///     an action.
        /// </summary>
        /// <typeparam name="T">The type that this extension is applicable for.</typeparam>
        /// <param name="collection">The enumerator instance that this extension operates on.</param>
        /// <param name="action">The action executed for each item in the enumerable.</param>
        public static IEnumerator<T> Each<T>(this IEnumerator<T> collection, Action<T> action)
        {
            while (collection.MoveNext())
                action(collection.Current);

            return collection;
        }

        /// <summary>
        ///     Finds the first entry in a collection with the id specified
        /// </summary>
        /// <typeparam name="T">Type of object the collection holds</typeparam>
        /// <typeparam name="TId">The type of the id property of the object</typeparam>
        /// <param name="collection">The collection to be queries</param>
        /// <param name="id">The id to find</param>
        /// <returns>The object found or null if it doesn't exist</returns>
        public static T FindWithId<T, TId>(this IEnumerable<T> collection, TId id) where T : IKeyedEntity<TId>
        {
            return collection.FirstOrDefault(x => x.Id.Equals(id));
        }

        /// <summary>
        ///     Provides a simple concatenation of functions that return a string value
        /// </summary>
        /// <param name="collection">The collection of to be queried</param>
        /// <param name="action">Function to return a string to join</param>
        /// <param name="delimiter">String to place in between each collection item</param>
        /// <returns>Delimited string</returns>
        public static string Concat<T>(this IEnumerable<T> collection, Func<T, string> action, string delimiter)
        {
            var enumerable = collection as T[] ?? collection.ToArray();
            if (!enumerable.Any())
                return string.Empty;

            var safeDelimiter = (delimiter ?? string.Empty);
            var returnMessage = new StringBuilder();
            enumerable.Each(item => returnMessage.AppendFormat("{0}{1}", action(item) ?? string.Empty, safeDelimiter));
            returnMessage.Length -= safeDelimiter.Length;
            return returnMessage.ToString();
        }

        /// <summary>
        ///     Provides a simple concatenation of validation errors, usually
        ///     for display in tests
        /// </summary>
        /// <param name="errors">The collection of validation errors</param>
        /// <returns>New line delimited string with spaces in between propertyname and message</returns>
        public static string Concat(this IEnumerable<ValidationError> errors)
        {
            return Concat(errors, x => $"{x.PropertyName} {x.Message}", "\r\n");
        }

        /// <summary>
        ///     Provides a simple concatenation of validation errors, usually
        ///     for display in tests
        /// </summary>
        /// <param name="errors">The collection of validation errors</param>
        /// <returns>New line delimited string with spaces in between propertyname and message</returns>
        public static string Concat(this IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> errors)
        {
            var result = new StringBuilder();
            foreach (var err in errors)
            {
                if (err is CompositeValidationResult)
                {
                    result.AppendLine(err.ErrorMessage);
                    var composite = (CompositeValidationResult)err;
                    foreach (var subError in composite.Results)
                    {
                        result.AppendLine(subError.ErrorMessage);
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        ///     Provides a simple concatenation of functions that return a string value
        /// </summary>
        /// <param name="collection">The collection of to be queried</param>
        /// <param name="action">Function to return a string to join</param>
        /// <returns>Comma delimited string</returns>
        public static string Concat<T>(this IEnumerable<T> collection, Func<T, string> action)
        {
            return Concat(collection, action, ",");
        }


        public static IEnumerable<KeyValuePair<string, string>> ToEnumerableKeyValuePairs<T>(this IEnumerable<T> source,
            Func<T, string> key, Func<T, string> value)
        {
            return source.Select(item => new KeyValuePair<string, string>(key(item), value(item)));
        }

        public static IDictionary<TKey, TValue> MergeAttributeDictionary<TKey, TValue>(
            this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target) where TValue : class
        {
            foreach (var entry in target)
            {
                if (source.ContainsKey(entry.Key))
                    source[entry.Key] = source[entry.Key] + " " + entry.Value as TValue;
                else
                    source.Add(entry.Key, entry.Value);
            }

            return source;
        }

        public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
        {
            ArrayList.Adapter((IList) list).Sort((IComparer) new ComparisonComparer<T>(comparison));
        }

        public static void Sort<T>(this IList<T> list, IComparer comparer)
        {
            ArrayList.Adapter((IList) list).Sort(comparer);
        }

        // Convenience method on IEnumerable<T> to allow passing of a
        // Comparison<T> delegate to the OrderBy method.
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, Comparison<T> comparison)
        {
            return list.OrderBy<T, T>(t => t, new ComparisonComparer<T>(comparison));
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string orderBy)
        {
            return enumerable.AsQueryable().OrderBy(orderBy).AsEnumerable();
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> collection, string orderBy)
        {
            foreach (var orderByInfo in ParseOrderBy(orderBy))
                collection = ApplyOrderBy(collection, orderByInfo);

            return collection;
        }

        private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> collection, OrderByInfo orderByInfo)
        {
            var props = orderByInfo.PropertyName.Split('.');
            var type = typeof (T);

            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (var prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                var pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var delegateType = typeof (Func<,>).MakeGenericType(typeof (T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);
            string methodName;

            if (!orderByInfo.Initial && collection is IOrderedQueryable<T>)
            {
                methodName = orderByInfo.Direction == SortDirection.Ascending ? "ThenBy" : "ThenByDescending";
            }
            else
            {
                methodName = orderByInfo.Direction == SortDirection.Ascending ? "OrderBy" : "OrderByDescending";
            }

            //TODO: apply caching to the generic methodsinfos?
            return (IOrderedQueryable<T>) typeof (Queryable).GetMethods().Single(
                method => method.Name == methodName
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof (T), type)
                .Invoke(null, new object[] {collection, lambda});
        }

        private static IEnumerable<OrderByInfo> ParseOrderBy(string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
                yield break;

            var items = orderBy.Split(',');
            var initial = true;
            foreach (var item in items)
            {
                var pair = item.Trim().Split(' ');

                if (pair.Length > 2)
                    throw new ArgumentException(
                        String.Format(
                            "Invalid OrderBy string '{0}'. Order By Format: Property, Property2 ASC, Property2 DESC",
                            item));

                var prop = pair[0].Trim();

                if (String.IsNullOrEmpty(prop))
                    throw new ArgumentException(
                        "Invalid Property. Order By Format: Property, Property2 ASC, Property2 DESC");

                var dir = SortDirection.Ascending;

                if (pair.Length == 2)
                    dir = ("desc".Equals(pair[1].Trim(), StringComparison.OrdinalIgnoreCase) ||
                           "descending".Equals(pair[1].Trim(), StringComparison.OrdinalIgnoreCase))
                        ? SortDirection.Descending
                        : SortDirection.Ascending;

                yield return new OrderByInfo
                {
                    PropertyName = prop,
                    Direction = dir,
                    Initial = initial
                };

                initial = false;
            }
        }


        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> collection,
            int minNumberToSelect,
            int maxNumberToSelect)
        {
            if (minNumberToSelect <= 0)
                throw new ArgumentException("You must select at least 1 item", nameof(minNumberToSelect));

            var source = collection.ToList();
            if (minNumberToSelect > source.Count)
                throw new ArgumentException("Collection does not contain the minimum number of required items", nameof(minNumberToSelect));

            if (maxNumberToSelect > source.Count)
                maxNumberToSelect = source.Count;

            if (maxNumberToSelect < minNumberToSelect)
                throw new ArgumentException("Maximum number of selections must be greater than minimum");

            var results = new List<IEnumerable<T>>();

            for (var numberToSelect = minNumberToSelect; numberToSelect <= maxNumberToSelect; numberToSelect++)
            {
                results.AddRange(Combinations(source, numberToSelect));
            }

            return results;
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> collection,
            int numberToSelect)
        {
            if (numberToSelect <= 0)
                throw new ArgumentException("You must select at least 1 item", nameof(numberToSelect));

            var source = collection.ToList();

            var i = 0;
            foreach (var item in source)
            {
                if (numberToSelect == 1)
                    yield return new[] { item };
                else
                {
                    foreach (var result in Combinations(source.Skip(i + 1), numberToSelect - 1))
                        yield return new[] { item }.Concat(result);
                }

                ++i;
            }
        }

        public static IEnumerable<IEnumerable<T>> InSetsOf<T>(this IEnumerable<T> source, int max)
        {
            var toReturn = new List<T>(max);
            foreach (var item in source)
            {
                toReturn.Add(item);
                if (toReturn.Count == max)
                {
                    yield return toReturn;
                    toReturn = new List<T>(max);
                }
            }

            if (toReturn.Any())
                yield return toReturn;
        }


        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> getValue)
        {
            if (!source.TryGetValue(key, out var value))
            {
                value = getValue();
                source.Add(key, value);
            }
            return value;
        }

        public static TValue SafeGet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue)
        {
            return !source.TryGetValue(key, out var value) ? defaultValue : value;
        }

        public static void SafeAdd<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, value);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string ToJustifiedTable(this IEnumerable<IEnumerable<string>> source,
            IEnumerable<int> widths, 
            IEnumerable<int> alignRights = null)
        {
            var rows = source.Select(x => x.ToArray()).ToArray();
            var colWidths = widths.ToArray();

            var result = new StringBuilder();
            
            var alignRightsArray = alignRights as int[] ?? (alignRights ?? new int[0]).ToArray();

            if (alignRightsArray.Any(x => x < 0 || x > colWidths.Length))
                throw new ArgumentException($"Align rights array {alignRightsArray} contains invalid column indexes");

            for (var i = 0; i < rows.Length; i++)
            {
                if (colWidths.Length != rows[i].Length)
                    throw new ArgumentException($"Item at index {i} does not contain the same " +
                                                $"number of fields specified by column widths {colWidths}");

                var row = new StringBuilder();
                for (var j = 0; j < rows[i].Length; j++)
                {
                    row.Append(alignRightsArray.Contains(j)
                        ? rows[i][j].PadLeft(colWidths[j])
                        : rows[i][j].PadRight(colWidths[j]));
                }

                result.AppendLine(row.ToString());
            }

            return result.ToString();
        }
    }
}
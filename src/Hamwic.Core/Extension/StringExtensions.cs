using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Hamwic.Core.Constants;

namespace Hamwic.Core.Extension
{
    public static class StringExtensions
    {
        public static readonly Regex CapitalLetterRegex = new Regex("([A-Z])", RegexOptions.Compiled);
        public static readonly HashAlgorithm Hash = new SHA256Managed();
        public static readonly Regex WhitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
        public static readonly Regex NormaliseRegex = new Regex(@"[^a-zA-Z0-9]");
        public static readonly Regex ExtractPostcodeRegex = new Regex(@"([A-Z]{1,2}[0-9][0-9A-Z]?\s?[0-9][A-Z]{2})", RegexOptions.Compiled);
        public static readonly Regex ExtractEmailRegex = new Regex(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])", RegexOptions.Compiled);
        public static readonly string[] ImageFileExtensions = { "png", "gif", "jpg", "jpeg", "bmp" };
        public static readonly string[] MergeableFileExtensions = ImageFileExtensions.Concat(new[]{"pdf"}).ToArray();
        public static readonly char[] WhitespaceAndNewLines =
        {
            '\r', '\n', ' ', '\t'
            
        };

        public static readonly Regex HtmlAsTextRegex = new Regex(@"<br\s?/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Dictionary<string, DayOfWeek> ShortDayMap = new Dictionary<string, DayOfWeek>
        {
            { "sat", DayOfWeek.Saturday },
            { "sun", DayOfWeek.Sunday },
            { "mon", DayOfWeek.Monday },
            { "tue", DayOfWeek.Tuesday },
            { "wed", DayOfWeek.Wednesday },
            { "thu", DayOfWeek.Thursday },
            { "fri", DayOfWeek.Friday },
        };

        public static string SplitOnCapitalLetter(this string s)
        {
            return string.IsNullOrEmpty(s) ? s : CapitalLetterRegex.Replace(s, " $1").TrimStart();
        }

        public static string ToTitleCase(this string s)
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((s ?? "").ToLower());
        }

        public static string HtmlAsText(this string s)
        {
            return string.IsNullOrEmpty(s) ? s : HtmlAsTextRegex.Replace(s, Environment.NewLine).Trim();
        }

        public static string CoalesceWhitespace(params string[] args)
        {
            return args.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        }

        public static string RemoveWhitespace(this string s)
        {
            return string.IsNullOrEmpty(s) ? s : WhitespaceRegex.Replace(s, string.Empty);
        }

        public static string NormaliseNameForMatching(this string s)
        {
            return string.IsNullOrEmpty(s) ? s : NormaliseRegex.Replace(s, string.Empty).ToLowerInvariant();
        }

        public static bool FilterBy(this string s, FilterType filterType, string criteria)
        {
            if (string.IsNullOrEmpty(criteria) || string.IsNullOrEmpty(s))
                return true;

            var contains = s.IndexOf(criteria, StringComparison.OrdinalIgnoreCase) > -1;
            return filterType == FilterType.Exclude ? !contains : contains;
        }

        public static DateTime? ToScheduleTime(this string s, DateTime baseDate)
        {
            var timeString = s.TrimWhitespaceAndNewLines();
            var dt = baseDate.Date;
            if (timeString.StartsWith("1&"))
            {
                timeString = timeString.Substring(2);
                dt = dt.AddDays(1);
            }

            if (timeString.Contains("24:00"))
            {
                timeString = "00:00";
                dt = dt.AddDays(1);
            }

            try
            {
                var additionalTime = TimeSpan.Parse(timeString);
                return dt.Add(additionalTime);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? ToDepartureTime(this string s, DateTime baseDate)
        {
            var timeString = s.TrimWhitespaceAndNewLines();
            var dt = baseDate.Date;
            
            try
            {
                var ticks = long.Parse(timeString);
                var additionalTime = TimeSpan.FromTicks(ticks);
                return dt.Add(additionalTime);
            }
            catch
            {
                return null;
            }
        }

        public static string TrimWhitespaceAndNewLines(this string s)
        {
            return (s ?? "").Trim(WhitespaceAndNewLines);
        }

        public static string TrimToLength(this string s, int maxLength)
        {
            if (string.IsNullOrEmpty(s) || s.Length < maxLength)
                return s;

            return s.Substring(0, maxLength);
        }

        public static string Coalesce(params string[] args)
        {
            return args.FirstOrDefault(x => !string.IsNullOrEmpty(x));
        }

        public static string WrapWith(this string s, string wrapWith)
        {
            var t = s ?? "";
            var a = t.StartsWith(wrapWith) ? "" : wrapWith;
            var b = t.EndsWith(wrapWith) ? "" : wrapWith;

            return string.Concat(a, t, b);
        }

        public static string ToStringFormat(this string template, params object[] args)
        {
            return string.IsNullOrEmpty(template)
                ? string.Empty
                : template.ToStringFormat(CultureInfo.InvariantCulture, args);
        }

        public static string ToStringFormat(this string template, IFormatProvider formatter, params object[] args)
        {
            return string.IsNullOrEmpty(template) ? string.Empty : string.Format(formatter, template, args);
        }

        public static Stream ToStream(this string s)
        {
            return ToStream(s, Encoding.ASCII);
        }

        public static Stream ToStream(this string s, Encoding encoding)
        {
            return new MemoryStream(encoding.GetBytes(s));
        }

        public static object FromXmlString(this string s, Type type)
        {
            if (string.IsNullOrEmpty(s))
                return Activator.CreateInstance(type);

            var serializer = new XmlSerializer(type);
            using (var reader = new StringReader(s))
            {
                return serializer.Deserialize(reader);
            }
        }

        public static T FromXmlString<T>(this string s)
        {
            return (T) FromXmlString(s, typeof (T));
        }

        public static string SafeTrim(this string s)
        {
            return s?.Trim();
        }

        public static string SafeTrimToEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? string.Empty : s.Trim();
        }

        public static bool SafeContains(this string s, string check)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(check))
                return false;

            return s.IndexOf(check, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public static string HashString(string plainText, string salt)
        {
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var plainTextWithSalt = new byte[plainTextBytes.Length + saltBytes.Length];
            plainTextBytes.CopyTo(plainTextWithSalt, 0);
            saltBytes.CopyTo(plainTextWithSalt, plainTextBytes.Length);

            var hashBytes = Hash.ComputeHash(plainTextWithSalt);
            var hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];
            hashBytes.CopyTo(hashWithSaltBytes, 0);
            saltBytes.CopyTo(hashWithSaltBytes, hashBytes.Length);

            return Convert.ToBase64String(hashWithSaltBytes);
        }

        public static string GenerateRandomString(int length, CharGroups allowedChars)
        {
            return GenerateRandomString(length, EnumToAvailableChars(allowedChars));
        }

        public static string GenerateRandomString(int length, IList<char> allowedChars)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var returnChars = new char[length];
            for (var i = 0; i < length; i++)
                returnChars[i] = allowedChars[random.Next(allowedChars.Count - 1)];

            return new string(returnChars);
        }

        public static IList<char> EnumToAvailableChars(CharGroups charGroups)
        {
            var allowedChars = new List<char>();

            if (charGroups.Has(CharGroups.UpperCaseLetters))
                for (var c = 'A'; c <= 'Z'; c++)
                    allowedChars.Add(c);

            if (charGroups.Has(CharGroups.LowerCaseLetters))
                for (var c = 'a'; c <= 'z'; c++)
                    allowedChars.Add(c);

            if (charGroups.Has(CharGroups.Numbers))
                for (var c = '0'; c <= '9'; c++)
                    allowedChars.Add(c);

            if (charGroups.Has(CharGroups.SpecialCharacters))
            {
                for (var c = 33; c <= 47; c++)
                    allowedChars.Add((char) c);
                for (var c = 58; c <= 64; c++)
                    allowedChars.Add((char) c);
                for (var c = 91; c <= 96; c++)
                    allowedChars.Add((char) c);
            }
            return allowedChars;
        }

        public static string RemoveInvalidFilenameCharacters(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
            {
                s = s.Replace(invalidFileNameChar.ToString(), "");
            }

            return s;
        }

        public static string RemoveFilePath(this string s)
        {
            return Path.GetFileName(s);
        }

        public static string[] Split(this string s, string delimiter = ",")
        {
            return (s ?? "").Split(new[] {delimiter}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string ItemAtSplitIndex(this string s, string delimiter, int itemIndex, string defaultValue)
        {
            var arr = s.Split(delimiter);
            return arr.Length > itemIndex ? arr[itemIndex] : defaultValue;
        }

        public static string ExtractPostcode(this string s)
        {
            string test;
            if (string.IsNullOrEmpty(s) || (test = s.ToUpperInvariant()).Length == 0 || !ExtractPostcodeRegex.IsMatch(test))
                return string.Empty;

            return ExtractPostcodeRegex.Match(test).Groups[0].Captures[0].Value;
        }

        public static string ExtractEmail(this string s)
        {
            string test;
            if (string.IsNullOrEmpty(s) || (test = s.ToLowerInvariant()).Length == 0 || !ExtractEmailRegex.IsMatch(test))
                return string.Empty;

            return ExtractEmailRegex.Match(test).Groups[0].Captures[0].Value;
        }

        public static string CleanPhoneNumber(this string s)
        {
            return CleanMobilePhoneNumber(s, null);
        }

        public static string CleanMobilePhoneNumber(this string s)
        {
            return CleanMobilePhoneNumber(s, GetCommonMobilePhoneCountryCodeReplacements());
        }

        public static string CleanMobilePhoneNumber(this string s,
            IDictionary<string, Func<string, string>> prefixCountryCodeReplacements)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            var clean = Regex.Replace(s, @"[^\d]", "");

            if (prefixCountryCodeReplacements != null)
            {
                foreach (var replacement in prefixCountryCodeReplacements)
                {
                    if (clean.StartsWith(replacement.Key))
                        clean = replacement.Value(clean);
                }
            }

            return clean;
        }

        public static IDictionary<string, Func<string, string>> GetCommonMobilePhoneCountryCodeReplacements()
        {
            return new Dictionary<string, Func<string, string>>
            {
                {"07", x => "44" + x.Substring(1)},
                {"440", x => "44" + x.Substring(3)}
            };
        }

        public static string TruncateToStringLengthAttribute<T>(this string s, Expression<Func<T, object>> entityMember)
        {
            var memberInfo = ObjectExtensions.GetMemberInfo(entityMember);
            var lengthAttribute = memberInfo.Member.GetAttribute<StringLengthAttribute>();
            if (lengthAttribute == null)
                return s;

            return s.Length <= lengthAttribute.MaximumLength
                ? s
                : s.Substring(0, lengthAttribute.MaximumLength);
        }

        public static string SubstringToFirstInstance(this string s, string find)
        {
            if (string.IsNullOrWhiteSpace(s))
                return s;

            var index = s.IndexOf(find, StringComparison.Ordinal);
            return (index > -1) ? s.Substring(0, index) : s;
        }

        public static string GenerateSlug(this string phrase, int maxLength = 255)
        {
            if (string.IsNullOrEmpty(phrase))
                return string.Empty;

            var str = phrase.RemoveAccent().ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= maxLength ? str.Length : maxLength).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }

        private static readonly Regex DateReplacementRegex =
            new Regex(@"\{\{Date:(?<DateTimeFormat>[^;}]+)(;(?<AddDays>[^}]+))?}}",
                RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex NameRegex = new Regex(@"\{\{Name}}",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public static string WithFileNameReplacements(this string s, Func<DateTime> date)
        {
            var match = DateReplacementRegex.Match(s);
            var addDays = string.IsNullOrEmpty(match.Groups["AddDays"].Value)
                ? 0
                : int.Parse(match.Groups["AddDays"].Value);

            return string.IsNullOrEmpty(s)
                ? s
                : DateReplacementRegex.Replace(s, m => date().AddDays(addDays).ToString(m.Groups["DateTimeFormat"].Value));
        }

        public static string WithFileNameReplacements(this string s, Func<DateTime> date, Func<string> name)
        {
            return NameRegex.Replace(WithFileNameReplacements(s, date),
                m => name().RemoveInvalidFilenameCharacters());
        }

        public static bool IsMergeableFile(this string s)
        {
            return !string.IsNullOrEmpty(s) && 
                Path.HasExtension(s) &&
                MergeableFileExtensions.Any(x => s.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsImage(this string s)
        {
            return !string.IsNullOrEmpty(s) &&
                Path.HasExtension(s) &&
                ImageFileExtensions.Any(x => s.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        public static string EnsureUniqueFilename(this string s, IEnumerable<string> existingFileNames)
        {
            var copyFileIndex = 0;
            var filename = s.RemoveInvalidFilenameCharacters();
            var testTargetFileName = filename;
            var fileNames = existingFileNames as IList<string> ?? existingFileNames.ToList();
            while (fileNames.Any(x => string.Equals(x, testTargetFileName, StringComparison.OrdinalIgnoreCase)))
            {
                copyFileIndex++;
                testTargetFileName =
                    $"{Path.GetFileNameWithoutExtension(filename)} ({copyFileIndex}){Path.GetExtension(filename)}";
            }

            return testTargetFileName;
        }

        public static string PadTime(this string s)
        {
            s = s ?? "";
            var pos = s.IndexOf(":", StringComparison.Ordinal);

            if (pos == -1)
                return s;

            while (pos < 2)
            {
                s = "0" + s;
                pos = s.IndexOf(":", StringComparison.Ordinal);
            }

            while (s.Length < 5)
                s += "0";

            return s;
        }
    }
}
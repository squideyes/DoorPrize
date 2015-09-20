using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DoorPrize.Generic
{
    public static partial class Extenders
    {
        private static readonly Dictionary<int, Regex> regexes =
            new Dictionary<int, Regex>();

        [DebuggerHidden]
        public static void EnsurePathExists(this string fileName)
        {
            var path = Path.GetDirectoryName(fileName);

            if ((path != null) && (!Directory.Exists(path)))
                Directory.CreateDirectory(path);
        }

        [DebuggerHidden]
        [Pure]
        public static string ToSingleLine(this string value)
        {
            var sb = new StringBuilder();

            var reader = new StringReader(value);

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (sb.Length > 0)
                    sb.Append(' ');

                sb.Append(line.Trim());
            }

            return sb.ToString();
        }

        [DebuggerHidden]
        [Pure]
        public static bool IsMatch(this string value, string pattern)
        {
            return value.IsMatch(pattern, RegexOptions.None);
        }

        [DebuggerHidden]
        [Pure]
        public static bool IsMatch(this string value,
            string pattern, RegexOptions options)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (string.IsNullOrWhiteSpace(pattern))
                return false;

            var hashCode = pattern.GetHashCode();

            Regex regex;

            if (regexes.TryGetValue(hashCode, out regex)) 
                return regex.IsMatch(value);

            options |= RegexOptions.Compiled;

            regex = new Regex(pattern, options);

            regexes.Add(hashCode, regex);

            return regex.IsMatch(value);
        }


        [DebuggerHidden]
        [Pure]
        public static bool IsTrimmed(this string value, bool emptyOk = false)
        {
            if (value == (string)null)
                return false;

            if (value == string.Empty)
                return emptyOk;

            return (!char.IsWhiteSpace(value[0]) &&
                (!char.IsWhiteSpace(value[value.Length - 1])));
        }

        [DebuggerHidden]
        [Pure]
        public static bool IsFileName(this string value, bool mustBeRooted = true)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(value));

            try
            {
                var dummy = new FileInfo(value);

                return !mustBeRooted || Path.IsPathRooted(value);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (PathTooLongException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }

        [DebuggerHidden]
        [Pure]
        public static bool IsDirectory(this string value, bool mustBeRooted = true)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(value));

            try
            {
                var dummy = new DirectoryInfo(value);

                return !mustBeRooted || Path.IsPathRooted(value);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (PathTooLongException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }
    }
}
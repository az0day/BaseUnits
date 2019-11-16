using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;

namespace BaseUnits.Core.Helpers
{
    public static class StringExtensions
    {
        public static string ToUrlSlug(this string value)
        {
            // First to lower case 
            value = value.ToLowerInvariant();

            // Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);

            value = Encoding.UTF8.GetString(bytes);

            // Replace spaces 
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            // Remove invalid chars 
            value = Regex.Replace(value, @"[^\w\s\p{Pd}]", "", RegexOptions.Compiled);

            // Trim dashes from end 
            value = value.Trim('-', '_');

            // Replace double occurences of - or \_ 
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/25259/how-does-stack-overflow-generate-its-seo-friendly-urls
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string ToUrlFriendly(this string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return "";
            }

            const int MAXLEN = 80;
            var len = title.Length;
            var prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (var i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if (c >= 128)
                {
                    var prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == MAXLEN) break;
            }

            if (prevdash)
            {
                return sb.ToString().Substring(0, sb.Length - 1);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate Slug
        /// </summary>
        /// <param name="slugs">[slug, tag]</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GenerateSlug(this IDictionary<string, string> slugs, string name)
        {
            var ori = name.ToUrlSlug();
            var slug = ori;
            var index = 1;
            while (slugs.ContainsKey(slug))
            {
                slug = string.Format("{0}-{1}", ori, index);
                index++;
            }

            return slug;
        }

        private static string RemapInternationalCharToAscii(char c)
        {
            var s = c.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }

            if ("èéêëę".Contains(s))
            {
                return "e";
            }

            if ("ìíîïı".Contains(s))
            {
                return "i";
            }

            if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }

            if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }

            if ("çćčĉ".Contains(s))
            {
                return "c";
            }

            if ("żźž".Contains(s))
            {
                return "z";
            }

            if ("śşšŝ".Contains(s))
            {
                return "s";
            }

            if ("ñń".Contains(s))
            {
                return "n";
            }

            if ("ýÿ".Contains(s))
            {
                return "y";
            }

            if ("ğĝ".Contains(s))
            {
                return "g";
            }

            if (c == 'ř')
            {
                return "r";
            }

            if (c == 'ł')
            {
                return "l";
            }

            if (c == 'đ')
            {
                return "d";
            }

            if (c == 'ß')
            {
                return "ss";
            }

            if (c == 'Þ')
            {
                return "th";
            }

            if (c == 'ĥ')
            {
                return "h";
            }

            if (c == 'ĵ')
            {
                return "j";
            }

            return "";
        }
    }
}

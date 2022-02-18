using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CSFileCabinetRenamer
{
    internal class TextConveter
    {
        static readonly Regex regexCombineChars = new Regex(@"[\u0300-\u036F\u1AB0-\u1AFF\u1DC0-\u1DFF\u20D0-\u20FF\uFE20-\uFE2F]+");

        /// <summary>
        /// Replace the combining characters in a string.
        /// </summary>
        /// <param name="text">Any text string</param>
        /// <returns>text string without combining characters</returns>
        public static string RemoveCombiningCharacters(string text)
        {
            string removedCombineChars = regexCombineChars.Replace(text, "");
            return removedCombineChars;
        }

        /// <summary>
        /// Gives the string representation of a unicode character.
        /// </summary>
        /// <param name="c">Single unicode character</param>
        /// <returns></returns>
        public static string CharToUnicodeFormat(char c)
        {
            return string.Format(@"U+{0:x4}", (int)c);
        }
        /// <summary>
        /// Gives the string representation of many unicode characters.
        /// </summary>
        /// <param name="s">Unicode string</param>
        /// <returns></returns>
        public static string CharToUnicodeFormat(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                sb.Append("\\u");
                sb.Append(String.Format("{0:x4}", (int)c));
            }
            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CSFileCabinetRenamer.Language;
using CSFileCabinetRenamer.TextInfo;

namespace CSFileCabinetRenamer
{
    //internal class TextConveter
    //{
    //    static readonly Regex regexCombineChars = new Regex(@"[\u0300-\u036F\u1AB0-\u1AFF\u1DC0-\u1DFF\u20D0-\u20FF\uFE20-\uFE2F]+");

    //    /// <summary>
    //    /// Replace the combining characters in a string.
    //    /// </summary>
    //    /// <param name="text">Any text string</param>
    //    /// <returns>text string without combining characters</returns>
    //    public static string RemoveCombiningCharacters(string text)
    //    {
    //        string removedCombineChars = regexCombineChars.Replace(text, "");
    //        return removedCombineChars;
    //    }

    //    /// <summary>
    //    /// Gives the string representation of a unicode character.
    //    /// </summary>
    //    /// <param name="c">Single unicode character</param>
    //    /// <returns>String in U+XXXX format for 1 character</returns>
    //    public static string CharToUnicodeFormat(char c)
    //    {
    //        return string.Format(@"U+{0:x4}", (int)c);
    //    }
    //    /// <summary>
    //    /// Gives the string representation of many unicode characters.
    //    /// </summary>
    //    /// <param name="s">Unicode string</param>
    //    /// <returns>String in \uXXXX for many characters</returns>
    //    public static string CharToUnicodeFormat(string s)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        foreach (char c in s)
    //        {
    //            sb.Append("\\u");
    //            sb.Append(String.Format("{0:x4}", (int)c));
    //        }
    //        return sb.ToString();
    //    }
    //}

    public class FileRenamer
    {
        bool RemoveCombiningChars { get; set; }
        bool RemoveExtendedUnicode { get; set; }
        bool RemoveBeyondUnicode { get; set; }
        bool ConvertChars { get; set; }

        // This should be the same as the regex in TextInformation
        static readonly Regex regexCombineChars = new Regex(@"[\u0300-\u036F\u1AB0-\u1AFF\u1DC0-\u1DFF\u20D0-\u20FF\uFE20-\uFE2F]+");

        public FileRenamer (bool removeCombineChars=true, bool removeExtended=false, bool removeBeyond=false, bool convertChars=false)
        {
            RemoveCombiningChars = removeCombineChars;
            RemoveExtendedUnicode = removeExtended;
            RemoveBeyondUnicode = removeBeyond;
            ConvertChars = convertChars;
        }

        ///// <summary>
        ///// Rename the given string following the rules from class construction.
        ///// 
        ///// Combining characters may be removed.
        ///// Convertable characters will be replaced.
        ///// Extended and Beyond characters may be removed.
        ///// </summary>
        ///// <param name="text">original string to rename</param>
        ///// <param name="hasCombineChars">Does the string have combining unicode characters?</param>
        ///// <param name="hasConvertableChars">Does the string have Characters to convert to the 1st 255 characters?</param>
        ///// <param name="hasExtended">Does the string have extended unicode characters to delete?</param>
        ///// <param name="HasBeyond">Does the string have unicode characters >0xFFFF to delete?</param>
        ///// <returns>The Renamed string</returns>
        //public string RenameString(string text, bool hasCombineChars, bool hasConvertableChars, bool hasExtended, bool HasBeyond)
        //{
        //    if (text == null) return "";

        //    string final_string = text;
        //    if (hasConvertableChars || ConvertChars) { final_string = convertChars(final_string); }
        //    if (hasCombineChars || RemoveCombiningChars) { final_string = removeCombineChars(final_string); }

        //    // remove beyond first because we wont to get rid of the whole element, not its surrogate pairs
        //    if (HasBeyond || RemoveBeyondUnicode) { final_string = removeBeyond(final_string); }
        //    if (hasExtended || RemoveExtendedUnicode) { final_string = removeExtended(final_string); }

        //    return final_string;
        //}

        /// <summary>
        /// Renames the string from a list of TextElements
        /// </summary>
        /// <param name="textElements">List of index, textElements</param>
        /// <returns>A single string</returns>
        public string RenameFromTextElements(List<(int, TextElement)> textElements)
        {
            var stringBuilder = new StringBuilder();
            foreach ((int index, TextElement textElement) in textElements)
            {
                string character = textElement.ToString();
                if (textElement.HasCombiningChar && RemoveCombiningChars) { character = removeCombineChars(character); }
                if (textElement.HasConvertableChar && ConvertChars) { character = convertChar(character); }

                // Remove the text element if it has the below unicode and user wants to remove it.
                if (textElement.HasBeyondUnicode && RemoveBeyondUnicode) { character = ""; }
                if (textElement.HasExtendedUnicode && RemoveExtendedUnicode) { character = ""; }
                stringBuilder.Append(character);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Replaces the character if it is definined in the AllAvaliableLanguages dictionary
        /// </summary>
        /// <param name="text">the string element to be replaced</param>
        /// <returns>A new string with replaced value</returns>
        private static string convertChar(string text)
        {
            foreach (var dictionary in LanguageDictionary.AllAvaliableLanguages)
            {
                // if the key is found return the new string
                // convert the text element to a char first
                if (dictionary.TryGetValue(text[0], out string? value)) { return value; }
            }
            // if this failed (even though it should not) return the original
            return text;
            
        }

        /// <summary>
        /// Removes standard combining unicode characters from a string.
        /// Does not remove language specific combining characters.
        /// </summary>
        /// <param name="text">Text with combining characters.</param>
        /// <returns>New String with combining characters removed.</returns>
        private static string removeCombineChars(string text)
        {
            string removedCombineChars = regexCombineChars.Replace(text, "");
            return removedCombineChars;
        }
    }
}

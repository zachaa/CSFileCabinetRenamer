using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using CSFileCabinetRenamer.Language;
using CSFileCabinetRenamer.TextInfo;

namespace CSFileCabinetRenamer
{
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
                // Needs to be else if because convertable characters are in extended unicode and
                //  we don't want to delete them after they have become valid.
                else if (textElement.HasBeyondUnicode && RemoveBeyondUnicode) { character = ""; }
                else if (textElement.HasExtendedUnicode && RemoveExtendedUnicode) { character = ""; }
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
                // If the key is found return the new string.
                // Must convert the text element to a char first.
                if (dictionary.TryGetValue(text[0], out string? value))
                {
                    return value;
                }
            }
            // If this failed (even though it should not) return the original.
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

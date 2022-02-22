using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CSFileCabinetRenamer.Language;


namespace CSFileCabinetRenamer.TextInfo
{
    /// <summary>
    /// A Class to hold information about a text element (grapheme cluster)
    /// from TextElementEnumerator or what appears to be a single character.
    /// </summary>
    public class TextElement
    {
        private string _Element { get; set; }
        public List<int> CodePoints { get; }
        public string U32Code { get; }
        public bool HasCombiningChar { get; }
        public bool HasConvertableChar { get; }
        public bool HasExtendedUnicode { get; }
        public bool HasBeyondUnicode { get; }

        public TextElement(string element)
        {
            _Element = element;
            CodePoints = ToCodePoints(_Element);
            U32Code = _ToHexCode();
            HasCombiningChar = _HasCombiningCharacters(_Element);
            HasConvertableChar = _HasConvertableCharacters(_Element);
            HasExtendedUnicode = _HasExtendedUnicodeChars(_Element);
            HasBeyondUnicode = _HasBeyondUnicodeChars();
        }

        // Only the standard combining characters for latin and phonetical alphabets
        static readonly Regex regexCombineChars = new Regex(@"[\u0300-\u036F\u1AB0-\u1AFF\u1DC0-\u1DFF\u20D0-\u20FF\uFE20-\uFE2F]+");

        // All characters after Latin-1 Supplement (U+00FF) in [\u0100-\uFFFF] (Excluding standard combining characters)
        static readonly Regex regexNonStandard = new Regex(@"[\u0100-\u02FF\u0370-\u1AAF\u1B00-\u1DBF\u1E00-\u20CF\u2100-\uFE1F\uFE30-\uFFFF]+");

        private static bool _HasCombiningCharacters(string text)
        {
            // Could probably do this with CodePoints.Count > 1 as there should only be 1 element in this class
            MatchCollection combiningChars = regexCombineChars.Matches(text);
            if (combiningChars.Count > 0) { return true; }
            return false;
        }

        private bool _HasConvertableCharacters(string text)
        {
            MatchCollection convertableChars = LanguageDictionary.MappingRegex.Matches(text);
            if (convertableChars.Count > 0) { return true; }
            return false;
        }

        private static bool _HasExtendedUnicodeChars(string text)
        {
            MatchCollection extendedChars = regexNonStandard.Matches(text);
            if (extendedChars.Count > 0) { return true; }
            return false;
        }

        private bool _HasBeyondUnicodeChars()
        {
            foreach (int codepoint in CodePoints)
            {
                if (codepoint > Int16.MaxValue) { return true; }
            }
            return false;
        }

        public override string ToString()
        {
            return _Element.ToString();
        }

        /// <summary>Gives the element string, hex code, and bools about the element</summary>
        public string ToDetailedString()
        {
            return $"{_Element}: {U32Code}\t\t CC:{HasCombiningChar} ConvC:{HasConvertableChar}\t EU:{HasExtendedUnicode} BU:{HasBeyondUnicode}";
        }

        /// <summary>
        /// Gives a list of integers for the different parts of the given element. Use U32Code to see the hex values
        /// </summary>
        /// <param name="str">Text Element string</param>
        /// <returns>List of int32</returns>
        private static List<int> ToCodePoints(string text)
        {
            var codePoints = new List<int>(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                codePoints.Add(Char.ConvertToUtf32(text, i));
                if (Char.IsHighSurrogate(text[i]))
                    i += 1;  // skips low surrogate in surrogate pair
            }
            return codePoints;
        }

        private string _ToHexCode()
        {
            List<string> HexList = new();
            foreach (int utf32 in CodePoints)
            {
                HexList.Add(Convert.ToString(utf32, 16));
            }
            return String.Join(", ", HexList);
        }

        /// <summary>
        /// Give the number of utf16 chars in the element.
        /// </summary>
        public int NumberOfChars() { return _Element.Length; }

        /// <summary>
        /// Gives the number of Runes in the element.
        /// </summary>
        public int NumberOfRunes() { return _Element.EnumerateRunes().Count(); }

        /// <summary>
        /// Is the text element an ASCII character?
        /// </summary>
        public bool IsAscii() { return CodePoints.Count == 1 && CodePoints[0] < 128; }
    }

    /// <summary>
    /// Class that holds the list of text elements for the given string
    /// </summary>
    public class TextElementsHolder
    {
        public string BaseText { get; }
        public List<(int, TextElement)> TextElements { get; }
        public bool HasCombiningCharacters { get; }
        public bool HasConvertableCharacters { get; }
        public bool HasExtendedUnicode { get; }
        public bool HasBeyondUnicode { get; }

        public TextElementsHolder(string text)
        {
            BaseText = text;
            TextElements = new List<(int, TextElement)>();
            HasCombiningCharacters = false;
            HasConvertableCharacters = false;
            HasExtendedUnicode = false;
            HasBeyondUnicode = false;

            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(BaseText);
            while (enumerator.MoveNext())
            {
                // Add index and TextElement item to list in order of thext elements
                int index = enumerator.ElementIndex;
                var tel = new TextElement(enumerator.GetTextElement());
                // set the bools to true if any text element fits
                if (tel.HasCombiningChar) { HasCombiningCharacters = true; }
                if (tel.HasConvertableChar) { HasConvertableCharacters = true; }
                if (tel.HasExtendedUnicode) { HasExtendedUnicode = true; }
                if (tel.HasBeyondUnicode) { HasBeyondUnicode = true; }
                TextElements.Add((index, tel));
            }
        }

        public bool IsClean()
        {
            return (!HasCombiningCharacters && !HasExtendedUnicode && !HasBeyondUnicode);
        }

        public override string ToString()
        {
            return $"{BaseText}: CC:{HasCombiningCharacters}, ConvC:{HasConvertableCharacters}, EU:{HasExtendedUnicode}, BU:{HasBeyondUnicode}";
        }
    }
}
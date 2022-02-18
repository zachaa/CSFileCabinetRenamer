using System.Globalization;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleTestUnicodeStrings
{
    internal class ClassTestStrings
    {
        string emoji1 = "👩🏽‍🚒"; // = 👩🏽‍🚒  and emoji with combining characters
        /*U+1F469 WOMAN(supplementary range, requires a surrogate pair)
          U+1F3FD EMOJI MODIFIER FITZPATRICK TYPE-4 (supplementary range, requires a surrogate pair)
          U+200D ZERO WIDTH JOINER
          U+1F692 FIRE ENGINE*/
        string combineChar = @"á";
        string singleChar = @"á";
        string greaterThanFFFF = @"🜀";  // alchemical symbol 🜀 = U+1F700
        //string greaterThanFFFF_u = "\u1F700";  // same as above

        //public string combinedItems = @"aáá🜀💩";

        public static void PrintTextElementCount(string s)
        {
            Console.WriteLine(s);
            Console.WriteLine($"Number of chars: {s.Length}");
            Console.WriteLine($"Number of runes: {s.EnumerateRunes().Count()}");

            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(s);

            int textElementCount = 0;
            while (enumerator.MoveNext())
            {
                textElementCount++;
            }

            Console.WriteLine($"Number of text elements: {textElementCount}");
        }

        public static void PrintElements(string s)
        {
            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(s);
            while (enumerator.MoveNext())
            {
                int a = enumerator.ElementIndex;
                object b = enumerator.Current;  // what kind of object?? seems to be a string
                string c = enumerator.GetTextElement();
                Debug.WriteLine($"{a}, {b}({b.GetType()}), {c}");
            }
        }

        // Check if element has normal combining chars
        // Check if element is beyond U+FFFF
        // Check if element is in extended unicode U+0100 -> U+FFFF
    }

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
        public bool HasExtendedUnicode { get; }
        public bool HasBeyondUnicode { get; }

        public TextElement(string element)
        {
            _Element = element;
            CodePoints = ToCodePoints(_Element);
            U32Code = _ToHexCode();
            HasCombiningChar = _HasCombiningCharacters(_Element);
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
        public bool HasExtendedUnicode { get; }
        public bool HasBeyondUnicode { get; }

        public TextElementsHolder(string text)
        {
            BaseText = text;
            TextElements = new List<(int, TextElement)>();
            HasCombiningCharacters = false;
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
            return $"{BaseText}: CC:{HasCombiningCharacters}, EU:{HasExtendedUnicode}, BU:{HasBeyondUnicode}";
        }
    }

    internal class TextConveter
    {
        // Only the standard combining characters for latin and phonetical alphabets
        static readonly Regex regexCombineChars = new Regex(@"[\u0300-\u036F\u1AB0-\u1AFF\u1DC0-\u1DFF\u20D0-\u20FF\uFE20-\uFE2F]+");

        // All characters after Latin-1 Supplement (U+00FF) in [\u0100-\uFFFF] (Excluding standard combining characters)
        static readonly Regex regexNonStandard = new Regex(@"[\u0100-\u02FF\u0370-\u1AAF\u1B00-\u1DBF\u1E00-\u20CF\u2100-\uFE1F\uFE30-\uFFFF]+");

        // Beyond normal unicode (Not in Basic Multilingual Plane such as: 🜊 🜀 😀)
        // TOO BIG C# can't handle this
        //static readonly Regex regexBeyond = new Regex(@"[\u00010000-\u0010FFFF]+");
        static readonly Regex regexBeyond = new Regex(@"[Xx]+");

        /// <summary>
        /// Checks if the given text has any unicode combining characters.
        /// 
        /// See: https://en.wikipedia.org/wiki/Combining_character
        /// 
        /// Does not check for any language specific combining characters.
        /// </summary>
        /// <param name="text">Any text string</param>
        /// <returns>true if there are combining characters</returns>
        public static bool HasCombiningCharacters(string text)
        {
            MatchCollection combiningChars = regexCombineChars.Matches(text);
            if (combiningChars.Count > 0) { return true; }
            return false;
        }

        public static bool HasExtendedUnicodeChars(string text)
        {
            MatchCollection extendedChars = regexNonStandard.Matches(text);
            if (extendedChars.Count > 0) { return true; }
            return false;
        }

        public static bool HasBeyondUnicodeChars(string text)
        {
            MatchCollection beyondUnicodeChars = regexBeyond.Matches(text);
            if (beyondUnicodeChars.Count > 0) { return true; }
            return false;
        }

        /// <summary>
        /// Replace the combining characters in a string.
        /// </summary>
        /// <param name="text">Any text string</param>
        /// <returns>text string without combining characters</returns>
        public static string RemoveCombiningCharacters(string text)
        {
            string combiningChars = regexCombineChars.Replace(text, "");
            return combiningChars;
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

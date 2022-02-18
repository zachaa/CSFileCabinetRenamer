using System.Globalization;
using System.Diagnostics;
using ConsoleTestUnicodeStrings;

// See https://aka.ms/new-console-template for more information
Debug.WriteLine("Testing Unicode characters");
string greaterThanFFFF = @"🜀";
string combinedItems = @"aáá🜀💩8";
Debug.WriteLine(greaterThanFFFF);
ClassTestStrings.PrintElements(greaterThanFFFF);
Debug.WriteLine(combinedItems);
ClassTestStrings.PrintElements(combinedItems);
Debug.WriteLine(combinedItems);
var tel0 = new TextElement("a");
var tel1 = new TextElement("á");  // single char
var tel2 = new TextElement("á");  // combined
var tel3 = new TextElement("🜀");  // beyond FFFF
var tel4 = new TextElement("👩🏽‍🚒"); // 👩🏽‍🚒 is redhead firefighter woman with 4 codes
Debug.WriteLine($"a: {tel0.HasCombiningChar} {tel0.HasExtendedUnicode} {tel0.HasBeyondUnicode} {tel0.U32Code}");
Debug.WriteLine($"á: {tel1.HasCombiningChar} {tel1.HasExtendedUnicode} {tel1.HasBeyondUnicode} {tel1.U32Code}");
Debug.WriteLine($"á: {tel2.HasCombiningChar} {tel2.HasExtendedUnicode} {tel2.HasBeyondUnicode} {tel2.U32Code}");
Debug.WriteLine($"🜀: {tel3.HasCombiningChar} {tel3.HasExtendedUnicode} {tel3.HasBeyondUnicode} {tel3.U32Code}");
Debug.WriteLine($"👩🏽‍🚒: {tel4.HasCombiningChar} {tel4.HasExtendedUnicode} {tel4.HasBeyondUnicode} {tel4.U32Code}");
var teh1 = new TextElementsHolder("aá");
var teh2 = new TextElementsHolder("Quáca");
Debug.WriteLine(teh1.ToString());
Debug.WriteLine(teh2.ToString());

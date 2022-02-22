using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using CSFileCabinetRenamer.TextInfo;
//using System.Diagnostics;

namespace CSFileCabinetRenamer
{
    public class File
    {
        public string FileName { get; }
        public string FileExtension { get; }
        public ImageSource? FileExtImage { get; }
        public string FilePath { get; }

        public bool IsCleanName { get; }
        public Brush BackgroundBrush { get; set; }

        /// <summary> Highlighted Text to put in the GridView </summary>
        public List<Inline> FileNameInlineList { get; set; }


        private TextElementsHolder textElementsHolder { get; }
        private bool _HasCombiningChar { get; set; }
        private bool _HasConvertableChar { get; set; }
        private bool _HasExtendedUnicode { get; set; }
        private bool _HasBeyondUnicode { get; set; }

        // Brushes for IsClean Column
        public readonly LinearGradientBrush _brushGood = new(Color.FromArgb(255, 50, 240, 80), Color.FromArgb(40, 50, 240, 80), 0.0);
        public readonly LinearGradientBrush _brushFix = new(Color.FromArgb(255, 250, 180, 10), Color.FromArgb(40, 250, 180, 10), 0.0);
        public readonly LinearGradientBrush _brushBad = new(Color.FromArgb(255, 240, 50, 50), Color.FromArgb(40, 240, 50, 50), 0.0);
        public readonly LinearGradientBrush _brushUnknown = new(Color.FromArgb(255, 100, 100, 100), Color.FromArgb(40, 100, 100, 100), 0.0);

        // Brushes for coloring parts of text to show problems
        public readonly SolidColorBrush _bConvertChar = new(Color.FromArgb(150, 50, 200, 230));
        public readonly SolidColorBrush _bCombineChar = new(Color.FromArgb(150, 240, 200, 10));
        public readonly SolidColorBrush _bExtendedUni = new(Color.FromArgb(150, 220, 40, 40));
        public readonly SolidColorBrush _bBeyondUni = new(Color.FromArgb(150, 180, 40, 220));

        public File(string file_path)
        {
            FileName = Path.GetFileNameWithoutExtension(file_path);
            FileExtension = Path.GetExtension(file_path);
            FileExtImage = ToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(file_path));
            FilePath = file_path;

            textElementsHolder = new TextElementsHolder(FileName);
            _HasCombiningChar = textElementsHolder.HasCombiningCharacters;
            _HasConvertableChar = textElementsHolder.HasConvertableCharacters;
            _HasExtendedUnicode = textElementsHolder.HasExtendedUnicode;
            _HasBeyondUnicode = textElementsHolder.HasBeyondUnicode;
            IsCleanName = textElementsHolder.IsClean();

            BackgroundBrush = BackgroundBrushFromContent();
            FileNameInlineList = CreateInlineList();
        }

        private List<Inline> CreateInlineList()
        {
            var col = new List<Inline>();

            // Most file name should end be here
            if (IsCleanName)
            {
                col.Add(new Run(FileName));
                return col;
            }

            foreach ((int index, TextInfo.TextElement tel) in textElementsHolder.TextElements)
            {
                var r = new Run(tel.ToString());
                // Color the background depending on the type of TextElement
                if (tel.HasBeyondUnicode) { r.Background = _bBeyondUni; }
                else if (tel.HasConvertableChar) { r.Background = _bConvertChar; }
                else if (tel.HasExtendedUnicode) { r.Background = _bExtendedUni; }
                else if (tel.HasCombiningChar) { r.Background = _bCombineChar; }

                // Add the element to the run to reform the full file name
                col.Add(r);
            }
            return col;
        }

        private Brush BackgroundBrushFromContent()
        {
            // Color the IsClean background depending on if it can be fixed or not
            if (IsCleanName) { return _brushGood; }
            else if (_HasExtendedUnicode || _HasBeyondUnicode) { return _brushBad; }
            else if (_HasCombiningChar || _HasConvertableChar) { return _brushFix; }
            else { return _brushUnknown; }
        }

        public void ChangeFileName(FileRenamer renamer)
        {
            // Can't change Extended or Beyond Unicode files, user will have to do that manually
            //if (!IsCleanName && !_HasBeyondUnicode && !_HasExtendedUnicode)
            //{
            //    if (System.IO.File.Exists(FilePath))
            //    {
            //        string new_name = TextConveter.RemoveCombiningCharacters(FileName);
            //        //string new_name_convert = TextConverter.ConvertCharacters(FileName);
            //        string new_path = (Path.GetDirectoryName(FilePath) + Path.DirectorySeparatorChar + new_name + FileExtension);
            //        //Trace.WriteLine(new_path);
            //        System.IO.File.Move(FilePath, new_path);
            //    }
            //}

            if (!IsCleanName)
            {
                string new_name = renamer.RenameFromTextElements(textElementsHolder.TextElements);
                string new_path = (Path.GetDirectoryName(FilePath) + Path.DirectorySeparatorChar + new_name + FileExtension);
                System.IO.File.Move(FilePath, new_path);
            }
        }

        /// <summary>
        /// Used to convert an icon from windows to a aceptable image source for WPF 'Image'.
        /// </summary>
        public static ImageSource? ToImageSource(System.Drawing.Icon? icon)
        {
            // catch null errors
            if (icon == null) { return null; }

            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}

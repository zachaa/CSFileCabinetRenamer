using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using CSFileCabinetRenamer.TextInfo;
using System.Collections.Generic;

namespace CSFileCabinetRenamer
{
    public class File
    {
        public string FileName { get; }
        public string FileExtension { get; }
        public string FilePath { get; }

        public bool IsCleanName { get; }
        public Brush BackgroundBrush { get; set; }
        /// <summary> Highlighted Text to put in the GridView </summary>
        // public FlowDocument FileNameFlowDocument { get; set; }
        public List<Inline> FileNameInlineList { get; set; }

        private TextElementsHolder textElementsHolder {get;}
        private bool _HasCombiningChar { get; set; }
        private bool _HasExtendedUnicode { get; set; }
        private bool _HasBeyondUnicode { get; set; }

        // Brushes for IsClean Column
        static readonly LinearGradientBrush _brushGood = new(Color.FromArgb(255, 50, 240, 80), Color.FromArgb(40, 50, 240, 80), 0.0);
        static readonly LinearGradientBrush _brushFix = new(Color.FromArgb(255, 250, 180, 10), Color.FromArgb(40, 250, 180, 10), 0.0);
        static readonly LinearGradientBrush _brushBad = new(Color.FromArgb(255, 240, 50, 50), Color.FromArgb(40, 240, 50, 50), 0.0);
        static readonly LinearGradientBrush _brushUnknown = new(Color.FromArgb(255, 100, 100, 100), Color.FromArgb(40, 100, 100, 100), 0.0);

        // Brushes for coloring parts of text to show problems
        static SolidColorBrush _bCombineChar = new(Color.FromArgb(150, 220, 180, 10));
        static SolidColorBrush _bExtendedUni = new(Color.FromArgb(150, 220, 40, 40));
        static SolidColorBrush _bBeyondUni = new(Color.FromArgb(150, 180, 40, 220));

        public File(string file_path)
        {
            FileName = Path.GetFileNameWithoutExtension(file_path);
            FileExtension = Path.GetExtension(file_path);
            FilePath = file_path;

            textElementsHolder = new TextElementsHolder(FileName);
            _HasCombiningChar = textElementsHolder.HasCombiningCharacters;
            _HasExtendedUnicode = textElementsHolder.HasExtendedUnicode;
            _HasBeyondUnicode = textElementsHolder.HasBeyondUnicode;
            IsCleanName = textElementsHolder.IsClean();

            BackgroundBrush = BackgroundBrushFromContent();
            // FileNameFlowDocument = CreateFlowDoc();
            FileNameInlineList = CreateInlineList();

        }

        /*private FlowDocument CreateFlowDoc()
        {
            var doc = new FlowDocument();

            // Most file name should end be here
            if (IsCleanName) 
            { 
                doc.Blocks.Add(new Paragraph(new Run(FileName)));
                return doc;
            }

            var paragraph = new Paragraph();
            foreach ((int index, TextInfo.TextElement tel) in textElementsHolder.TextElements)
            {
                var r = new Run(tel.ToString());

                // Color the background depending on the type of TextElement
                if (tel.HasBeyondUnicode) { r.Background = _bBeyondUni; }
                else if (tel.HasExtendedUnicode) { r.Background = _bExtendedUni; }
                else if (tel.HasCombiningChar) { r.Background = _bCombineChar; }
                
                // Add the element to the run to reform the full file name
                paragraph.Inlines.Add(r);
            }
            doc.Blocks.Add(paragraph);
            return doc;
        }*/

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
            else if (_HasCombiningChar) { return _brushFix; }
            else { return _brushUnknown; }
        }

        public void ChangeFileName()
        {
            // Can't change Extended or Beyond Unicode files, user will have to do that manually
            if (!IsCleanName && !_HasBeyondUnicode && !_HasExtendedUnicode)
            {
                if (System.IO.File.Exists(FilePath))
                {
                    string new_name = TextConveter.RemoveCombiningCharacters(FileName);
                    string new_path = (Path.GetDirectoryName(FilePath) + Path.DirectorySeparatorChar + new_name + FileExtension);
                    //Trace.WriteLine(new_path);
                    System.IO.File.Move(FilePath, new_path);
                }
            }
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}

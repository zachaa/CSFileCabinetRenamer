using System.Diagnostics;
using System.IO;
using System.Windows.Media;

namespace CSFileCabinetRenamer
{
    public class File
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public bool IsCleanName { get; set; }
        public Brush IsCleanBackground { get; set; }

        static readonly LinearGradientBrush _brushGood = new(Color.FromArgb(255, 50, 240, 80), Color.FromArgb(40, 50, 240, 80), 0.0);
        static readonly LinearGradientBrush _brushBad = new(Color.FromArgb(255, 240, 50, 50), Color.FromArgb(40, 240, 50, 50), 0.0);

        public File(string file_path)
        {
            FileName = Path.GetFileNameWithoutExtension(file_path);
            FileExtension = Path.GetExtension(file_path);
            FilePath = file_path;
            IsCleanName = IsNameClean();
            IsCleanBackground = BackgroundBrush();

        }

        private bool IsNameClean()
        {
            return !TextConveter.HasCombiningCharacters(FileName);
        }

        private Brush BackgroundBrush()
        {
            return IsCleanName ? _brushGood : _brushBad ;
        }

        public void ChangeFileName()
        {
            if (!IsCleanName)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;


// Does not do what I want ?
namespace CSFileCabinetRenamer.Models
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FileViewModel : ViewModelBase
    {
        File _file;

        public FileViewModel(File file) { _file = file; }
        public FileViewModel(FileViewModel file_view_model) { _file = new File(file_view_model.FilePath); }
        public FileViewModel(string file_path) { _file = new File(file_path); }

        public string FileName
        {
            get { return _file.FileName; }
        }
        public string FilePath
        {
            get { return _file.FilePath; }
            set
            {
                _file.FilePath = value;
                NotifyPropertyChanged("FilePath");
            }
        }
        public bool IsCleanName
        {
            get { return _file.IsCleanName; }
        }
    }

    public class FileListViewModel : ViewModelBase
    {
        private FileViewModel _fileViewModel;
        private ObservableCollection<FileViewModel> _files = new ObservableCollection<FileViewModel>();

        public FileViewModel FileViewModel
        {
            get { return _fileViewModel; }
            set
            {
                _fileViewModel = value;
                NotifyPropertyChanged("FileViewModel");
            }
        }

        public ObservableCollection<FileViewModel> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                NotifyPropertyChanged("Files");
            }
        }

        public FileListViewModel()
        {
            _fileViewModel = new FileViewModel("");
            _files = new ObservableCollection<FileViewModel>();
        }

    }
}

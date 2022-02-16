using System;
using System.Windows;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace CSFileCabinetRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<File> filesInFolder = new ObservableCollection<File>();
        public MainWindow()
        {
            InitializeComponent();
            listview.ItemsSource = filesInFolder;
        }

        private void button_rename_files_Click(object sender, RoutedEventArgs e)
        {
            foreach (File file_item in filesInFolder)
            {
                try
                {
                    file_item.ChangeFileName();
                }
                catch (System.IO.IOException)
                {
                    // If there is an error, warn the user of file, then continue.
                    string message = String.Format("Unable to rename \"{0}{1}\" due to another file having the new name." +
                        " Try renaming manally to something else",
                         file_item.FileName, file_item.FileExtension);
                    MessageBox.Show(message, "Rename error", MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }
            }
            filesInFolder.Clear();
        }

        private void button_open_files_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                filesInFolder.Clear();
                foreach (string file_path in openFileDialog.FileNames)
                {
                    filesInFolder.Add(new File(file_path));
                }  
            }
        }

    }
}

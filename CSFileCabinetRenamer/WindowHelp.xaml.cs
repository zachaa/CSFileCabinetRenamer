using System.Windows;

namespace CSFileCabinetRenamer
{
    /// <summary>
    /// Interaction logic for WindowHelp.xaml
    /// </summary>
    public partial class WindowHelp : Window
    {
        public WindowHelp()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e) =>
            DialogResult = true;
    }
}

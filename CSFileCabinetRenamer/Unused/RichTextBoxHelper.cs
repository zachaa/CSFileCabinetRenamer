using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

// from: https://stackoverflow.com/questions/343468/richtextbox-wpf-binding

namespace CSFileCabinetRenamer
{
    public class RichTextBoxHelper : DependencyObject
    {
        public static string GetDocumentXaml(DependencyObject obj) { return (string)obj.GetValue(DocumentXamlProperty); }

        public static void SetDocumentXaml(DependencyObject obj,
                                           string value)
        {
            obj.SetValue(DocumentXamlProperty, value);
        }

        public static readonly DependencyProperty DocumentXamlProperty = DependencyProperty.RegisterAttached
        (
            "DocumentXaml",
            typeof(string),
            typeof(RichTextBoxHelper),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                PropertyChangedCallback = (obj,
                                           e) =>
                {
                    var richTextBox = (RichTextBox)obj;
                    var xaml = GetDocumentXaml(richTextBox);
                    Stream sm = new MemoryStream(Encoding.UTF8.GetBytes(xaml));
                    richTextBox.Document = (FlowDocument)XamlReader.Load(sm);
                    sm.Close();
                }
            }
        );
    }
}
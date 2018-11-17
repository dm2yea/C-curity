using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace StudyBuddyApp
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void NewModule(object sender, RoutedEventArgs e)
        {
            // Navigate to the page, using the NavigationService
            this.NavigationService.Navigate(new EditMode());

            //once we enter in the document name, we will initialize the xml doc before opening into edit mode
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Module>WHATEVER_MODULE_NAME_HERE</Module>");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            // Save the document to a file and auto-indent the output.
            XmlWriter writer = XmlWriter.Create("module_data.xml", settings);

            doc.Save(writer);
            writer.Close(); //closes the writer so we can later edit it in edit mode
        }
    }
}

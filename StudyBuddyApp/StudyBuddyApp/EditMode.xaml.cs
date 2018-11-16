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
using System.Xml.Linq;

namespace StudyBuddyApp
{
    /// <summary>
    /// Interaction logic for EditMode.xaml
    /// </summary>
    public partial class EditMode : Page
    {
        public EditMode()
        {
            InitializeComponent();

            //this is just me messing with label names and seeing if we can change them after they are created
            string test = Chapter1Label.ToString();
            Console.WriteLine(test);
            Console.WriteLine(Chapter1Label.Name);
            Chapter1Label.Name = "Label1";
            Chapter1Label.Content = "TESTING";
            Console.WriteLine(Chapter1Label.Name);
            Label label2 = new Label();
            label2.Content = "SOMETHING";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            XElement test = XElement.Load(@"module_data.xml");
            XmlDocument doc = new XmlDocument();
            Console.WriteLine(test);
            doc.LoadXml(test.ToString());
            Console.WriteLine(doc);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create("module_data.xml", settings);
            XmlElement newElem = doc.CreateElement("Chapter");
            newElem.InnerText = "INSERT_CHAPTER_NAME";
            doc.DocumentElement.AppendChild(newElem);
            doc.Save(writer);
            writer.Close();
        }
    }
}

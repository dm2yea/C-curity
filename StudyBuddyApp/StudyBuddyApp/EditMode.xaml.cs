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
    /// Interaction logic for EditMode.xaml
    /// </summary>
    public partial class EditMode : Page
    {
        public EditMode()
        {
            InitializeComponent();
            // As soon as you enter edit mode, creates the XmlDocument.
            // the path is StudyBuddyApp/bin/Debug, it's in debug rn because that's how we're running it, 
            // when we actually make the exe it will be in the release folder I believe
            // to get to that folder you can right click on the solution explorer and open folder in file explorer
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<item><name>wrench</name></item>");

            // Add a price element.
            XmlElement newElem = doc.CreateElement("price");
            newElem.InnerText = "10.95";
            doc.DocumentElement.AppendChild(newElem);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            // Save the document to a file and auto-indent the output.
            XmlWriter writer = XmlWriter.Create("data.xml", settings);
            doc.Save(writer);

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
    }
}

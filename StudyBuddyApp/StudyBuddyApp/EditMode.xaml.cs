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
            //testing adding a label to the sidebar
            Label label2 = new Label();
            label2.Content = "SOMETHING";
            TitleBar.Children.Add(label2); //The sidebar has the name of TitleBar in xaml so we can add children to it programatically 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"module_data.xml");
            Console.WriteLine(doc);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create("module_data.xml", settings);
            XmlElement newElem = doc.CreateElement("Chapter");
            newElem.InnerText = "INSERT_CHAPTER_NAME";
            doc.DocumentElement.AppendChild(newElem);
            doc.Save(writer);
            writer.Close();
            test();
        }

        private void test()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"module_data.xml");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter writer = XmlWriter.Create("module_data.xml", settings);
            XmlElement e1 = (XmlElement)doc.SelectSingleNode("/Module/" + "Chapter");
            XmlElement elem = doc.CreateElement("Section");
            elem.InnerText = "SECTION_NAME";
            e1.AppendChild(elem);
            XmlElement elem2 = doc.CreateElement("Quiz");
            elem2.InnerText = "QUIZ_CONTENT";
            e1.AppendChild(elem2);

            doc.Save(writer);
            writer.Close();
        }

        //Creat pop-up when "plus" button is clicked for user to create new Chapter/Section/Subsection/Quiz
        private void Click_Add(object sender, MouseButtonEventArgs e)
        {
            AddPopup.IsOpen = true;
        }

        private void Click_Chapter(object sender, RoutedEventArgs e)
        {
            AddPopup.IsOpen = false;
            NamePopup.IsOpen = true;
        }

        private void Click_Section(object sender, RoutedEventArgs e)
        {
            AddPopup.IsOpen = false;
            NamePopup.IsOpen = true;
        }

        private void Click_Sub(object sender, RoutedEventArgs e)
        {
            AddPopup.IsOpen = false;
            NamePopup.IsOpen = true;
        }

        private void Click_Quiz(object sender, RoutedEventArgs e)
        {
            AddPopup.IsOpen = false;
            NamePopup.IsOpen = true;
        }

        private void Click_Name_Ok(object sender, RoutedEventArgs e)
        {
            //will need to know what kind of section is being added, so if we don't want to make 4 different name popups 
            //for each type then we'll want to add some kind of flag variable
            NamePopup.IsOpen = false;
            Label newLabel = new Label();
            newLabel.Content = nameTextBox.Text;
            TitleBar.Children.Add(newLabel); //creates a label and adds it to the title bar on the left side, but not underneath??
            
        }

    }
}

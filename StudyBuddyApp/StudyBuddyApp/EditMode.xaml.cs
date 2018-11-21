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
        String moduleName = ModuleData.ModuleName;
        int addType;

        public EditMode()
        {
            InitializeComponent();            

            //testing adding a label to the sidebar
            Label label2 = new Label();
            label2.Content = moduleName;
            TitleBar.Children.Add(label2); //The sidebar has the name of TitleBar in xaml so we can add children to it programatically 
        }

        private void Exit_To_Home(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Home());
        }

        private void Click_Name_Ok(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = false;

            XDocument doc = XDocument.Load(moduleName + ".xml");

            XmlWriterSettings settings = new XmlWriterSettings {Indent = true};

            if (addType == 1)
            {
                XElement chapter = new XElement("Chapter", new XElement("ChapterTitle", nameTextBox.Text), new XElement("SectionCount", 0), 
                    new XElement("SectionQuiz", 0), new XElement("QuizAverage", null));
                doc.Root.Add(chapter);
            }
            else if (addType == 2)
            {
                XElement chapter = new XElement("Section", new XElement("SectionTitle", nameTextBox.Text), new XElement("SectionContent", ""),
                    new XElement("Flagged", 0));
                doc.Root.Add(chapter);
            }
            else if (addType == 3)
            {
                XElement chapter = new XElement("Quiz", new XElement("QuizTitle", nameTextBox.Text), new XElement("QuizGrade", null));
                doc.Root.Add(chapter);
            }


            doc.Save(moduleName+".xml");
        }

        private void Click_Chapter(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = true;
            addType = 1;
        }

        private void Click_Section(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = true;
            addType = 2;
        }

        private void Click_Quiz(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = true;
            addType = 3;
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = false;
            addType = 0;
        }
    }
}

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
            XDocument doc = XDocument.Load(moduleName + ".xml");
            IEnumerable<XElement> ts = doc.Root.Elements().Elements();
            foreach (XElement node in ts)
            {
                if (node.Name == "ChapterTitle")
                {
                    ModuleData.CurrentChapter = node.Value;
                    break;
                }
            }
        }

        private void Exit_To_Home(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Home());
        }

        private void Click_Name_Ok(object sender, RoutedEventArgs e)
        {
            if(addType == 1)
            {
                ModuleData.CurrentChapter = nameTextBox.Text;
            }
            NamePopup.IsOpen = false;

            XDocument doc = XDocument.Load(moduleName + ".xml");

            XmlWriterSettings settings = new XmlWriterSettings {Indent = true};

            IEnumerable<XElement> ts = doc.Root.Elements();
            IEnumerable<XElement> ts2 = doc.Root.Elements().Elements();

            

            if (addType == 1)
            {
                XElement chapter = new XElement("Chapter", new XElement("ChapterTitle", nameTextBox.Text), new XElement("SectionCount", 0), 
                    new XElement("QuizCount", 0), new XElement("QuizAverage", "-"));
                doc.Root.Add(chapter);
                ModuleData.CurrentChapter = nameTextBox.Text;
                foreach (XElement node in ts)
                {
                    if (node.Name == "ChapterCount")
                    {
                        node.Value = Convert.ToString(ts.Count()-2);
                    }
                }
            }
            else if (addType == 2)
            {
                XElement section = new XElement("Section", new XElement("SectionTitle", nameTextBox.Text), new XElement("SectionContent", "Add some notes"),
                    new XElement("Flagged", 0));
                foreach (XElement node in ts2)
                {
                    if (node.Value.Equals(ModuleData.CurrentChapter))
                    {
                        foreach(XElement node2 in ts2)
                        {
                            if(node2.Name == "SectionCount")
                            {
                                node2.Value = Convert.ToString(Convert.ToInt32(node2.Value) + 1);
                            }
                        }
                        node.Parent.Add(section);
                    }
                }
            }
            else if (addType == 3)
            {
                XElement quiz = new XElement("Quiz", new XElement("QuizTitle", nameTextBox.Text), new XElement("QuizGrade", "-"));
                foreach (XElement node in ts2)
                {
                    if (node.Value.Equals(ModuleData.CurrentChapter))
                    {
                        foreach (XElement node2 in ts2)
                        {
                            if (node2.Name == "QuizCount")
                            {
                                node2.Value = Convert.ToString(Convert.ToInt32(node2.Value) + 1);
                            }
                        }
                        node.Parent.Add(quiz);
                    }
                }
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

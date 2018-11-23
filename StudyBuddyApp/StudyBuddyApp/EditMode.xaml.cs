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
                    if (node.Value == ModuleData.CurrentChapter)
                    {
                        XElement parent = node.Parent;
                        XElement count = parent.Element("SectionCount");
                        count.Value = Convert.ToString(Convert.ToInt32(count.Value)+1);
                        node.Parent.Add(section);
                    }
                }
            }
            else if (addType == 3)
            {
                XElement quiz = new XElement("Quiz", new XElement("QuizTitle", nameTextBox.Text), new XElement("QuizGrade", "-"));
                foreach (XElement node in ts2)
                {
                    if (node.Value == ModuleData.CurrentChapter)
                    {
                        XElement parent = node.Parent;
                        XElement count = parent.Element("QuizCount");
                        count.Value = Convert.ToString(Convert.ToInt32(count.Value) + 1);
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
        
        //opens popup to select question type
        private void Click_Add_Question_Button(object senter, RoutedEventArgs e)
        {
            QuizPopup.IsOpen = true;
        }
        
        int qCount = 0; //question number counter

        //method for adding a Multiple Choice question template
        private void Click_Multiple_Choice_Button(object sender, RoutedEventArgs e)
        {
            QuizPopup.IsOpen = false;
            qCount++; //updating question number

            //group box with border and question number containing the template
            GroupBox question = new GroupBox();
            question.Name = "question";
            question.Header = "Question" + qCount + ": Multiple Choice - Select the correct response.";
            question.Margin = new Thickness(0, 0, 168, 0);
            question.Height = 189;
            question.BorderBrush = Brushes.Black;

            //canvas inside group box to add more controls
            Canvas c1 = new Canvas();
            c1.Height = 169;
            c1.Width = 826;
            c1.Margin = new Thickness(0, 0, -2, -2);

            //text box for the question written by the user
            TextBox t1 = new TextBox();
            t1.Height = 74;
            t1.Width = 626;
            t1.TextWrapping = TextWrapping.Wrap;
            c1.Children.Add(t1);
            
            //4 radio buttons for the potential answers
            RadioButton rA = new RadioButton();
            rA.Content = "A.";
            rA.Margin = new Thickness(65, 79, 0, 0);
            c1.Children.Add(rA);
            RadioButton rB = new RadioButton();
            rB.Content = "B.";
            rB.Margin = new Thickness(423, 79, 0, 0);
            c1.Children.Add(rB);
            RadioButton rC = new RadioButton();
            rC.Content = "C.";
            rC.Margin = new Thickness(65, 125, 0, 0);
            c1.Children.Add(rC);
            RadioButton rD = new RadioButton();
            rD.Content = "D.";
            rD.Margin = new Thickness(423, 125, 0, 0);
            c1.Children.Add(rD);

            //4 text boxes for the potential answers
            TextBox tA = new TextBox();
            tA.Height = 44;
            tA.Width = 303;
            tA.Margin = new Thickness(99, 79, 0, 0);
            tA.TextWrapping = TextWrapping.Wrap;
            c1.Children.Add(tA);
            TextBox tB = new TextBox();
            tB.Height = 44;
            tB.Width = 303;
            tB.Margin = new Thickness(456, 79, 0, 0);
            tB.TextWrapping = TextWrapping.Wrap;
            c1.Children.Add(tB);
            TextBox tC = new TextBox();
            tC.Height = 44;
            tC.Width = 303;
            tC.Margin = new Thickness(99, 125, 0, 0);
            tC.TextWrapping = TextWrapping.Wrap;
            c1.Children.Add(tC);
            TextBox tD = new TextBox();
            tD.Height = 44;
            tD.Width = 303;
            tD.Margin = new Thickness(456, 125, 0, 0);
            tD.TextWrapping = TextWrapping.Wrap;
            c1.Children.Add(tD);

            //combo box the quiz maker uses to define the answer
            ComboBox answer = new ComboBox();
            answer.Margin = new Thickness(650, 0, 0, 0);
            answer.Items.Add("A");
            answer.Items.Add("B");
            answer.Items.Add("C");
            answer.Items.Add("D");
            c1.Children.Add(answer);

            question.Content = c1;
            quizSpace.Children.Add(question); //adds the question # group box to the quiz stack panel
        }
        private void Click_True_False_Button(object senter, RoutedEventArgs e)
        {
            QuizPopup.IsOpen = false;
            qCount++; //updating question number

            //group box with border and question number containing the template
            GroupBox question = new GroupBox();
            question.Name = "question";
            question.Header = "Question" + qCount + ": True or False - Select the correct response.";
            question.Margin = new Thickness(0, 0, 168, 0);
            question.Height = 189;
            question.BorderBrush = Brushes.Black;

            //canvas inside group box to add more controls
            Canvas c1 = new Canvas();
            c1.Height = 169;
            c1.Width = 826;
            c1.Margin = new System.Windows.Thickness(0, 0, -2, -2);

            //text box for the question written by the user
            TextBox t1 = new TextBox();
            t1.Height = 74;
            t1.Width = 626;
            t1.TextWrapping = TextWrapping.Wrap;
            c1.Children.Add(t1);

            //2 radio buttons for true/false answers
            RadioButton r1 = new RadioButton();
            r1.Margin = new Thickness(65, 100, 0, 0);
            r1.Content = "True";
            c1.Children.Add(r1);
            RadioButton r2 = new RadioButton();
            r2.Margin = new Thickness(65, 125, 0, 0);
            r2.Content = "False";
            c1.Children.Add(r2);

            //combo box the quiz maker uses to define the answer
            ComboBox answer = new ComboBox();
            answer.Margin = new Thickness(650, 0, 0, 0);
            answer.Items.Add("True");
            answer.Items.Add("False");
            c1.Children.Add(answer);

            question.Content = c1;
            quizSpace.Children.Add(question); //adds the question # group box to the quiz stack panel
        }
        private void Click_Fill_Blank_Button(object senter, RoutedEventArgs e)
        {
            QuizPopup.IsOpen = false;
            qCount++; //updating question number

            //group box with border and question number containing the template
            GroupBox question = new GroupBox();
            question.Name = "question";
            question.Header = "Question" + qCount + ": Fill in the Blank - Type the answer to the given problem.";
            question.Margin = new Thickness(0, 0, 168, 0);
            question.Height = 189;
            question.BorderBrush = Brushes.Black;

            //canvas inside group box to add more controls
            Canvas c1 = new Canvas();
            c1.Height = 169;
            c1.Width = 826;
            c1.Margin = new Thickness(0, 0, -2, -2);

            //text box for the question written by the user
            TextBox t1 = new TextBox();
            t1.Height = 74;
            t1.Width = 626;
            t1.TextWrapping = TextWrapping.Wrap;
            c1.Children.Add(t1);

            //text box for the answer
            TextBox t2 = new TextBox();
            t2.Margin = new Thickness(0, 80, 0, 0);
            t2.Height = 70;
            t2.Width = 500;
            t2.Text = "Enter a response here.";
            c1.Children.Add(t2);

            TextBox answer = new TextBox();
            answer.Margin = new Thickness(650, 0, 0, 0);
            answer.Width = 150;
            answer.Height = 74;
            answer.Text = "ANSWER";
            c1.Children.Add(answer);

            question.Content = c1;
            quizSpace.Children.Add(question); //adds the question # group box to the quiz stack panel
        }
        private void Click_Quiz_Cancel_Button(object senter, RoutedEventArgs e)
        {
            QuizPopup.IsOpen = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Media.Media3D;
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
        List<Chapter> chapters;
        TreeViewItem item;
        int itemCount;
        String moduleName = ModuleData.ModuleName;
        int addType;
        XDocument doc;

        public EditMode(string title)
        {
            InitializeComponent();
            moduleNameBar.Text = title;
            itemCount = 0;
            chapters = new List<Chapter>();
            sectionTitle.Visibility = Visibility.Hidden;
            sectionContent.Visibility = Visibility.Hidden;

            doc = XDocument.Load(moduleName + ".xml");
            IEnumerable<XElement> ts = doc.Root.Elements().Elements();
            foreach (XElement node in ts)
            {
                if (node.Name == "ChapterTitle")
                {
                    ModuleData.CurrentChapter = node.Value;
                    break;
                }
            }

            foreach (XElement node in ts)
            {
                if (node.Name == "ChapterTitle")
                {
                    chapters.Add(new Chapter(node.Value, "_" + itemCount++));
                }
            }

            IEnumerable<XElement> ts2 = doc.Root.Elements().Elements().Elements();
            foreach(XElement node in ts2)
            { 
                if (node.Name == "SectionTitle")
                {
                    XElement parentChapter = node.Parent.Parent;
                    IEnumerable<XElement> parentChapterElements = parentChapter.Elements();
                    foreach(XElement childElement in parentChapterElements)
                    {
                        if (childElement.Name == "ChapterTitle")
                        {
                            Chapter sectionParent = ChapterNameAlreadyExists(childElement.Value);
                            if(sectionParent != null)
                            {
                                sectionParent.GetSectionList().Add(new section(node.Value, "_" + itemCount++, sectionParent));
                            }
                        }
                    }
                }
            }
            IEnumerable<XElement> ts3 = doc.Root.Elements().Elements().Elements();
            foreach (XElement node in ts3)
            {
                if (node.Name == "QuizTitle")
                {
                    XElement parentChapter = node.Parent.Parent;
                    IEnumerable<XElement> parentChapterElements = parentChapter.Elements();
                    foreach (XElement childElement in parentChapterElements)
                    {
                        if (childElement.Name == "ChapterTitle")
                        {
                            Chapter quizParent = ChapterNameAlreadyExists(childElement.Value);
                            if (quizParent != null)
                            {
                                quizParent.GetQuizList().Add(new Quizzes(node.Value, "_" + itemCount++, quizParent));
                            }
                        }
                    }
                }
            }

            show(treeView);
            Rename_textBox.Visibility = Visibility.Hidden;
        }

        //saves the contents of the current section
        private void Save_Contents(object sender, RoutedEventArgs e)
        {
            doc = XDocument.Load(moduleName + ".xml");
            XElement parent = null;
            IEnumerable<XElement> ts = doc.Root.Elements().Elements().Elements();
            foreach (XElement node in ts)
            {
                if (node.Value == ModuleData.CurrentSection)
                {
                    node.Value = sectionTitle.Text;
                }
                if(node.Name == "SectionContent")
                {
                    parent = node.Parent;
                    if(parent.Element("SectionTitle").Value == ModuleData.CurrentSection)
                    {
                        node.Value = sectionContent.Text;
                    }
                }
            }
            doc.Save(moduleName+".xml");
            ModuleData.CurrentSection = sectionTitle.Text;
            show(treeView);
        }

        //sends the user back to the homepage
        private void Exit_To_Home(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Home());
        }

        //determines if the given name has already been used for a chapter
        private Chapter ChapterNameAlreadyExists(String title)
        {
            foreach(Chapter currentChapter in chapters)
            {
                if(currentChapter.getName() == title)
                {
                    return currentChapter;
                }
            }
            return null;
        }

        //creates chapter, section, or quiz
        private void Click_Name_Ok(object sender, RoutedEventArgs e)
        {
            if (addType == 1)
            {
                ModuleData.CurrentChapter = nameTextBox.Text;
            }
            else
            {
                if (treeView.SelectedItem != null)
                {
                    Chapter tempChapter = treeViewItemToChapter((TreeViewItem)treeView.SelectedItem);
                    ModuleData.CurrentChapter = tempChapter.getName();
                }
                else
                {
                    int x = 0;
                    foreach (Chapter tempChapter in chapters)
                    {
                        x++;
                        if (x == chapters.Count)
                        {
                            ModuleData.CurrentChapter = tempChapter.getName();
                            break;
                        }
                    }
                }
            }

            XDocument doc = XDocument.Load(moduleName + ".xml");

            XmlWriterSettings settings = new XmlWriterSettings { Indent = true };

            IEnumerable<XElement> ts = doc.Root.Elements();
            IEnumerable<XElement> ts2 = doc.Root.Elements().Elements();



            if (addType == 1)
            {
                String name = nameTextBox.GetLineText(0);
                if (ChapterNameAlreadyExists(name) == null)
                {
                    XElement chapter = new XElement("Chapter", new XElement("ChapterTitle", nameTextBox.Text), new XElement("SectionCount", 0),
                        new XElement("QuizCount", 0), new XElement("QuizAverage", "-"));
                    doc.Root.Add(chapter);
                    ModuleData.CurrentChapter = nameTextBox.Text;
                    foreach (XElement node in ts)
                    {
                        if (node.Name == "ChapterCount")
                        {
                            node.Value = Convert.ToString(ts.Count() - 2);
                        }
                    }

                    chapters.Add(new Chapter(name, "_" + itemCount++));
                    show(treeView);
                }
                else
                {
                    InvalidNameWarningPopup.IsOpen = true;
                    Warning_Ok_Button.Focus();
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
                        count.Value = Convert.ToString(Convert.ToInt32(count.Value) + 1);
                        node.Parent.Add(section);
                    }
                }


                TreeViewItem item2 = (TreeViewItem)treeView.SelectedItem;

                if (item2 != null)
                {
                    foreach (Chapter tempChapter in chapters)
                    {
                        if (tempChapter.getItemID() == item2.Name)
                        {
                            tempChapter.GetSectionList().Add(new section(nameTextBox.GetLineText(0), "_" + itemCount++, tempChapter));
                            show(treeView);
                            item2.MouseLeftButtonUp += Section_Display_Click;
                            break;
                        }
                    }
                }
                else
                {
                    int x = 0;
                    foreach (Chapter tempChapter in chapters)
                    {
                        x++;
                        if (x == chapters.Count)
                        {
                            tempChapter.GetSectionList().Add(new section(nameTextBox.GetLineText(0), "_" + itemCount++, tempChapter));
                            show(treeView);
                            break;
                        }
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

                TreeViewItem item3 = (TreeViewItem)treeView.SelectedItem;

                if (item3 != null)
                {
                    foreach (Chapter tempChapter in chapters)
                    {
                        if (tempChapter.getItemID() == item3.Name)
                        {
                            tempChapter.GetQuizList().Add(new Quizzes(nameTextBox.GetLineText(0), "_" + itemCount++, tempChapter));
                            show(treeView);
                            //item3.MouseLeftButtonUp += Quiz_Display_Click;
                            break;
                        }
                    }
                }
                else
                {
                    int x = 0;
                    foreach (Chapter tempChapter in chapters)
                    {
                        x++;
                        if (x == chapters.Count)
                        {
                            tempChapter.GetQuizList().Add(new Quizzes(nameTextBox.GetLineText(0), "_" + itemCount++, tempChapter));
                            show(treeView);
                            break;
                        }
                    }
                }
            }
            doc.Save(moduleName+".xml");
            if(InvalidNameWarningPopup.IsOpen == false)
            {
                NamePopup.IsOpen = false;
            }
        }

        /* Tanner Chauncy - 11/24/2018
         * getKeyboardFocus() - This method improves quality of life for the user when adding a new item. It resets the text in the new
         * item popup textbox, diverts focus to the textbox, selects all of the text in the textbox, and allows the user to press enter
         * as an alternative to the "Ok button".
         * */
        private void getKeyboardFocus()
        {
            nameTextBox.Text = "Enter Name";
            nameTextBox.Focus();
            Keyboard.Focus(nameTextBox);
            nameTextBox.SelectAll();
            Ok_Button.IsDefault = true;
        }

        private void Click_Chapter(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = true;
            getKeyboardFocus();
            addType = 1;
        }

        private void Click_Section(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = true;
            getKeyboardFocus();
            addType = 2;
        }

        private void Click_Quiz(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = true;
            getKeyboardFocus();
            addType = 3;
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = false;
            getKeyboardFocus();
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

        /* Tanner Chauncy - 11/24/2018
         * Click_Remove_Item() - This method handles the task of removing an item selected to be removed in its TreeView context menu.
         * 
         * Parameters: 
         * These parameters were autogenerated.
         * */
        private void Click_Remove_Item(object senter, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = (TreeViewItem)treeView.SelectedItem;

            if (treeViewItem != null)
            {
                doc = XDocument.Load(moduleName + ".xml");

                Chapter chapterToRemove = treeViewItemToChapter(treeViewItem);
                if (chapterToRemove != null)
                {
                    IEnumerable<XElement> ts = doc.Root.Elements().Elements();
                    foreach (XElement node in ts)
                    {
                        if (node.Name == "ChapterTitle" && node.Value == chapterToRemove.getName())
                        {
                            node.Parent.Remove();
                        }
                    }

                    chapters.Remove(chapterToRemove);
                    show(treeView);
                }
                else
                {
                    section sectionToRemove = treeViewItemToSection(treeViewItem);
                    if (sectionToRemove != null)
                    {
                        IEnumerable<XElement> ts = doc.Root.Elements().Elements().Elements();
                        foreach (XElement node in ts)
                        {
                            if (node.Name == "SectionTitle" && node.Value == sectionToRemove.getName())
                            {
                                node.Parent.Remove();
                            }
                        }
                        sectionToRemove.getParent().GetSectionList().Remove(sectionToRemove);
                        show(treeView);
                    }
                }

            }
            doc.Save(moduleName + ".xml");
        }

        /* Tanner Chauncy - 11/24/2018
         * treeViewItemToChapter() - This method takes a TreeViewItem and searches through the list of Chapters named "chapters" to
         * compare the itemID of each Chapter to the TreeViewItem. If there is a match, it returns the TreeViewItem's
         * corresponding Chapter object.
         * 
         * Parameters: 
         * item - A TreeViewItem of which the caller wants to find the corresponding Chapter for.
         * */
        private Chapter treeViewItemToChapter(TreeViewItem item)
        {
            Chapter returnValue = null;
            foreach (Chapter currentChapter in chapters)
            {
                if (currentChapter.getItemID() == item.Name)
                {
                    returnValue = currentChapter;
                    break;
                }
            }
            return returnValue;
        }

        /* Tanner Chauncy - 11/24/2018
         * treeViewItemToSection() - This method takes a TreeViewItem and searches through the section list of each chapter in the list 
         * "chapters" to compare the itemID of each section to the TreeViewItem. If there is a match, it returns the TreeViewItem's
         * corresponding section object.
         * 
         * Parameters: 
         * item - A TreeViewItem of which the caller wants to find the corresponding section for.
         * */
        private section treeViewItemToSection(TreeViewItem item)
        {
            section returnValue = null;
            foreach (Chapter currentChapter in chapters)
            {
                foreach (section currentSection in currentChapter.GetSectionList())
                {
                    if (currentSection.getItemID() == item.Name)
                    {
                        returnValue = currentSection;
                        break;
                    }
                }
            }
            return returnValue;
        }

        private Quizzes treeViewItemToQuiz(TreeViewItem item)
        {
            Quizzes returnValue = null;
            foreach (Chapter currentChapter in chapters)
            {
                foreach (Quizzes currentQuiz in currentChapter.GetQuizList())
                {
                    if (currentQuiz.getItemID() == item.Name)
                    {
                        returnValue = currentQuiz;
                        break;
                    }
                }
            }
            return returnValue;
        }

        /* Tanner Chauncy - 11/24/2018
         * TreeViewItem_PreviewMouseRightButtonDown() - This method handles the task of showing the context menu correctly depending on
         * the type of TreeViewItem being right clicked.
         * 
         * Parameters: 
         * These parameters were autogenerated.
         * */
        void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem =
                      VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                Boolean isChapter = false;
                Chapter tempChapter = treeViewItemToChapter(treeViewItem);
                if(tempChapter != null)
                {
                    isChapter = true;
                }

                if (isChapter)
                {
                    addSectionMenuItem.IsEnabled = true;
                    removeItem.Header = "Remove Chapter";
                    ModuleData.CurrentChapter = tempChapter.getName();
                }
                else
                {
                    addSectionMenuItem.IsEnabled = false;
                    removeItem.Header = "Remove Section";
                }
                treeViewItem.IsSelected = true;
                treeViewContextMenu.IsOpen = true;
                e.Handled = true;
            }
        }


        static T VisualUpwardSearch<T>(DependencyObject source) where T : DependencyObject
        {
            DependencyObject returnVal = source;

            while (returnVal != null && !(returnVal is T))
            {
                DependencyObject tempReturnVal = null;
                if (returnVal is Visual || returnVal is Visual3D)
                {
                    tempReturnVal = VisualTreeHelper.GetParent(returnVal);
                }
                if (tempReturnVal == null)
                {
                    returnVal = LogicalTreeHelper.GetParent(returnVal);
                }
                else returnVal = tempReturnVal;
            }

            return returnVal as T;
        }

        /* Tanner Chauncy - 11/24/2018
         * show() - This method clears the TreeView given in the parameters and then repopulates the TreeView with the updated information.
         * 
         * Parameters: 
         * tree - A TreeView object to be repopulated.
         * */
        public void show(TreeView tree)
        {
            tree.Items.Clear();
            foreach (Chapter currentChapter in chapters)
            {
                item = GetTreeView(currentChapter.getItemID(), currentChapter.getName(), "Chapter.png");


                foreach (section currentSection in currentChapter.GetSectionList())
                {
                    TreeViewItem subItem = GetTreeView(currentSection.getItemID(), currentSection.getName(), "Section.png");
                    item.Items.Add(subItem);
                }
                foreach (Quizzes currentQuiz in currentChapter.GetQuizList())
                {
                    TreeViewItem subItem = GetTreeView(currentQuiz.getItemID(), currentQuiz.getName(), "Quiz.png");
                    item.Items.Add(subItem);
                }
                tree.Items.Add(item);
                item.ExpandSubtree();
            }

        }

        /* Tanner Chauncy - 11/24/2018
         * GetTreeView() - This method takes the given parameters to create a TreeViewItem to be displayed in the TreeView on Edit Mode.
         * 
         * Parameters: 
         * itemID - A string containing the Study Buddy item's itemID.
         * header - A string containing the text to be displayed next to the TreeViewItem.
         * imagePath - A string containing the filepath to an image in the resources folder to be used as an icon next to the TreeViewItem.
         * */
        private TreeViewItem GetTreeView(string itemID, string header, string imagePath)
        {
            TreeViewItem item = new TreeViewItem();
            item.Name = itemID;
            item.IsExpanded = false;

            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create Image
            Image image = new Image();
            Uri img = new Uri("Resources/" + imagePath, UriKind.RelativeOrAbsolute);
            image.Source = new BitmapImage(img);
            image.Width = 8;
            image.Height = 8;
            // Label
            Label lbl = new Label();
            lbl.Content = header;


            // Add into stack
            stack.Children.Add(image);
            stack.Children.Add(lbl);

            // assign stack to header
            item.Header = stack;
            return item;
        }

        public void Section_Display_Click(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
            
            if (item != null)
            {
                doc = XDocument.Load(moduleName + ".xml");
                section sectionCon = treeViewItemToSection(item);
                if (sectionCon != null)
                {
                    sectionTitle.Visibility = Visibility.Visible;
                    sectionContent.Visibility = Visibility.Visible;
                    IEnumerable<XElement> ts = doc.Root.Elements().Elements().Elements();
                  foreach (XElement node in ts)
                  {
                     if (node.Name == "SectionTitle" && node.Value == sectionCon.getName())
                     {
                        ModuleData.CurrentSection = node.Value;
                        sectionTitle.Text = node.Value;
                        XElement tempNode = node.Parent;
                        tempNode = tempNode.Element("SectionContent");
                        sectionContent.Text = tempNode.Value;
                        sectionContent.Focus();
                        sectionContent.SelectAll();
                     }
                  }
                }
                else
                {
                    sectionTitle.Visibility = Visibility.Hidden;
                    sectionContent.Visibility = Visibility.Hidden;
                }
            }
            e.Handled = true;
        }

        //public void Quiz_Display_Click(object sender, MouseButtonEventArgs e)
        //{
        //    TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
        //    ModuleData.CurrentSection = this.Name;


        //    if (item != null)
        //    {
        //        doc = XDocument.Load(moduleName + ".xml");
        //        Quiz quizCon = treeViewItemToQuiz(item);
        //        if (sectionCon != null)
        //        {
        //            IEnumerable<XElement> ts = doc.Root.Elements().Elements().Elements();
        //            foreach (XElement node in ts)
        //            {
        //                if (node.Name == "SectionTitle" && node.Value == sectionCon.getName())
        //                {
        //                    sectionTitle.Text = node.Value;
        //                    XElement tempNode = node.Parent;
        //                    tempNode = tempNode.Element("SectionContent");
        //                    sectionContent.Text = tempNode.Value;
        //                }
        //            }
        //        }
        //    }
        //    e.Handled = true;
        //}

        private void Click_Warning_Ok(object sender, RoutedEventArgs e)
        {
            InvalidNameWarningPopup.IsOpen = false;
            getKeyboardFocus();
        }

        private void Click_Module_Rename(object sender, RoutedEventArgs e)
        {
            Rename_textBox.Text = moduleNameBar.Text;
            moduleNameBar.Text = "";
            Rename_textBox.Visibility = Visibility.Visible;
            Rename_textBox.Focus();
            Rename_textBox.SelectAll();
            Confirm_Rename_Button.IsDefault = true;
            exitButton.IsEnabled = false;
        }

        private void Click_Confirm_Rename(object sender, RoutedEventArgs e)
        {
            String oldModuleFile = @"..\..\bin\Debug\" + moduleName + ".xml";
            String newModuleName = Rename_textBox.Text;
            String newModuleFile = @"..\..\bin\Debug\" + newModuleName + ".xml";

            if (File.Exists(newModuleFile)){
                ModuleExistsWarningPopup.IsOpen = true;
                Module_Exists_Ok_Button.Focus();
            }
            else
            {
                Rename_textBox.Visibility = Visibility.Hidden;
                moduleNameBar.Text = newModuleName;
                saveButton.Focus();

                System.IO.File.Move(oldModuleFile, newModuleFile);
                ModuleData.ModuleName = newModuleName;
                moduleName = ModuleData.ModuleName;
                doc = XDocument.Load(moduleName + ".xml");
                IEnumerable<XElement> ts = doc.Root.Elements();
                foreach (XElement node in ts)
                {
                    if (node.Name == "ModuleName")
                    {
                        node.Value = moduleName;
                        break;
                    }
                }
                doc.Save(moduleName + ".xml");
                exitButton.IsEnabled = true;
            }
        }
        

        private void Click_Module_Exists_Ok(object sender, RoutedEventArgs e)
        {
            ModuleExistsWarningPopup.IsOpen = false;
            Rename_textBox.Focus();
            Rename_textBox.SelectAll();
            Confirm_Rename_Button.IsDefault = true;
        }
    }
}

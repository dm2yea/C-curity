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
        public partial class StudyMode : Page
        {
            List<Chapter> chapters;
            TreeViewItem item;
            int itemCount;
            String moduleName = ModuleData.ModuleName;
            XDocument doc;

            public StudyMode(string title)
            {
                InitializeComponent();
                moduleNameBar.Text = title;
                itemCount = 0;
                chapters = new List<Chapter>();

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
                foreach (XElement node in ts2)
                {
                    if (node.Name == "SectionTitle")
                    {
                        XElement parentChapter = node.Parent.Parent;
                        IEnumerable<XElement> parentChapterElements = parentChapter.Elements();
                        foreach (XElement childElement in parentChapterElements)
                        {
                            if (childElement.Name == "ChapterTitle")
                            {
                                Chapter sectionParent = chapterNameAlreadyExists(childElement.Value);
                                if (sectionParent != null)
                                {
                                    sectionParent.GetSectionList().Add(new section(node.Value, "_" + itemCount++, sectionParent));
                                }
                            }
                        }
                    }
                }

                show(treeView);
                Rename_textBox.Visibility = Visibility.Hidden;
            }

            private void Exit_To_Home(object sender, RoutedEventArgs e)
            {
                this.NavigationService.Navigate(new Home());
            }

            private Chapter chapterNameAlreadyExists(String title)
            {
                foreach (Chapter currentChapter in chapters)
                {
                    if (currentChapter.getName() == title)
                    {
                        return currentChapter;
                    }
                }
                return null;
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
                ModuleData.CurrentSection = this.Name;


                if (item != null)
                {
                    doc = XDocument.Load(moduleName + ".xml");
                    section sectionCon = treeViewItemToSection(item);
                    if (sectionCon != null)
                    {
                        IEnumerable<XElement> ts = doc.Root.Elements().Elements().Elements();
                        foreach (XElement node in ts)
                        {
                            if (node.Name == "SectionTitle" && node.Value == sectionCon.getName())
                            {
                                sectionTitle.Text = node.Value;
                                XElement tempNode = node.Parent;
                                tempNode = tempNode.Element("SectionContent");
                                sectionContent.Text = tempNode.Value;
                            }
                        }
                    }
                }
                e.Handled = true;
            }
        
        }
    }



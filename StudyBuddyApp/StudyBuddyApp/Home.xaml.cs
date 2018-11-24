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
using System.Data;
using System.IO;

namespace StudyBuddyApp
{
    //Interaction logic for Home.xaml

    public partial class Home : Page
    {
        //determines study mode or edit mode
        public Boolean viewMode = true;

        //initializes homepage
        public Home()
        {
            InitializeComponent();
            ReadingXmlFiles();
        }
        
        //opens popup when new module button is clicked
        private void NewModule(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = true;
            nameTextBox.Focus();
            Keyboard.Focus(nameTextBox);
            nameTextBox.SelectAll();
            Ok_Button.IsDefault = true;
        }
    
        //changes colors on the homepage depending on the current mode
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                BrushConverter bc = new BrushConverter();
                
                if (viewMode)
                {                   
                    image.Source = new BitmapImage(new Uri("Resources/grey-switch.jpg", UriKind.RelativeOrAbsolute));                    
                    newModuleButton.IsEnabled = true;
                    rectangle.Fill = (Brush)bc.ConvertFrom("#FF808080");
                    ScrollViewer.Background = (Brush)bc.ConvertFrom("#FF808080");
                    ScrollGrid.Background = (Brush)bc.ConvertFrom("#FF808080");
                    viewMode = false;                
                }
                else
                {                   
                    image.Source = new BitmapImage(new Uri("Resources/blue-switch.jpg", UriKind.RelativeOrAbsolute));                   
                    newModuleButton.IsEnabled =false;
                    rectangle.Fill = (Brush)bc.ConvertFrom("#FF3399FF");
                    ScrollViewer.Background = (Brush)bc.ConvertFrom("#FF3399FF");
                    ScrollGrid.Background = (Brush)bc.ConvertFrom("#FF3399FF");
                    viewMode = true;
                }
            }
        }

        //closes popup when ok button is clicked, creates the initial xml doc, and sends the user to edit mode
        private void Click_Name_Ok(object sender, RoutedEventArgs e)
        {
            String moduleName = nameTextBox.Text;
            NamePopup.IsOpen = false;

            //once we enter in the document name, we will initialize the xml doc before opening into edit mode
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Module><ModuleName>" + moduleName + "</ModuleName><ChapterCount>"+0+"</ChapterCount></Module>");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            // Save the document to a file and auto-indent the output.
            XmlWriter writer = XmlWriter.Create(moduleName + ".xml", settings);
            doc.Save(writer);
            writer.Close(); //closes the writer so we can later edit it in edit mode

            //saves module name to use for opening
            ModuleData.ModuleName = moduleName;

            // Navigate to edit page
            this.NavigationService.Navigate(new EditMode(moduleName));
        }



        //Handles event when user clicks on the Module they want to open
        public void Module_Click(object sender, RoutedEventArgs e)
        {
            String title = sender.ToString();
            title = title.Substring(32);
            int space = title.IndexOf('\n');
            title = title.Remove(space);
            ModuleData.ModuleName = title;
            if (viewMode)
            {
                this.NavigationService.Navigate(new StudyMode());
            }
            else
            {
                this.NavigationService.Navigate(new EditMode(title));
            }
        }

        //used to position Module correctly on home page
        int location = 20;

        //This creates the Module Icon on the Home Page when an XML file is read
        public void CreateModuleIcon(String title, String score)
        {
            Grid grid = new Grid
            {
                Height = 100,
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            grid.Margin = new Thickness(location, 10, 0, 0);
            ScrollGrid.Children.Add(grid);
            location += 200;
            grid.Background = new SolidColorBrush(Colors.White);


            Button modulebutton = new Button
            {
                Height = 100,
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Content = title + "\n\nQuiz Average: " + score
            };
            modulebutton.Click += Module_Click;
            grid.Children.Add(modulebutton);

            ScrollGrid.Width = ScrollGrid.Width + 200;
            ScrollPanel.Width = ScrollPanel.Width + 200;
        }

        //this reads the XML files in specified location
        public void ReadingXmlFiles()
        {
            string[] xmlfinder = Directory.GetFiles(@"..\..\bin\Debug", "*.xml");
            foreach (string filename in xmlfinder)
            {
                XmlTextReader reader = new XmlTextReader(filename);
                XmlNodeType type;

                String moduleName = null;
                String quizAverage = null;

                while (reader.Read())
                {
                    type = reader.NodeType;
                    if (type == XmlNodeType.Element)
                    {
                        if (reader.Name == "ModuleName")
                        {
                            reader.Read();
                            moduleName = reader.Value;
                        }
                        if (reader.Name == "QuizAverage")
                        {
                            reader.Read();
                            quizAverage = reader.Value;
                        }
                    }
                }
                CreateModuleIcon(moduleName, quizAverage);
            }
        }
    }
}

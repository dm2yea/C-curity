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
        public Boolean viewMode = true;

        public Home()
        {
            InitializeComponent();
            ReadingXmlFiles();
        }

        private void NewModule(object sender, RoutedEventArgs e)
        {
            NamePopup.IsOpen = true;
        }
    
        private void image_MouseDown(object sender, MouseButtonEventArgs e)
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
            this.NavigationService.Navigate(new EditMode());
        }



        //Handles event when user clicks on the Module they want to open
        public void Module_Click(object sender, RoutedEventArgs e)
        {
            label.Content = "Clicked";
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
            grid.MouseDown += new MouseButtonEventHandler(Module_Click);


            Label nameLabel = new Label
            {
                Height = 50,
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Content = title
            };
            grid.Children.Add(nameLabel);

            Label quizGradeLabel = new Label
            {
                Height = 50,
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Content = "Quiz Average: " + score
            };
            grid.Children.Add(quizGradeLabel);

            ScrollGrid.Width = ScrollGrid.Width + 200;
            ScrollPanel.Width = ScrollPanel.Width + 200;

        }

        //this reads the XML files in specified location
        public void ReadingXmlFiles()
        {
            string[] xmlfinder = Directory.GetFiles(@"C:\Users\16783\source\repos\StudyBuddy\StudyBuddyApp\StudyBuddyApp\bin\Debug", "*.xml");
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

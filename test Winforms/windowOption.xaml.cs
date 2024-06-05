using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using test_Winforms.utils;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace test_Winforms
{
    /// <summary>
    /// Логика взаимодействия для windowOption.xaml
    /// </summary>
    public partial class windowOption : Window
    {
        public windowOption()
        {
            InitializeComponent();
            /*Closed += (s, e) => {
                Option.TitleGame = nameGame.Text;
            };*/
        }

        public void WhenOpen()
        {
            if (File.Exists("assets\\icon.png"))
                imageIcon.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "assets\\icon.png"));
            if (File.Exists("assets\\mouse.png"))
                imageMouse.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "assets\\mouse.png"));
            nameGame.Text = Option.TitleGame;
            nameSceneStart.Text = Option.NameSceneStart;
            widthGame.Text = Option.WidhtScreen.ToString();
            heightGame.Text = Option.HeightScreen.ToString();
            checkBoxDebug.IsChecked = Option.DebugMode;
            mouseInGame.IsChecked = Option.MouseInGame;
            checkBoxDeviceConnect.IsChecked = Option.DeviceConnect;
            sizeMouseTexture.Text = Option.SizeMouseTexture;
        }


        public void openButtonHandler(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "assets\\ScriptButtonHandler.cs");
        }


        public void openFolderEngine(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory);
        }


        public void openFolderGame(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Option.OutPathOutFolder.Remove(Option.OutPathOutFolder.Length - 7));
        }

        public void findIcon(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialoge = new OpenFileDialog();
            fileDialoge.ShowDialog();
            if (fileDialoge.FileName == null || fileDialoge.FileName == "") return;
            //if(File.Exists("assets\\icon.png")) File.Delete("assets\\icon.png");
            File.Copy(fileDialoge.FileName, "assets\\icon.png", true);

            imageIcon.Source = new BitmapImage(new Uri(fileDialoge.FileName));
            Option.IconGame = fileDialoge.SafeFileName;
        }

        public void findImageMouse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialoge = new OpenFileDialog();
            fileDialoge.ShowDialog();
            if (fileDialoge.FileName == null || fileDialoge.FileName == "") return;
            //if(File.Exists("assets\\mouse.png")) File.Delete("assets\\mouse.png");
            File.Copy(fileDialoge.FileName, "assets\\mouse.png", true);

            imageMouse.Source = new BitmapImage(new Uri(fileDialoge.FileName));
            Option.ImageMouse = fileDialoge.SafeFileName;
        }

        public void findFont(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialoge = new OpenFileDialog();
            fileDialoge.ShowDialog();
            if (fileDialoge.FileName == null || fileDialoge.FileName == "") return;
            if(File.Exists("assets\\font.ttf")) File.Delete("assets\\font.ttf");
            File.Copy(fileDialoge.FileName, "assets\\font.ttf");

            //Option.FontPath = fileDialoge.SafeFileName;
        }

        public void selectColor(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AnyColor = true;
            colorDialog.ShowDialog();

            Option.ColorBackground = new byte[] { colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B };
        }

        public void buttonPathStart(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialoge = new OpenFileDialog();
            fileDialoge.ShowDialog();
            if (fileDialoge.FileName == null || fileDialoge.FileName == "") return;

            Option.PathStartFile = fileDialoge.FileName;
        }

        public void buttonOutFolder(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();

            Option.OutPathOutFolder = fbd.SelectedPath;
            /*OpenFileDialog fileDialoge = new OpenFileDialog();
            fileDialoge.ShowDialog();
            if (fileDialoge.FileName == null || fileDialoge.FileName == "") return;

            Option.OutPathOutFolder = fileDialoge.FileName;*/
        }

        public void buttonSave(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            Option.TitleGame = nameGame.Text;
            Option.WidhtScreen = int.Parse(widthGame.Text);
            Option.HeightScreen = int.Parse(heightGame.Text);
            Option.DebugMode = checkBoxDebug.IsChecked.Value;
            Option.MouseInGame = mouseInGame.IsChecked.Value;
            Option.DeviceConnect = checkBoxDeviceConnect.IsChecked.Value;
            Option.NameSceneStart = nameSceneStart.Text;
            Option.SizeMouseTexture = sizeMouseTexture.Text;
            MainWindow.saveXmlInfoGameEngine();
        }
    }
}

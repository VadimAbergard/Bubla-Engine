using bubla.utils;
using Bubla.utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using test_Winforms.utils;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Image = System.Windows.Controls.Image;

namespace test_Winforms
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private static List<Scene> scenes = new List<Scene>();
        private static List<string> listNameUI = new List<string>();
        private TextBlock textBlockSelectNow;
        private static int sceneSelectNow = 0;
        private windowOption windowOption;

        private int widthCanvas = 728;
        private int heightCanvas = /*335*/485;

        private Regex regex = new Regex("[^0-9.,-]+");

        private const double sizeEffectTexture = 50;

        private static bool shiftHold = false;
        private static bool ctrlHold = false;

        private static TypeProject typeProject = TypeProject.DESKTOP;

        private static Vector posCamera = new Vector(0 ,0);
        private static float zoomCamera = 0.4f;

        private static int maxLayer = 0;
        private static int maxLayerScript = 0;

        public MainWindow()
        {
            InitializeComponent();
            LoadInfoGame();
            LoadInfo();

            if(!scenes.Any())
            {
                Scene scene = new Scene("mainScene");
                scenes.Add(scene);
                listScenes.Items.Add(scene.Name);
                listScenes.SelectedItem = scene.Name;
            } else
            {
                foreach(Scene scene in scenes)
                {
                    listScenes.Items.Add(scene.Name);
                    //Console.WriteLine(scene.Uis.Count);
                }
                listScenes.SelectedItem = scenes[0].Name;

                UpdateScene();
            }

            nameScene.Text = scenes[0].Name;
            /*brbCanvas.Width = Option.WidhtScreen;
            brbCanvas.Height = Option.HeightScreen;
            canvas.Width = Option.WidhtScreen;
            canvas.Height = Option.HeightScreen;*/

            //AddAllElementsOnScene();

            if (!File.Exists("assets\\ScriptButtonHandler.cs"))
            {
                FileInfo scriptButtonHandler = new FileInfo("resourse\\scriptDefault.cs");
                scriptButtonHandler.CopyTo("assets\\ScriptButtonHandler.cs");
            }

            Closed += (s, e) => { saveXmlInfoGame(); saveXmlInfoGameEngine(); windowOption.Close(); };
            windowOption = new windowOption();
            nameObject.PreviewKeyUp += tb_KeyDownName_GameObject;
            nameUI.PreviewKeyUp += tb_KeyDownName_UI;
            xObject.PreviewKeyUp += tb_KeyDownPos_GameObject;
            yObject.PreviewKeyUp += tb_KeyDownPos_GameObject;

            xUI.PreviewKeyUp += tb_KeyDownPos_UI;
            yUI.PreviewKeyUp += tb_KeyDownPos_UI;

            textUI.PreviewKeyUp += tb_KeyDownText_UI;

            sizeObjectX.PreviewKeyUp += tb_KeyDownScale_GameObject;
            sizeObjectY.PreviewKeyUp += tb_KeyDownScale_GameObject;
            sizeUIX.PreviewKeyUp += tb_KeyDownScale_UI;
            sizeUIY.PreviewKeyUp += tb_KeyDownScale_UI;

            sortListEditor.DropDownClosed += cb_inEdit;

            sortList.DropDownClosed += (s, e) => {
                TypeUI typeUI = TypeUI.NONE;
                switch (sortList.SelectionBoxItem)
                {
                    case "звук":
                        typeUI = TypeUI.SOUND;
                        break;
                    case "текст":
                        typeUI = TypeUI.TEXT;
                        break;
                    case "кнопка":
                        typeUI = TypeUI.BUTTON;
                        break;
                    case "картинка":
                        typeUI = TypeUI.IMAGE;
                        break;
                    case "эффект":
                        typeUI = TypeUI.EFFECT;
                        break;
                    case "bossBar":
                        typeUI = TypeUI.BOSSBAR;
                        break;
                    case "скрипт":
                        typeUI = TypeUI.SCRIPT;
                        break;
                }
                //List<string> uiForList = new List<string>();
                if (!listNameUI.Any()) {
                    foreach (TextBlock uiName in listUI.Items)
                    {
                        listNameUI.Add(uiName.Text);
                        UI ui = findUI(uiName.Text);
                        bool hasElement = false;
                        foreach(string nameUI in listNameUI)
                        {
                            if (nameUI.Equals(uiName.Text))
                            {
                                hasElement = true;
                                break;
                            }
                        }
                        if (hasElement) continue;
                        if (typeUI.Equals(ui.TypeUI)) listNameUI.Add(uiName.Text);
                    }
                }
                listUI.Items.Clear();
                foreach (string nameUI in listNameUI)
                {
                    if (!findUI(nameUI).TypeUI.Equals(typeUI) && !typeUI.Equals(TypeUI.NONE)) continue;
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = nameUI;
                    textBlock.PreviewMouseDown += clickEvent_UI;

                    listUI.Items.Add(textBlock);
                }

            };

            physicsMass.PreviewKeyUp += (s, e) => {
                if (physicsMass.Text.Equals("")) return;
                try
                {
                    findGameObject(textBlockSelectNow.Text).Mass = float.Parse(physicsMass.Text, CultureInfo.InvariantCulture);
                }
                catch { }
            };

            rgbRInside.PreviewKeyUp += (s, e) => {
                if (rgbRInside.Text.Equals("")) return;
                try
                {
                    findUI(textBlockSelectNow.Text).GetRgbInside().R = byte.Parse(rgbRInside.Text, CultureInfo.InvariantCulture);
                }
                catch (FormatException ex) { };
            };

            rgbGInside.PreviewKeyUp += (s, e) => {
                if (rgbGInside.Text.Equals("")) return;
                try
                {
                    findUI(textBlockSelectNow.Text).GetRgbInside().G = byte.Parse(rgbGInside.Text, CultureInfo.InvariantCulture);
                }
                catch (FormatException ex) { };
            };

            rgbBInside.PreviewKeyUp += (s, e) => {
                if (rgbBInside.Text.Equals("")) return;
                try
                {
                    findUI(textBlockSelectNow.Text).GetRgbInside().B = byte.Parse(rgbBInside.Text, CultureInfo.InvariantCulture);
                }
                catch (FormatException ex) { };
            };
            

            rgbROutside.PreviewKeyUp += (s, e) => {
                if (rgbROutside.Text.Equals("")) return;
                try
                {
                    findUI(textBlockSelectNow.Text).GetRgbOutside().R = byte.Parse(rgbROutside.Text, CultureInfo.InvariantCulture);
                }
                catch (FormatException ex) { };
            };

            rgbGOutside.PreviewKeyUp += (s, e) => {
                if (rgbGOutside.Text.Equals("")) return;
                try
                {
                    findUI(textBlockSelectNow.Text).GetRgbOutside().G = byte.Parse(rgbGOutside.Text, CultureInfo.InvariantCulture);
                }
                catch (FormatException ex) { };
            };

            rgbBOutside.PreviewKeyUp += (s, e) => {
                if (rgbBOutside.Text.Equals("")) return;
                try
                {
                    findUI(textBlockSelectNow.Text).GetRgbOutside().B = byte.Parse(rgbBOutside.Text, CultureInfo.InvariantCulture);
                }
                catch (FormatException ex) { };
            };

            columns.PreviewKeyUp += (s, e) => {
                if (columns.Text.Equals("")) return;
                try
                {
                    findGameObject(textBlockSelectNow.Text).SetSplitSpriteSizeY(int.Parse(columns.Text));
                }
                catch (FormatException ex) { };
            };

            rows.PreviewKeyUp += (s, e) => {
                if (rows.Text.Equals("")) return;
            try
                {
                findGameObject(textBlockSelectNow.Text).SetSplitSpriteSizeX(int.Parse(rows.Text));
                }
                catch (FormatException ex) { };
            };

            columnPos.PreviewKeyUp += (s, e) => {
                if (columnPos.Text.Equals("")) return;
                try
                {
                    findGameObject(textBlockSelectNow.Text).SetSplitSpritePosY(int.Parse(columnPos.Text));
                }
                catch (FormatException ex) { };
            };

            rowPos.PreviewKeyUp += (s, e) => {
                if (rowPos.Text.Equals("")) return;
                try
                {
                    findGameObject(textBlockSelectNow.Text).SetSplitSpritePosX(int.Parse(rowPos.Text));
                }
                catch (FormatException ex) { };
            };

            animation.PreviewKeyUp += (s, e) => {
                if (rowPos.Text.Equals("")) return;
                findGameObject(textBlockSelectNow.Text).Animation = animation.Text;
            };

            countUI.PreviewKeyUp += (s, e) => {
                if (countUI.Text.Equals("")) return;
                try
                {
                    findUI(textBlockSelectNow.Text).CountParticle = int.Parse(countUI.Text);
                }
                catch (FormatException ex) { };
            };

            volume.PreviewKeyUp += (s, e) => {
                if (volume.Text.Equals("")) return;
                try
                {
                    findUI(textBlockSelectNow.Text).Volume = float.Parse(volume.Text, CultureInfo.InvariantCulture);
                }
                catch (FormatException ex) { };
            };

            angle.PreviewKeyUp += (s, e) => {
                try
                {
                    if (angle.Text.Equals("")) return;
                    findGameObject(textBlockSelectNow.Text).Angle = float.Parse(angle.Text, CultureInfo.InvariantCulture);

                    updateGameObjectsOnScene();
                }
                catch (FormatException ex) { };
            };

            rotateObject.PreviewKeyUp += (s, e) => {
                try
                {
                    if (rotateObject.Text.Equals("")) return;
                    findGameObject(textBlockSelectNow.Text).Rotate = float.Parse(rotateObject.Text, CultureInfo.InvariantCulture);

                    updateGameObjectsOnScene();
                }
                catch (FormatException ex) { };
            };

            tagObject.PreviewKeyUp += (s, e) => {
                if (tagObject.Text.Equals("")) return;
                findGameObject(textBlockSelectNow.Text).Tag = tagObject.Text;
            };

            sizeColliderX.PreviewKeyUp += (s, e) => {
                if (sizeColliderX.Text.Equals("")) return;
                try
                {
                    findGameObject(textBlockSelectNow.Text).SetSizeAdditonColliderX(int.Parse(sizeColliderX.Text));
                }
                catch (FormatException ex) { };
            };

            sizeColliderY.PreviewKeyUp += (s, e) => {
                if (sizeColliderX.Text.Equals("")) return;
                try
                {
                    findGameObject(textBlockSelectNow.Text).SetSizeAdditonColliderY(int.Parse(sizeColliderY.Text));
                }
                catch (FormatException ex) { };
            };

            posColliderX.PreviewKeyUp += (s, e) => {
                if (posColliderX.Text.Equals("")) return;
                try
                {
                    findGameObject(textBlockSelectNow.Text).SetPosAdditonColliderX(int.Parse(posColliderX.Text));
                }
                catch (FormatException ex) { };
            };

            posColliderY.PreviewKeyUp += (s, e) => {
                if (posColliderX.Text.Equals("")) return;
                try
                {
                    findGameObject(textBlockSelectNow.Text).SetPosAdditonColliderY(int.Parse(posColliderY.Text));
                }
                catch (FormatException ex) { };
            };

            layerScript.PreviewKeyUp += (s, e) => {
                if (layerScript.Text.Equals("")) return;
                try
                {
                    findUI(textBlockSelectNow.Text).Layer = int.Parse(layerScript.Text);
                    maxLayerScript = 0;
                    foreach (UI ui in GetScene().Uis)
                    {
                        if (ui.Layer > maxLayerScript) maxLayerScript = ui.Layer;
                    }
                }
                catch (FormatException ex) { };
            };

            layer.PreviewKeyUp += (s, e) => {
                if (layer.Text.Equals("")) return;
                try
                {
                    findGameObject(textBlockSelectNow.Text).Layer = int.Parse(layer.Text);
                    maxLayer = 0;
                    foreach (GameObject gameObject in GetScene().GameObjects)
                    {
                        if (gameObject.Layer > maxLayer) maxLayer = gameObject.Layer;
                    }
                }
                catch (FormatException ex) { };
            };


            listScenes.DropDownClosed += (s, e) => {
                for(int i = 0;i < scenes.Count;i++)
                {
                    if (listScenes.SelectionBoxItem.Equals(scenes[i].Name))
                    {
                        sceneSelectNow = i;
                        break;
                    }
                }

                UpdateScene();
                AddImageCamera();
                AddImageFrame();
            };


            listShapesCollider.DropDownClosed += (s, e) => {
                //Console.WriteLine(listShapesCollider.SelectionBoxItem);
                findGameObject(textBlockSelectNow.Text).TypeCollider = (string)listShapesCollider.SelectionBoxItem;
            };



            nameScene.PreviewKeyUp += (s, e) => {
                //ComboBoxItem comboBoxItem = (ComboBoxItem)listScenes.SelectedItem;
                //Console.WriteLine(listScenes.Text);
                if (nameScene.Text.Equals("") || e.Key.ToString().Equals("Right") || e.Key.ToString().Equals("Left") || e.Key.ToString().Equals("Up") || e.Key.ToString().Equals("Down")) return;
                
                listScenes.Items.Remove(listScenes.SelectedItem);
                listScenes.Items.Add(nameScene.Text);
                listScenes.SelectedItem = nameScene.Text;
                GetScene().Name = nameScene.Text;
                listScenes.Text = nameScene.Text;
                /*foreach (Scene scene in scenes)
                {
                    Console.WriteLine(scene.Name);
                }*/
            };

            //listScenes.Items.Add("ossss");
            //listScenes.SelectedItem = "ossss2";

            //Timer.Run(() => { updateGameObjectsOnScene(); }, 1);

            //updateGameObjectsOnScene();

            SetUIEditor(TypeUI.NONE);

            AddImageCamera();
            AddImageFrame();

            // if progect WEB
            if (typeProject.Equals(TypeProject.WEB)) {
                playGameButton.Visibility = Visibility.Hidden;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = System.Windows.Window.GetWindow(this);
            window.KeyDown += HandleKeyPress;
            window.KeyUp += HandleKeyRelised;
            window.MouseWheel += HandleMouseWheel;
            window.MouseMove += HandleMouseMove;
            window.MouseDown += HandleKeyPressMouse;
        }

        private void HandleKeyPressMouse(object sender, MouseButtonEventArgs e)
        {
            
            int i = 0;
            Point mouse = Mouse.GetPosition(System.Windows.Application.Current.MainWindow);
            float mousePosX = (float)(mouse.X + posCamera.X + 168);
            float mousePosY = (float)(mouse.Y + posCamera.Y + 51);
            foreach (UIElement child in canvas.Children)
            {
                try
                {
                    float posX = (float)(widthCanvas - GetScene().GameObjects[i].Position.X * zoomCamera - ((Image)child).Width + posCamera.X) / zoomCamera;
                    float posY = (float)(heightCanvas - GetScene().GameObjects[i].Position.Y * zoomCamera - ((Image)child).Height + posCamera.Y) / zoomCamera;
                    float width = (float)(GetScene().GameObjects[i].Size.X * zoomCamera);
                    float height = (float)(GetScene().GameObjects[i].Size.Y * zoomCamera);
                    //RotateTransform rotateTransform = new RotateTransform(GetScene().GameObjects[i].Rotate);
                    //rotateTransform.CenterX = GetScene().GameObjects[i].Size.X * zoomCamera / 2;
                    //rotateTransform.CenterY = GetScene().GameObjects[i].Size.Y * zoomCamera / 2;
                    //((Image)child).RenderTransform = rotateTransform;
                    //if (mousePosX > posX && mousePosX < posX + width &&
                    //        posY < mousePosY && posY + height > mousePosY) Console.WriteLine("asd");
                    //Console.WriteLine($"{mousePosX} > {posX} && {mousePosX} < {posX + width} && {posY} < {mousePosY} && {posY + height} > {mousePosY}");
                    //Console.WriteLine($"{posX} {posY} {width} {height} | {mousePosX} {mousePosY}");
                    i++;
                    continue;
                }
                catch { }
            }
                //if () Console.WriteLine("");

                updateGameObjectsOnScene();
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            //Console.WriteLine(e.Key);
            // camera move
            if (e.Key.ToString().Equals("LeftShift") || e.Key.ToString().Equals("RightShift")) shiftHold = true;
            if (e.Key.ToString().Equals("LeftCtrl") || e.Key.ToString().Equals("RightCtrl")) ctrlHold = true;

            float speed = 10 * (shiftHold ? 2 : 1) + (zoomCamera < 1 ? zoomCamera * 15 : zoomCamera / 2);
            if (e.Key.ToString().Equals("Right")) posCamera.X += speed;
            else if (e.Key.ToString().Equals("Left")) posCamera.X -= speed;
            else if (e.Key.ToString().Equals("Up")) posCamera.Y -= speed;
            else if (e.Key.ToString().Equals("Down")) posCamera.Y += speed;
            //Console.WriteLine(e.Key.ToString());

            updateGameObjectsOnScene();
        }

        /*private int rightButtonMouseX;
        private bool rightButtonMousePressed = false;*/
        private Vector lastPos = new Vector();

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            Point mouse = Mouse.GetPosition(System.Windows.Application.Current.MainWindow);
            
            if (e.RightButton.ToString().Equals("Pressed"))
            {
                
                float deltaX = (float)(mouse.X - lastPos.X);
                float deltaY = (float)(mouse.Y - lastPos.Y);
                
                //Console.WriteLine(posCamera.ToString());
                posCamera.X -= deltaX;
                posCamera.Y -= deltaY;
                
                /*var container = VisualTreeHelper.GetParent(this) as UIElement;
                var mousePosition = e.GetPosition(container);

                if (!rightButtonMousePressed)
                {
                    rightButtonMouseX = (int)mousePosition.X;
                }
                rightButtonMousePressed = true;

                if (rightButtonMouseX - mousePosition.X > 0) posCamera.X += 1;
                Console.WriteLine(rightButtonMouseX - mousePosition.X);
                Console.WriteLine(rightButtonMouseX);*/

                updateGameObjectsOnScene();
            }
            lastPos = new Vector(mouse.X, mouse.Y);
            /*if (e.RightButton.ToString().Equals("Pressed"))
            {
                var container = VisualTreeHelper.GetParent(this) as UIElement;
                var mousePosition = e.GetPosition(container);
                
                if (!rightButtonMousePressed)
                {
                    rightButtonMouseX = (int)mousePosition.X;
                }
                rightButtonMousePressed = true;

                if(rightButtonMouseX - mousePosition.X > 0) posCamera.X += 1;
                Console.WriteLine(rightButtonMouseX - mousePosition.X);
                Console.WriteLine(rightButtonMouseX);

                updateGameObjectsOnScene();
            }
            else
            {
                rightButtonMouseX = 0;
                rightButtonMousePressed = false;
            }*/
        }

        private void HandleKeyRelised(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Equals("LeftShift") || e.Key.ToString().Equals("RightShift")) shiftHold = false;
            if (e.Key.ToString().Equals("LeftCtrl") || e.Key.ToString().Equals("RightCtrl")) ctrlHold = false;
        }

        private void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //if (ctrlHold)
            //{
                float speedZoom = 0.07f;
                if (e.Delta < 0) zoomCamera -= speedZoom;
                else if (e.Delta > 0) zoomCamera += speedZoom;

                if (zoomCamera < 0.005f) zoomCamera = 0.005f;

                updateGameObjectsOnScene();
                return;
            //}

            /*float speedPos = 50;
            if (shiftHold) posCamera.Y += e.Delta < 0 ? speedPos : speedPos * -1;
            else posCamera.X += e.Delta < 0 ? speedPos : speedPos * -1;
            //else if (e.Delta > 0) zoomCamera += speedPos;

            if (zoomCamera < 0.1f) zoomCamera = 0.1f;*/

            updateGameObjectsOnScene();
        }

        private void buttonSelectRgbInside(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.AnyColor = true;
            colorDialog.ShowDialog();

            rgbRInside.Text = "" + colorDialog.Color.R;
            rgbGInside.Text = "" + colorDialog.Color.G;
            rgbBInside.Text = "" + colorDialog.Color.B;
            findUI(textBlockSelectNow.Text).SetRgbInside(new Rgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
        }

        private void buttonSelectRgbOutside(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.AnyColor = true;
            colorDialog.ShowDialog();

            rgbROutside.Text = "" + colorDialog.Color.R;
            rgbGOutside.Text = "" + colorDialog.Color.G;
            rgbBOutside.Text = "" + colorDialog.Color.B;
            findUI(textBlockSelectNow.Text).SetRgbOutside(new Rgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
        }

        private void UpdateScene()
        {
            //canvas.cle
            canvas.Children.Clear();
            listObjects.Items.Clear();
            listUI.Items.Clear();
            //AddImageCamera();
            foreach (GameObject gameObject in GetScene().GameObjects)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = gameObject.Name;
                textBlock.PreviewMouseDown += clickEvent_GameObject;

                listObjects.Items.Add(textBlock);
            }
            foreach (UI ui in GetScene().Uis)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = ui.Name;
                textBlock.PreviewMouseDown += clickEvent_UI;

                listUI.Items.Add(textBlock);
            }

            AddAllElementsOnScene();
            Thread.Sleep(1);
            updateGameObjectsOnScene();

            SetUIEditor(TypeUI.NONE);
        }

        private void AddAllElementsOnScene()
        {
            for (int i = 0; i < GetScene().GameObjects.Count; i++)
            {
                addGameObjectInScene(GetScene().GameObjects[i].Name);
            }

            for (int i = 0; i < GetScene().Uis.Count; i++)
            {
                addButtonInScene(GetScene().Uis[i]);
                addTextInScene(GetScene().Uis[i]);
                addImageInScene(GetScene().Uis[i]);
            }
        }

        private void cb_inEdit(object sender, EventArgs e)
        {
            TypeUI typeUI = TypeUI.SOUND;
            UI ui = findUI(textBlockSelectNow.Text);
            switch (sortListEditor.SelectionBoxItem)
            {
                case "звук":
                    //SetUIEditor(TypeUI.SOUND);
                    typeUI = TypeUI.SOUND;
                    break;
                case "текст":
                    typeUI = TypeUI.TEXT;
                    /*UI ui = new UI(nameUI.Text, TypeUI.TEXT);
                    uis.Add(ui);*/
                    //ui.TypeUI = TypeUI.TEXT;
                    addTextInScene(ui);
                    //SetUIEditor(TypeUI.TEXT);
                    break;
                case "кнопка":
                    typeUI = TypeUI.BUTTON;
                    //ui.TypeUI = TypeUI.BUTTON;
                    addButtonInScene(ui);
                    //SetUIEditor(TypeUI.BUTTON);
                    break;
                case "картинка":
                    typeUI = TypeUI.IMAGE;
                    //ui.TypeUI = TypeUI.IMAGE;
                    addImageInScene(ui);
                    //SetUIEditor(TypeUI.IMAGE);
                    //Console.WriteLine("new image");
                    break;
                case "эффект":
                    typeUI = TypeUI.EFFECT;
                    //ui.TypeUI = TypeUI.EFFECT;
                    //addImageInScene(findUI(textBlockSelectNow.Text));
                    //SetUIEditor(TypeUI.EFFECT);

                    BitmapImage bitmapImage = GetBitmapImageFromResourse("effect.png");
                    Image tem = new Image();
                    tem.Stretch = Stretch.Fill;
                    tem.Uid = ui.Name;
                    tem.Width = sizeEffectTexture;
                    tem.Height = sizeEffectTexture;
                    RenderOptions.SetBitmapScalingMode(tem, BitmapScalingMode.NearestNeighbor);
                    tem.Source = bitmapImage;

                    canvas.Children.Add(tem);
                    Canvas.SetBottom(tem, 335 - ui.Position.Y - tem.Height);
                    Canvas.SetRight(tem, 728 - ui.Position.X - tem.Width);
                    break;
                case "bossBar":
                    typeUI = TypeUI.BOSSBAR;
                    break;
                case "скрипт":
                    typeUI = TypeUI.SCRIPT;
                    if(typeProject.Equals(TypeProject.WEB)) ui.SourseScript = "assets\\" + ui.Name + ".js";
                    else ui.SourseScript = "assets\\" + ui.Name + ".lua";
                    if (!File.Exists(ui.SourseScript))
                    {
                        FileStream fileScript = new FileStream(ui.SourseScript, FileMode.CreateNew);
                        /*string text = "using Bubla;\r\n" +
                            "using System;\r\n" +
                            "" +
                            $"\r\npublic class {ui.Name.Replace(" ", "_")}\r\n" +
                            "{\r\n" +
                            "    public static void Start() {\r\n" +
                            "\n" +
                            "    }\r\n\r" +
                            "\n" +
                            "    public static void Update(float delta) {\r\n\r\n" +
                            "\n" +
                            "    }\r\n" +
                            "}";*/

                        string text =
                            "function Start()\r\n" +
                            "\n" +
                            "end\r\n" +
                            "\n" +
                            "function Update(delta)\r\n" +
                            "\n" +
                            "end";
                        if (typeProject.Equals(TypeProject.WEB)) text = "";
                        fileScript.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
                        fileScript.Close();
                        //byte[] textByte = text;
                        /*while((data = ) != -1)
                        {

                        }*/
                        /*File.Copy("resourse\\scriptDefault.cs", ui.SourseScript);
                        File.Open(ui.SourseScript, FileMode.Open);*/
                    }
                    else File.Open(ui.SourseScript, FileMode.Open).Close();
                    break;
            }
            ui.TypeUI = typeUI;
            SetUIEditor(typeUI);
            /*foreach(UI ui2 in GetScene().Uis)
            {
                Console.WriteLine(ui2.TypeUI);
            }*/
        }

        private void tb_KeyDownName_GameObject(object sender, KeyEventArgs e)
        {
            GameObject gameObject = findGameObject(textBlockSelectNow.Text);

            foreach (UIElement child in canvas.Children)
            {
                try
                {
                    if (((Image)child).Uid.Equals(gameObject.Name))
                    {
                        ((Image)child).Uid = nameObject.Text;
                        //Console.WriteLine(((Image)child).Uid);
                        break;
                    }
                }
                catch { };
            }

            gameObject.Name = nameObject.Text;
            textBlockSelectNow.Text = nameObject.Text;
        }

        private void tb_KeyDownName_UI(object sender, KeyEventArgs e)
        {
            UI ui = findUI(textBlockSelectNow.Text);

            if (ui.TypeUI.Equals(TypeUI.SOUND)) {
                try {
                    File.Move(ui.SourseSound, "assets\\" + nameUI.Text + ".ogg");
                    ui.SourseSound = "assets\\" + nameUI.Text + ".ogg";
                } catch (Exception ignored) { }
            }
            else if (ui.TypeUI.Equals(TypeUI.SCRIPT)) {
                try {
                    File.Move(ui.SourseScript, "assets\\" + nameUI.Text + ".cs");
                    ui.SourseScript = "assets\\" + nameUI.Text + ".cs";
                } catch (Exception ignored) { }
            }

            ui.Name = nameUI.Text;
            textBlockSelectNow.Text = nameUI.Text;

            if (!ui.TypeUI.Equals(TypeUI.BUTTON)) return;
            foreach (UIElement child in canvas.Children)
            {
                try
                {
                    if (((Image)child).Uid.Equals(ui.Name))
                    {
                        ((Image)child).Uid = nameObject.Text;
                        break;
                    }
                }
                catch { }
            }
        }

        private void tb_KeyDownPos_GameObject(object sender, KeyEventArgs e)
        {
            try
            {
                GameObject gameObject = findGameObject(textBlockSelectNow.Text);
                gameObject.SetPositionX(float.Parse(xObject.Text, CultureInfo.InvariantCulture));
                gameObject.SetPositionY(float.Parse(yObject.Text, CultureInfo.InvariantCulture));
                updateGameObjectsOnScene();
            } catch (FormatException ignored) { };
        }

        private void tb_KeyDownScale_GameObject(object sender, KeyEventArgs e)
        {
            try
            {
                GameObject gameObject = findGameObject(textBlockSelectNow.Text);
                gameObject.Size = new Vector(float.Parse(sizeObjectX.Text, CultureInfo.InvariantCulture), float.Parse(sizeObjectY.Text, CultureInfo.InvariantCulture));
                updateGameObjectsOnScene();
            } catch (FormatException ignored) { };
        }

        private void tb_KeyDownPos_UI(object sender, KeyEventArgs e)
        {
            try
            {
                UI ui = findUI(textBlockSelectNow.Text);
                ui.SetPositionX(float.Parse(xUI.Text, CultureInfo.InvariantCulture));
                ui.SetPositionY(float.Parse(yUI.Text, CultureInfo.InvariantCulture));
                updateGameObjectsOnScene();
            } catch (FormatException ignored) {  };
        }

        private void tb_KeyDownText_UI(object sender, KeyEventArgs e)
        {
            UI ui = findUI(textBlockSelectNow.Text);
            ui.Text = textUI.Text;
            updateGameObjectsOnScene();
        }

        private void tb_KeyDownScale_UI(object sender, KeyEventArgs e)
        {
            try
            {
                UI ui = findUI(textBlockSelectNow.Text);
                ui.Size = new Vector(float.Parse(sizeUIX.Text, CultureInfo.InvariantCulture), float.Parse(sizeUIY.Text, CultureInfo.InvariantCulture));
                updateGameObjectsOnScene();
            } catch (FormatException ignored) { };
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CheckBoxPoint(object sender, RoutedEventArgs e)
        {
            findGameObject(textBlockSelectNow.Text).Point = ((CheckBox)sender).IsChecked;

            updateGameObjectsOnScene();
        }

        private void CheckBoxGlobalScript(object sender, RoutedEventArgs e)
        {
            //findUI(textBlockSelectNow.Text).GlobalScript = ((CheckBox)sender).IsChecked;

            updateGameObjectsOnScene();
        }

        private void CheckBoxTrigger(object sender, RoutedEventArgs e)
        {
            findGameObject(textBlockSelectNow.Text).Trigger = ((CheckBox)sender).IsChecked;

            updateGameObjectsOnScene();
        }

        private void CheckBoxFixedRotation(object sender, RoutedEventArgs e)
        {
            findGameObject(textBlockSelectNow.Text).FixedRotation = ((CheckBox)sender).IsChecked;
        }

        private void CheckBoxFixedRotationTexture(object sender, RoutedEventArgs e)
        {
            findGameObject(textBlockSelectNow.Text).FixedRotationTexture = ((CheckBox)sender).IsChecked;
        }

        private void CheckBoxCollider(object sender, RoutedEventArgs e) 
        {
            findGameObject(textBlockSelectNow.Text).Collider = ((CheckBox)sender).IsChecked;
            //checkBoxPhysics.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            checkBoxTrigger.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            labelMass.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            physicsMass.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;

            updateGameObjectsOnScene();
        }

        private void CheckBoxSplitSprite(object sender, RoutedEventArgs e)
        {
            findGameObject(textBlockSelectNow.Text).SplitSprite = ((CheckBox)sender).IsChecked;
            //checkBoxPhysics.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            columnsLabel.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            columns.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            rows.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            rowsLabel.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;

            columnPosLabel.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            rowPosLabel.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            rowPos.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            columnPos.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
        }

        private void CheckBoxSplitSpritePixels(object sender, RoutedEventArgs e)
        {
            findGameObject(textBlockSelectNow.Text).SplitSpritePixel = ((CheckBox)sender).IsChecked;
            rowsLabel.Content = ((CheckBox)sender).IsChecked.Value ? "width" : "rows"; 
            columnsLabel.Content = ((CheckBox)sender).IsChecked.Value ? "height" : "columns";
        }

        private void CheckBoxLoop(object sender, RoutedEventArgs e)
        {
            findUI(textBlockSelectNow.Text).Loop = ((CheckBox)sender).IsChecked;
        }

        private void CheckBox3DSound(object sender, RoutedEventArgs e)
        {
            findUI(textBlockSelectNow.Text).Sound3D = ((CheckBox)sender).IsChecked;
        }

        /*private void CheckBoxPhysics(object sender, RoutedEventArgs e)
        {
            //findGameObject(textBlockSelectNow.Text).Physics = ((CheckBox)sender).IsChecked;
            labelMass.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            physicsMass.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Hidden;

            updateGameObjectsOnScene();
        }*/

        private void CheckBoxResourse(object sender, RoutedEventArgs e)
        {
            findGameObject(textBlockSelectNow.Text).Resourse = ((CheckBox)sender).IsChecked;

            updateGameObjectsOnScene();
        }

        private void CheckBoxRepited(object sender, RoutedEventArgs e)
        {
            findGameObject(textBlockSelectNow.Text).Repited = ((CheckBox)sender).IsChecked;
        }

        private void CheckBoxSmooth(object sender, RoutedEventArgs e)
        {
            findGameObject(textBlockSelectNow.Text).Smooth = ((CheckBox)sender).IsChecked;
        }

        private void addGameObjectInScene(string nameGameObject) {

            GameObject gameObject = findGameObject(nameGameObject);
            BitmapImage bitmapImage = GetBitmapImageFromResourse("noneTexture.png");
            try
            {
                if (gameObject.NameTexture != null) bitmapImage = GetBitmapImageFromAssets(gameObject.NameTexture);
            }
            catch { return; }
            Image tem = new Image();
            tem.Stretch = Stretch.Fill;
            tem.Uid = gameObject.Name;
            RenderOptions.SetBitmapScalingMode(tem, BitmapScalingMode.NearestNeighbor);
            tem.Source = bitmapImage;
            canvas.Children.Add(tem);
            Canvas.SetBottom(tem, 335 - gameObject.Position.Y - tem.Height);
            Canvas.SetRight(tem, 728 - gameObject.Position.X - tem.Width);
            /*tem.MouseEnter += (s, e) => {
                Console.WriteLine("asdkjhgsdfa");
                Console.WriteLine(tem.Uid);
            };*/
        }

        private void addButtonInScene(UI ui)
        {
            if (!ui.TypeUI.Equals(TypeUI.BUTTON)) return;
            BitmapImage bitmapImage = GetBitmapImageFromResourse("noneTexture.png");
            if (ui.NameTexture != "")
                bitmapImage = GetBitmapImageFromAssets(ui.NameTexture);
            Image tem = new Image();
            tem.Stretch = Stretch.Fill;
            tem.Uid = ui.Name;
            RenderOptions.SetBitmapScalingMode(tem, BitmapScalingMode.NearestNeighbor);
            tem.Source = bitmapImage;

            canvas.Children.Add(tem);
            Canvas.SetBottom(tem, 335 - ui.Position.Y - tem.Height);
            Canvas.SetRight(tem, 728 - ui.Position.X - tem.Width);
        }

        private void addImageInScene(UI ui)
        {
            if (!ui.TypeUI.Equals(TypeUI.IMAGE)) return;
            BitmapImage bitmapImage = GetBitmapImageFromResourse("noneTexture.png");
            if (ui.NameTexture != "")
                bitmapImage = GetBitmapImageFromAssets(ui.NameTexture);
            Image tem = new Image();
            tem.Stretch = Stretch.Fill;
            tem.Uid = ui.Name;
            RenderOptions.SetBitmapScalingMode(tem, BitmapScalingMode.NearestNeighbor);
            tem.Source = bitmapImage;

            canvas.Children.Add(tem);
            Canvas.SetBottom(tem, 335 - ui.Position.Y - tem.Height);
            Canvas.SetRight(tem, 728 - ui.Position.X - tem.Width);
        }

        private void addTextInScene(UI ui)
        {
            if (!ui.TypeUI.Equals(TypeUI.TEXT)) return;

            Label label = new Label();
            label.Content = ui.Text;
            label.FontSize = ui.Size.X == 0 ? 20 : ui.Size.X;
            label.Margin = new Thickness(0, 0, 0, 0);
            //label.FontFamily = new FontFamily(GetBitmapImageFromAssets("font.ttf").ToString());
            label.Uid = ui.Name;

            canvas.Children.Add(label);
        }

        private void buttonAddGameObject(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "обьект " + GetScene().GameObjects.Count;
            textBlock.PreviewMouseDown += clickEvent_GameObject;

            listObjects.Items.Add(textBlock);
            GetScene().GameObjects.Add(new GameObject(textBlock.Text));

            addGameObjectInScene(textBlock.Text);
        }

        GameObject cloneGameObject = null;
        UI cloneUI = null;

        private void ButtonPaste(object sender, RoutedEventArgs e)
        {
            if (cloneGameObject != null)
            {
                TextBlock textBlock = new TextBlock();
                cloneGameObject.Name = cloneGameObject.Name + new Random().Next(999999);
                textBlock.Text = cloneGameObject.Name;
                textBlock.PreviewMouseDown += clickEvent_GameObject;

                listObjects.Items.Add(textBlock);
                GetScene().GameObjects.Add(cloneGameObject);
                listShapesCollider.Text = cloneGameObject.TypeCollider;
                Console.WriteLine(cloneGameObject.TypeCollider);

                addGameObjectInScene(textBlock.Text);
            } else if(cloneUI != null)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = cloneUI.Name;
                textBlock.PreviewMouseDown += clickEvent_UI;

                listUI.Items.Add(textBlock);
                GetScene().Uis.Add(cloneUI);
            }
        }

        private void ButtonCloneObject(object sender, RoutedEventArgs e)
        {
            this.cloneGameObject = findGameObject(textBlockSelectNow.Text).Clone();
            cloneUI = null;
        }

        private void ButtonCloneUI(object sender, RoutedEventArgs e)
        {
            this.cloneUI = findUI(textBlockSelectNow.Text).Clone();
            cloneGameObject = null;
        }

        private void buttonAddUI(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "ui " + GetScene().Uis.Count;
            textBlock.PreviewMouseDown += clickEvent_UI;

            listUI.Items.Add(textBlock);
            GetScene().Uis.Add(new UI(textBlock.Text, TypeUI.SOUND));
            //listNameUI.Add(textBlock.Text);
            //исправить баг с удалением обьекта при сортировке
        }

        private void buttonResetZoomCamera(object sender, RoutedEventArgs e)
        {
            zoomCamera = 1;

            updateGameObjectsOnScene();
        }

        private void buttonUpdateLayer(object sender, RoutedEventArgs e)
        {
            /*List<UIElement> uiDelete = new List<UIElement>();
            foreach (UIElement child in canvas.Children)
            {
                if (child.Uid.Equals("sceneCamView") || child.Uid.Equals("frameTexture")) continue;
                foreach (GameObject gameObject in GetScene().GameObjects)
                {
                    if (gameObject.Name.Equals(child.Uid)) {
                        uiDelete.Add(child);
                        //canvas.Children.Remove(child);
                        break;
                    }
                }
            }
            foreach (UIElement uIElement in uiDelete)
            {
                canvas.Children.Remove(uIElement);
            }*/
                canvas.Children.Clear();

                // add all gameObjects
                List<GameObject> listObjects = new List<GameObject>();
            int layer = 0;
            while (true)
            {
                foreach (GameObject gameObject in GetScene().GameObjects)
                {
                    if (gameObject.Layer == layer)
                    {
                        //Console.WriteLine($"load gameObject name = {gameObject.Name}");
                        BitmapImage bitmapImage = GetBitmapImageFromAssets(gameObject.NameTexture);
                        Image tem = new Image();
                        tem.Stretch = Stretch.Fill;
                        tem.Uid = gameObject.Name;
                        RenderOptions.SetBitmapScalingMode(tem, BitmapScalingMode.NearestNeighbor);
                        tem.Source = bitmapImage;

                        canvas.Children.Add(tem);
                        Canvas.SetBottom(tem, widthCanvas - tem.Height);
                        Canvas.SetRight(tem, heightCanvas - tem.Width);

                        listObjects.Add(gameObject);
                    }
                }
                if (layer == maxLayer) break;
                layer++;
            }
            GetScene().GameObjects = listObjects;

            updateGameObjectsOnScene();
        }

        private void buttonResetPosCamera(object sender, RoutedEventArgs e)
        {
            posCamera.X = 0;
            posCamera.Y = 0;

            updateGameObjectsOnScene();
        }

        /*private void buttonColorTrigger(object sender, RoutedEventArgs e)
        {
            asd
        }*/

        private void clickEvent_GameObject(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            textBlockSelectNow = textBlock;
            GameObject gameObjectNow = findGameObject(textBlock.Text);
            nameObject.Text = textBlock.Text;
            tagObject.Text = gameObjectNow.Tag;
            checkBoxPoint.IsChecked = gameObjectNow.Point;
            checkBoxResourse.IsChecked = gameObjectNow.Resourse;
            checkBoxRepited.IsChecked = gameObjectNow.Repited;
            checkBoxSmooth.IsChecked = gameObjectNow.Smooth;
            checkBoxCollider.IsChecked = gameObjectNow.Collider;
            checkBoxFixedRotation.IsChecked = gameObjectNow.FixedRotation;
            checkBoxFixedRotationTexture.IsChecked = gameObjectNow.FixedRotationTexture;
            checkBoxSplitSprite.IsChecked = gameObjectNow.SplitSprite;
            checkBoxSplitSpritePixels.IsChecked = gameObjectNow.SplitSpritePixel;
            checkBoxTrigger.IsChecked = gameObjectNow.Trigger;
            rowsLabel.Content = checkBoxSplitSpritePixels.IsChecked.Value ? "width" : "rows";
            columnsLabel.Content = checkBoxSplitSpritePixels.IsChecked.Value ? "height" : "columns";
            //Console.WriteLine("asd");
            columnsLabel.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            columns.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            rows.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            labelAnimation.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            animation.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            rowsLabel.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            columnPosLabel.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            columnPos.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            rowPos.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            rowPosLabel.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            checkBoxSplitSpritePixels.Visibility = checkBoxSplitSprite.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            //checkBoxPhysics.IsChecked = gameObjectNow.Physics;
            //checkBoxPhysics.Visibility = checkBoxCollider.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;

            checkBoxTrigger.Visibility = !checkBoxCollider.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            labelMass.Visibility = checkBoxCollider.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            physicsMass.Visibility = checkBoxCollider.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            sizeColliderX.Visibility = checkBoxCollider.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            sizeColliderY.Visibility = checkBoxCollider.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            posColliderX.Visibility = checkBoxCollider.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            posColliderY.Visibility = checkBoxCollider.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            labelSizeCollider.Visibility = checkBoxCollider.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            labelPosCollider.Visibility = checkBoxCollider.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;

            yObject.Text = gameObjectNow.Position.Y.ToString().Replace(",", ".");
            xObject.Text = gameObjectNow.Position.X.ToString().Replace(",", ".");
            sizeObjectX.Text = gameObjectNow.Size.X.ToString().Replace(",", ".");
            sizeObjectY.Text = gameObjectNow.Size.Y.ToString().Replace(",", ".");
            sizeColliderX.Text = gameObjectNow.SizeAdditionCollider.X.ToString();
            sizeColliderY.Text = gameObjectNow.SizeAdditionCollider.Y.ToString();
            posColliderX.Text = gameObjectNow.PosAdditionCollider.X.ToString();
            posColliderY.Text = gameObjectNow.PosAdditionCollider.Y.ToString();
            physicsMass.Text = gameObjectNow.Mass.ToString();
            angle.Text = gameObjectNow.Angle.ToString();
            listShapesCollider.Text = gameObjectNow.TypeCollider;
            animation.Text = gameObjectNow.Animation;
            columns.Text = gameObjectNow.SplitSpriteSize.Y.ToString();
            rows.Text = gameObjectNow.SplitSpriteSize.X.ToString();
            columnPos.Text = gameObjectNow.SplitSpritePos.Y.ToString();
            rowPos.Text = gameObjectNow.SplitSpritePos.X.ToString();
            layer.Text = gameObjectNow.Layer.ToString();
            if (gameObjectNow.NameTexture != null && gameObjectNow.NameTexture != "")
                imageObject.Source = GetBitmapImageFromAssets(gameObjectNow.NameTexture);
            else imageObject.Source = null;

            // update cam
            if (gameObjectNow.NameTexture != null && teleportThenClick.IsChecked.Value) {
                posCamera.X = gameObjectNow.Position.X * zoomCamera - widthCanvas / 2 + gameObjectNow.Size.X / 2;
                posCamera.Y = gameObjectNow.Position.Y * zoomCamera - heightCanvas + gameObjectNow.Size.Y / 2;
            }
            SetVisibleEditor(Visibility.Visible, VisibilityEditor.Object);
            SetVisibleEditor(Visibility.Hidden, VisibilityEditor.Ui);

            updateGameObjectsOnScene();
        }

        private void clickEvent_UI(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            textBlockSelectNow = textBlock;
            UI uiNow = findUI(textBlock.Text);
            nameUI.Text = textBlock.Text;
            yUI.Text = uiNow.Position.Y.ToString().Replace(",", ".");
            xUI.Text = uiNow.Position.X.ToString().Replace(",", ".");
            sizeUIX.Text = uiNow.Size.X.ToString().Replace(",", ".");
            sizeUIY.Text = uiNow.Size.X.ToString().Replace(",", ".");
            textUI.Text = uiNow.Text;
            countUI.Text = uiNow.CountParticle.ToString().Replace(",", ".");
            rgbRInside.Text = "" + uiNow.GetRgbInside().R;
            //Console.WriteLine(uiNow.GetRgbInside().R);
            rgbGInside.Text = "" + uiNow.GetRgbInside().G;
            rgbBInside.Text = "" + uiNow.GetRgbInside().B;
            rgbROutside.Text = "" + uiNow.GetRgbOutside().R;
            rgbGOutside.Text = "" + uiNow.GetRgbOutside().G;
            rgbBOutside.Text = "" + uiNow.GetRgbOutside().B;
            volume.Text = "" + uiNow.Volume;
            layerScript.Text = uiNow.Layer.ToString();
            checkBoxLoop.IsChecked = uiNow.Loop;
            checkBox3DSound.IsChecked = uiNow.Sound3D;
            layerScript.Text = uiNow.Layer.ToString();
            int index = 0;
            int indexEffect = 0;
            switch(uiNow.TypeUI)
            {
                case TypeUI.SCRIPT:
                    index = 6;
                    break;
                case TypeUI.BOSSBAR:
                    index = 5;
                    break;
                case TypeUI.EFFECT:
                    index = 4;
                    switch (uiNow.TypeEffect)
                    {
                        case TypeEffect.PULSE:
                            indexEffect = 0;
                            SetImageUI(uiNow);
                            break;
                    }
                    SetImageUI(uiNow);
                    break;
                case TypeUI.IMAGE:
                    index = 3;
                    SetImageUI(uiNow);
                    break;
                case TypeUI.BUTTON:
                    index = 2;
                    SetImageUI(uiNow);
                    break;
                case TypeUI.TEXT:
                    index = 1;
                    break;
                case TypeUI.SOUND:
                    index = 0;
                    break;
            }
            SetVisibleEditor(Visibility.Visible, VisibilityEditor.Ui);
            SetVisibleEditor(Visibility.Hidden, VisibilityEditor.Object);
            SetUIEditor(uiNow.TypeUI);
            sortListEditor.SelectedIndex = index;
            typeEffect.SelectedIndex = indexEffect;

            // update cam
            if (uiNow.NameTexture != null && (index == 4 || index == 3 || index == 2 || index == 1))
            {
                posCamera.X = uiNow.Position.X * zoomCamera - widthCanvas / 2 + uiNow.Size.X / 2;
                posCamera.Y = uiNow.Position.Y * zoomCamera - heightCanvas / 2 + uiNow.Size.Y / 2;
            }

            //cb_inEdit(null, null);
            /*if (uiNow.NameTexture != null && uiNow.NameTexture != "")
                imageObject.Source = GetBitmapImageFromAssets(uiNow.NameTexture);
            else imageObject.Source = null;*/

            updateGameObjectsOnScene();
        }

        private void buttonAddTexture(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialoge = new OpenFileDialog();
            fileDialoge.ShowDialog();
            if (fileDialoge.FileName == null || fileDialoge.FileName == "") return;
            if(!File.Exists("assets\\" + fileDialoge.SafeFileName)) File.Copy(fileDialoge.FileName, "assets\\" + fileDialoge.SafeFileName);

            GameObject gameObject = findGameObject(textBlockSelectNow.Text);
            gameObject.NameTexture = fileDialoge.SafeFileName;
            imageObject.Source = new BitmapImage(new Uri(fileDialoge.FileName));

            foreach (UIElement child in canvas.Children)
            {
                //Console.WriteLine(((Image)child).Source);
                try
                {
                    if (((Image)child).Uid.Equals(gameObject.Name))
                    {
                        ((Image)child).Source = GetBitmapImageFromAssets(gameObject.NameTexture);
                        break;
                    }
                }
                catch { };
            }

            updateGameObjectsOnScene();
        }

        private void buttonAddTextureUI(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialoge = new OpenFileDialog();
            fileDialoge.ShowDialog();
            if (fileDialoge.FileName == null || fileDialoge.FileName == "") return;
            if(!File.Exists("assets\\" + fileDialoge.SafeFileName)) File.Copy(fileDialoge.FileName, "assets\\" + fileDialoge.SafeFileName);

            UI ui = findUI(textBlockSelectNow.Text);
            ui.NameTexture = fileDialoge.SafeFileName;
            imageUI.Source = new BitmapImage(new Uri(fileDialoge.FileName));

            foreach (UIElement child in canvas.Children)
            {
                if (child is Label) continue;
                if(((Image)child).Uid.Equals(ui.Name)) {
                    ((Image)child).Source = GetBitmapImageFromAssets(ui.NameTexture);
                    break;
                }
            }

            updateGameObjectsOnScene();
        }

        private void buttonDeleteObject(object sender, RoutedEventArgs e)
        {
            GameObject gameObject = findGameObject(textBlockSelectNow.Text);

            foreach (UIElement child in canvas.Children)
            {
                if (((Image)child).Uid.Equals(gameObject.Name))
                {
                    canvas.Children.Remove(child);
                    break;
                }
            }

            GetScene().GameObjects.Remove(gameObject);
            listObjects.Items.Remove(textBlockSelectNow);
            textBlockSelectNow = null;

            SetVisibleEditor(Visibility.Hidden, VisibilityEditor.Object);
            /*MessageBox.Show(
                "Выберите один из вариантов",
                "Сообщение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information,
                MessageBoxResult.Yes,
                MessageBoxOptions.DefaultDesktopOnly);*/

            /*OpenFileDialog fileDialoge = new OpenFileDialog();
            fileDialoge.ShowDialog();
            if (fileDialoge.FileName == null || fileDialoge.FileName == "") return;
            if(!File.Exists("assets\\" + fileDialoge.SafeFileName)) File.Copy(fileDialoge.FileName, "assets\\" + fileDialoge.SafeFileName);

            findGameObject(textBlockSelectNow.Text).NameTexture 
                = fileDialoge.SafeFileName;
            imageObject.Source = new BitmapImage(new Uri(fileDialoge.FileName));*/
        }

        private void ButtonDeleteUI(object sender, RoutedEventArgs e)
        {
            UI ui = findUI(textBlockSelectNow.Text);

            foreach (UIElement child in canvas.Children)
            {
                try
                {
                    if (((Image)child).Uid.Equals(ui.Name))
                    {
                        canvas.Children.Remove(child);
                        break;
                    }
                }
                catch (InvalidCastException ex) { }
            }

            // delete from assets
            if (ui.SourseSound != "") File.Delete(ui.SourseSound);
            else if (ui.SourseScript != "") File.Delete(ui.SourseScript);
            /*else
            {
                Console.WriteLine($"ui.NameTexture = {ui.NameTexture}");
                bool otherUIUseImage = false;
                foreach (UI _ui in GetScene().Uis)
                {
                    if (_ui.Name.Equals(ui.Name)) continue;
                    if (_ui.NameTexture.Equals(ui.NameTexture)) otherUIUseImage = true;
                }
                Console.WriteLine(otherUIUseImage);
                if (!otherUIUseImage) File.Delete($"assets\\{ui.NameTexture}");
            }*/

            GetScene().Uis.Remove(ui);
            listUI.Items.Remove(textBlockSelectNow);
            textBlockSelectNow = null;
            SetVisibleEditor(Visibility.Hidden, VisibilityEditor.Ui);
            UpdateScene();
        }

        private void buttonSave(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                loadAllFile();
                saveXmlInfoGame();
                saveXmlInfoGameEngine();
            }

            string nameGame = Option.TitleGame.Replace(' ', '_');
            string pathLaba1 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + nameGame;
            //Console.WriteLine(pathLaba1);
            if (Directory.Exists(pathLaba1)) Directory.Delete(pathLaba1, true); // delete file save.bubla
            //loadAllFile();
            //updateGameObjectsOnScene();
        }

        private const int step = 100;

        private void buttonXUp(object sender, RoutedEventArgs e)
        {
            int editValue = int.Parse(xObject.Text) + step * (shiftHold ? 2 : 1);
            xObject.Text = "" + editValue;
            findGameObject(textBlockSelectNow.Text).SetPositionX(editValue);
            updateGameObjectsOnScene();
        }

        private void buttonXDown(object sender, RoutedEventArgs e)
        {
            int editValue = int.Parse(xObject.Text) - step * (shiftHold ? 2 : 1);
            xObject.Text = "" + editValue;
            findGameObject(textBlockSelectNow.Text).SetPositionX(editValue);
            updateGameObjectsOnScene();
        }

        private void buttonYUp(object sender, RoutedEventArgs e)
        {
            int editValue = int.Parse(yObject.Text) + step * (shiftHold ? 2 : 1);
            yObject.Text = "" + editValue;
            findGameObject(textBlockSelectNow.Text).SetPositionY(editValue);
            updateGameObjectsOnScene();
        }

        private void buttonYDown(object sender, RoutedEventArgs e)
        {
            int editValue = int.Parse(yObject.Text) - step * (shiftHold ? 2 : 1);
            yObject.Text = "" + editValue;
            findGameObject(textBlockSelectNow.Text).SetPositionY(editValue);
            updateGameObjectsOnScene();
        }

        private void buttonOption(object sender, RoutedEventArgs e)
        {
            windowOption.Show();
            windowOption.WhenOpen();
        }

        private void findSoundFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialoge = new OpenFileDialog();
            fileDialoge.ShowDialog();
            if (fileDialoge.FileName == null || fileDialoge.FileName == "") return;
            if (File.Exists("assets\\" + nameUI.Text + ".ogg")) File.Delete("assets\\" + nameUI.Text + ".ogg");
            File.Copy(fileDialoge.FileName, "assets\\" + nameUI.Text + ".ogg");

            findUI(textBlockSelectNow.Text).SourseSound = "assets\\" + nameUI.Text + ".ogg";
        }

        private void buttonOpenScript(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(findUI(textBlockSelectNow.Text).SourseScript);
        }

        /*private void playSound(object sender, RoutedEventArgs e)
        {
            //findUI(textBlockSelectNow.Text).SourseSound;
        }*/


        /*private void buttonOpenScript(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "assets\\" + nameUI + ".cs"))
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "resourse\\scriptDefault.txt",
                    AppDomain.CurrentDomain.BaseDirectory + "assets\\" + nameUI + ".txt");
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "assets\\" + nameUI + ".cs");
        }*/

        private void buttonAddScene(object sender, RoutedEventArgs e)
        {
            Scene scene = new Scene("scene" + scenes.Count);
            listScenes.Items.Add(scene.Name);
            listScenes.SelectedItem = scene.Name;
            scenes.Add(scene);
        }

        private void buttonPlayGame(object sender, RoutedEventArgs e)
        {
            //loadAllFile();

            //saveXmlInfoGame();
            //saveXmlInfoGameEngine();

            //Thread.Sleep(3);

            //Console.OutputEncoding = Encoding.UTF8;

            Process myProcess = new Process();
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.Arguments = @"/C cd " + Option.PathStartFile.Replace("\\FrameworkEngine.exe", "") + " & FrameworkEngine.exe";
            //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //myProcess.StartInfo.CreateNoWindow = true;
            myProcess.Start();
            myProcess.WaitForExit();

            /*System.Diagnostics.Process.Start(Option.PathStartFile);
            */

            updateGameObjectsOnScene();
        }

        private GameObject findGameObject(string name)
        {
            foreach (GameObject gameObject in GetScene().GameObjects)
            {
                
                if (gameObject.Name.Equals(name))
                {
                    //Console.WriteLine(gameObject.Name + " == " + name);
                    return gameObject;
                }
            }
            return null;
        }

        private UI findUI(string name)
        {
            foreach (UI ui in GetScene().Uis)
            {
                if (ui.Name.Equals(name))
                {
                    return ui;
                }
            }
            return null;
        }

        private void saveXmlInfoGame()
        {
            XDocument xdoc = new XDocument();
            XElement gameObjectsElement = new XElement("game");
            gameObjectsElement.Add(new XElement("title", Option.TitleGame));
            gameObjectsElement.Add(new XElement("colorBackgroundR", Option.ColorBackground[0]));
            gameObjectsElement.Add(new XElement("colorBackgroundG", Option.ColorBackground[1]));
            gameObjectsElement.Add(new XElement("colorBackgroundB", Option.ColorBackground[2]));
            gameObjectsElement.Add(new XElement("widhtScreen", Option.WidhtScreen));
            gameObjectsElement.Add(new XElement("heightScreen", Option.HeightScreen));
            gameObjectsElement.Add(new XElement("fullScreen", Option.DebugMode));
            gameObjectsElement.Add(new XElement("deviceConnect", Option.DeviceConnect));
            gameObjectsElement.Add(new XElement("maxLayer", maxLayer));
            gameObjectsElement.Add(new XElement("nameSceneStart", Option.NameSceneStart));
            gameObjectsElement.Add(new XElement("sizeMouseTexture", Option.SizeMouseTexture));
            gameObjectsElement.Add(new XElement("mouseInGame", Option.MouseInGame));

            
            foreach (Scene scene in scenes) {
                //string luaText = "";
                List<UI> luaScripts = new List<UI>();

                XElement xScene = new XElement("scene");
                XAttribute nameScene = new XAttribute("name", scene.Name);
                xScene.Add(nameScene);
                //Console.WriteLine(scene.Name);

                foreach (GameObject gameObject in scene.GameObjects)
                {
                    XElement gameObjectElement = new XElement("gameObject");
                    XAttribute name = new XAttribute("name", gameObject.Name);
                    XAttribute layer = new XAttribute("layer", gameObject.Layer);

                    XElement gameObjectPositionX = new XElement("x", gameObject.Position.X);
                    XElement gameObjectTag = new XElement("tag", gameObject.Tag);
                    XElement gameObjectPositionY = new XElement("y", gameObject.Position.Y);
                    XElement gameObjectPositionSizeX = new XElement("sizeX", gameObject.Size.X);
                    XElement gameObjectPositionSizeY = new XElement("sizeY", gameObject.Size.Y);
                    XElement gameObjectRotate = new XElement("rotate", gameObject.Rotate);
                    XElement gameObjectPathTexture = null;
                    XElement gameObjectPathTextureEncript = null;
                    try
                    {
                        gameObjectPathTexture = new XElement("nameTexture", RemoveEndNameFile(gameObject.NameTexture));
                        //gameObjectPathTextureEncript = new XElement("nameTextureEncript", StringToByteToString(gameObject.NameTexture) + ".bubla");
                    }
                    catch (NullReferenceException e) { Console.WriteLine($"вы не указали текстуру для обьекта {gameObject.Name}"); }
                    XElement gameObjectPoint = new XElement("point", gameObject.Point);
                    XElement gameObjectResourse = new XElement("resourse", gameObject.Resourse);
                    XElement gameObjectSmooth = new XElement("smooth", gameObject.Smooth);
                    XElement gameObjectRepited = new XElement("repited", gameObject.Repited);

                    XElement gameObjectCollider = new XElement("collider", gameObject.Collider);
                    XElement gameObjectTrigger = new XElement("trigger", gameObject.Trigger);
                    XElement gameObjectFixedRotation = new XElement("fixedRotation", gameObject.FixedRotation);
                    XElement gameObjectFixedRotationTexture = new XElement("fixedRotationTexture", gameObject.FixedRotationTexture);
                    XElement gameObjectMass = new XElement("mass", gameObject.Mass);
                    XElement gameObjectSizeAddColliderX = new XElement("sizeAddColliderX", gameObject.SizeAdditionCollider.X);
                    XElement gameObjectSizeAddColliderY = new XElement("sizeAddColliderY", gameObject.SizeAdditionCollider.Y);
                    XElement gameObjectPosAddColliderX = new XElement("posAddColliderX", gameObject.PosAdditionCollider.X);
                    XElement gameObjectPosAddColliderY = new XElement("posAddColliderY", gameObject.PosAdditionCollider.Y);
                    XElement gameObjectTypeCollider = new XElement("typeCollider", gameObject.TypeCollider);

                    XElement gameObjectSplitSprite = new XElement("splitSprite", gameObject.SplitSprite);
                    XElement gameObjectSplitSpritePixel = new XElement("splitSpritePixel", gameObject.SplitSpritePixel);
                    XElement gameObjectAngle = new XElement("angle", gameObject.Angle);
                    XElement gameObjectColumns = new XElement("splitSpriteColumns", gameObject.SplitSpriteSize.Y);
                    XElement gameObjectRows = new XElement("splitSpriteRows", gameObject.SplitSpriteSize.X);
                    XElement gameObjectColumnPos = new XElement("splitSpriteColumnPos", gameObject.SplitSpritePos.Y);
                    XElement gameObjectRowPos = new XElement("splitSpriteRowPos", gameObject.SplitSpritePos.X);
                    XElement gameObjectAnimation = new XElement("animation", gameObject.Animation);

                    gameObjectElement.Add(name);
                    gameObjectElement.Add(layer);
                    gameObjectElement.Add(gameObjectPositionX);
                    gameObjectElement.Add(gameObjectTag);
                    gameObjectElement.Add(gameObjectPositionY);
                    gameObjectElement.Add(gameObjectPositionSizeX);
                    gameObjectElement.Add(gameObjectPositionSizeY);
                    gameObjectElement.Add(gameObjectRotate);
                    if (gameObjectPathTexture != null) gameObjectElement.Add(gameObjectPathTexture);
                    if (gameObjectPathTextureEncript != null) gameObjectElement.Add(gameObjectPathTextureEncript);
                    gameObjectElement.Add(gameObjectPoint);
                    gameObjectElement.Add(gameObjectResourse);
                    gameObjectElement.Add(gameObjectSmooth);
                    gameObjectElement.Add(gameObjectRepited);

                    gameObjectElement.Add(gameObjectCollider);
                    gameObjectElement.Add(gameObjectTrigger);
                    gameObjectElement.Add(gameObjectFixedRotation);
                    gameObjectElement.Add(gameObjectFixedRotationTexture);

                    gameObjectElement.Add(gameObjectAnimation);
                    gameObjectElement.Add(gameObjectMass);
                    gameObjectElement.Add(gameObjectSizeAddColliderX);
                    gameObjectElement.Add(gameObjectSizeAddColliderY);
                    gameObjectElement.Add(gameObjectPosAddColliderX);
                    gameObjectElement.Add(gameObjectPosAddColliderY);
                    gameObjectElement.Add(gameObjectAngle);
                    gameObjectElement.Add(gameObjectTypeCollider);

                    gameObjectElement.Add(gameObjectSplitSprite);
                    gameObjectElement.Add(gameObjectSplitSpritePixel);
                    gameObjectElement.Add(gameObjectColumns);
                    gameObjectElement.Add(gameObjectRows);
                    gameObjectElement.Add(gameObjectColumnPos);
                    gameObjectElement.Add(gameObjectRowPos);

                    xScene.Add(gameObjectElement);
                }
                foreach (UI ui in scene.Uis)
                {
                    if (ui.TypeUI.Equals(TypeUI.SOUND))
                    {
                        XElement gameObjectElement = new XElement("sound");
                        XAttribute name = new XAttribute("name", ui.Name);

                        XElement loop = new XElement("loop", ui.Loop);
                        XElement sound3D = new XElement("sound3D", ui.Sound3D);
                        XElement volume = new XElement("volume", ui.Volume);
                        XElement path = new XElement("path", RemoveEndNameFile(ui.SourseSound));
                        XElement pathEncript = new XElement("pathEncript", StringToByteToString(ui.SourseSound) + ".bubla");

                        gameObjectElement.Add(name);
                        gameObjectElement.Add(loop);
                        gameObjectElement.Add(sound3D);
                        gameObjectElement.Add(volume);
                        gameObjectElement.Add(path);
                        //gameObjectElement.Add(pathEncript);

                        xScene.Add(gameObjectElement);
                    }
                    else if (ui.TypeUI.Equals(TypeUI.BUTTON))
                    {
                        XElement gameObjectElement = new XElement("button");
                        XAttribute name = new XAttribute("name", ui.Name);

                        XElement gameObjectPositionPathTexture = new XElement("nameTexture", RemoveEndNameFile(ui.NameTexture));
                        XElement gameObjectPositionPathTextureEncript = new XElement("nameTextureEncript", StringToByteToString(ui.NameTexture) + ".bubla");
                        XElement gameObjectPositionPosX = new XElement("x", ui.Position.X);
                        XElement gameObjectPositionPosY = new XElement("y", ui.Position.Y);
                        XElement gameObjectPositionSizeX = new XElement("sizeX", ui.Size.X);
                        XElement gameObjectPositionSizeY = new XElement("sizeY", ui.Size.Y);

                        gameObjectElement.Add(name);
                        gameObjectElement.Add(gameObjectPositionPathTexture);
                        //gameObjectElement.Add(gameObjectPositionPathTextureEncript);
                        gameObjectElement.Add(gameObjectPositionPosX);
                        gameObjectElement.Add(gameObjectPositionPosY);
                        gameObjectElement.Add(gameObjectPositionSizeX);
                        gameObjectElement.Add(gameObjectPositionSizeY);

                        xScene.Add(gameObjectElement);
                    }
                    else if (ui.TypeUI.Equals(TypeUI.TEXT))
                    {
                        XElement gameObjectElement = new XElement("text");
                        XAttribute name = new XAttribute("name", ui.Name);

                        XElement uiPosX = new XElement("x", ui.Position.X);
                        XElement uiPosY = new XElement("y", ui.Position.Y);
                        XElement uiSize = new XElement("size", ui.Size.X);
                        XElement uiText = new XElement("text", ui.Text);

                        gameObjectElement.Add(name);
                        gameObjectElement.Add(uiText);
                        gameObjectElement.Add(uiPosX);
                        gameObjectElement.Add(uiPosY);
                        gameObjectElement.Add(uiSize);

                        xScene.Add(gameObjectElement);
                    }
                    else if (ui.TypeUI.Equals(TypeUI.IMAGE))
                    {
                        XElement gameObjectElement = new XElement("image");
                        XAttribute name = new XAttribute("name", ui.Name);

                        XElement gameObjectPositionPathTexture = new XElement("nameTexture", RemoveEndNameFile(ui.NameTexture));
                        XElement gameObjectPositionPathTextureEncript = new XElement("nameTextureEncript", StringToByteToString(ui.NameTexture) + ".bubla");
                        XElement gameObjectPositionPosX = new XElement("x", ui.Position.X);
                        XElement gameObjectPositionPosY = new XElement("y", ui.Position.Y * (typeProject.Equals(TypeProject.WEB) ? -1 : 1));
                        XElement gameObjectPositionSizeX = new XElement("sizeX", ui.Size.X);
                        XElement gameObjectPositionSizeY = new XElement("sizeY", ui.Size.Y);

                        gameObjectElement.Add(name);
                        gameObjectElement.Add(gameObjectPositionPathTexture);
                        //gameObjectElement.Add(gameObjectPositionPathTextureEncript);
                        gameObjectElement.Add(gameObjectPositionPosX);
                        gameObjectElement.Add(gameObjectPositionPosY);
                        gameObjectElement.Add(gameObjectPositionSizeX);
                        gameObjectElement.Add(gameObjectPositionSizeY);

                        xScene.Add(gameObjectElement);
                    }
                    else if (ui.TypeUI.Equals(TypeUI.EFFECT))
                    {
                        XElement gameObjectElement = new XElement("effect");
                        XAttribute name = new XAttribute("name", ui.Name);

                        XElement gameObjectPathTexture = new XElement("nameTexture", RemoveEndNameFile(ui.NameTexture));
                        XElement gameObjectPathTextureEncript = new XElement("nameTextureEncript", StringToByteToString(ui.NameTexture) + ".bubla");
                        XElement gameObjectPosX = new XElement("x", ui.Position.X);
                        XElement gameObjectPosY = new XElement("y", ui.Position.Y);
                        XElement gameObjectSizeX = new XElement("sizeX", ui.Size.X);
                        XElement gameObjectCountParticle = new XElement("countParticle", ui.CountParticle);
                        XElement gameObjectTypeEffect = new XElement("typeEffect", ui.TypeEffect.ToString().ToLower());

                        gameObjectElement.Add(name);
                        gameObjectElement.Add(gameObjectPathTexture);
                        //gameObjectElement.Add(gameObjectPathTextureEncript);
                        gameObjectElement.Add(gameObjectPosX);
                        gameObjectElement.Add(gameObjectPosY);
                        gameObjectElement.Add(gameObjectSizeX);
                        gameObjectElement.Add(gameObjectCountParticle);
                        gameObjectElement.Add(gameObjectTypeEffect);

                        xScene.Add(gameObjectElement);
                    }
                    else if (ui.TypeUI.Equals(TypeUI.BOSSBAR))
                    {
                        XElement gameObjectElement = new XElement("bossbar");
                        XAttribute name = new XAttribute("name", ui.Name);

                        //XElement gameObjectPathTexture = new XElement("nameTexture", ui.NameTexture);
                        //XElement gameObjectPathTexture = new XElement("nameTextureEncript", StringToByteToString(ui.NameTexture));
                        XElement gameObjectPosX = new XElement("x", ui.Position.X);
                        XElement gameObjectPosY = new XElement("y", ui.Position.Y);
                        XElement gameObjectSizeX = new XElement("sizeX", ui.Size.X);
                        XElement gameObjectSizeY = new XElement("sizeY", ui.Size.Y);
                        XElement rgbRInside = new XElement("rgbRInside", ui.GetRgbInside().R);
                        XElement rgbGInside = new XElement("rgbGInside", ui.GetRgbInside().G);
                        XElement rgbBInside = new XElement("rgbBInside", ui.GetRgbInside().B);
                        XElement rgbROutside = new XElement("rgbROutside", ui.GetRgbOutside().R);
                        XElement rgbGOutside = new XElement("rgbGOutside", ui.GetRgbOutside().G);
                        XElement rgbBOutside = new XElement("rgbBOutside", ui.GetRgbOutside().B);

                        gameObjectElement.Add(name);
                        //gameObjectElement.Add(gameObjectPathTexture);
                        gameObjectElement.Add(gameObjectPosX);
                        gameObjectElement.Add(gameObjectPosY);
                        gameObjectElement.Add(gameObjectSizeX);
                        gameObjectElement.Add(gameObjectSizeY);
                        gameObjectElement.Add(rgbRInside);
                        gameObjectElement.Add(rgbGInside);
                        gameObjectElement.Add(rgbBInside);
                        gameObjectElement.Add(rgbROutside);
                        gameObjectElement.Add(rgbGOutside);
                        gameObjectElement.Add(rgbBOutside);

                        xScene.Add(gameObjectElement);
                    }
                    else if (ui.TypeUI.Equals(TypeUI.SCRIPT))
                    {
                        //if (($"assets\\{ui.Name}.lua").Equals("assets\\main.lua")) ; 
                        luaScripts.Add(ui);
                    }
                }

                string luaText = "";

                int layerScript = 0;
                while (true)
                {
                    foreach (UI script in luaScripts)
                    {
                        //Console.WriteLine($"{script.Name} {script.Layer} = {layerScript}");
                        if (script.Layer == layerScript)
                        {
                            luaText += ReadFile($"assets\\{script.Name}.lua") + "\n";
                            //Console.WriteLine(luaText);
                        }
                    }
                    if (layerScript == maxLayerScript) break;
                    layerScript++;
                }
                //Console.WriteLine($"загрузка скриптов прекратилась на слое: { layerScript}");

                Encryption.EncodTextAndExport(luaText, Option.OutPathOutFolder + "\\mainScript" + nameScene.Value + ".bubla");
                gameObjectsElement.Add(xScene);
            }
            xdoc.Add(gameObjectsElement);
            xdoc.Save("assets\\infoGame.xml");
            //xdoc.Save("assets\\infoGame.bubla");
            //if(Option.OutPathOutFolder != null) xdoc.Save(Option.OutPathOutFolder + "\\infoGame.xml");
        }

        public static void saveXmlInfoGameEngine()
        {
            XDocument xdoc = new XDocument();
            XElement gameObjectsElement = new XElement("data");

            gameObjectsElement.Add(new XElement("outPathOutFolder", Option.OutPathOutFolder));
            gameObjectsElement.Add(new XElement("typeProject", typeProject.ToString().ToLower()));
            gameObjectsElement.Add(new XElement("pathStartFile", Option.PathStartFile));
            gameObjectsElement.Add(new XElement("maxLayerScript", maxLayerScript));

            foreach (Scene scene in scenes)
            {
                XElement xScene = new XElement("scene");
                XAttribute nameScene = new XAttribute("name", scene.Name);
                xScene.Add(nameScene);

                foreach (UI ui in scene.Uis)
                {
                    XElement gameObjectElement = new XElement("ui");
                    XAttribute name = new XAttribute("name", ui.Name);

                    XElement positionX = new XElement("x", ui.Position.X);
                    XElement positionY = new XElement("y", ui.Position.Y);
                    XElement sourse = new XElement("fileSound", ui.SourseSound);
                    XElement sourseScript = new XElement("fileScript", ui.SourseScript);
                    XElement type = new XElement("type", ui.TypeUI.ToString().ToLower());
                    XElement sizeX = new XElement("sizeX", ui.Size.X);
                    XElement sizeY = new XElement("sizeY", ui.Size.Y);
                    XElement pathTexture = new XElement("nameTexture", ui.NameTexture);
                    XElement text = new XElement("text", ui.Text);
                    XElement countParticle = new XElement("countParticle", ui.CountParticle);
                    XElement typeEffect = new XElement("typeEffect", ui.TypeEffect.ToString().ToLower());
                    XElement loop = new XElement("loop", ui.Loop);
                    XElement sound3D = new XElement("sound3D", ui.Sound3D);
                    XElement globalScript = new XElement("layer", ui.Layer);
                    XElement volume = new XElement("volume", ui.Volume);
                    XElement rgbRInside = new XElement("rgbRInside", ui.GetRgbInside().R);
                    XElement rgbGInside = new XElement("rgbGInside", ui.GetRgbInside().G);
                    XElement rgbBInside = new XElement("rgbBInside", ui.GetRgbInside().B);
                    XElement rgbROutside = new XElement("rgbROutside", ui.GetRgbOutside().R);
                    XElement rgbGOutside = new XElement("rgbGOutside", ui.GetRgbOutside().G);
                    XElement rgbBOutside = new XElement("rgbBOutside", ui.GetRgbOutside().B);

                    gameObjectElement.Add(name);
                    gameObjectElement.Add(positionX);
                    gameObjectElement.Add(positionY);
                    gameObjectElement.Add(sourse);
                    gameObjectElement.Add(sourseScript);
                    gameObjectElement.Add(type);
                    gameObjectElement.Add(sizeX);
                    gameObjectElement.Add(sizeY);
                    gameObjectElement.Add(pathTexture);
                    gameObjectElement.Add(text);
                    gameObjectElement.Add(countParticle);
                    gameObjectElement.Add(typeEffect);
                    gameObjectElement.Add(loop);
                    gameObjectElement.Add(sound3D);
                    gameObjectElement.Add(globalScript);
                    gameObjectElement.Add(volume);

                    gameObjectElement.Add(rgbRInside);
                    gameObjectElement.Add(rgbGInside);
                    gameObjectElement.Add(rgbBInside);
                    gameObjectElement.Add(rgbROutside);
                    gameObjectElement.Add(rgbGOutside);
                    gameObjectElement.Add(rgbBOutside);

                    xScene.Add(gameObjectElement);
                }
                gameObjectsElement.Add(xScene);
            }

            xdoc.Add(gameObjectsElement);
            xdoc.Save("info.xml");
        }

        private void LoadInfo()
        {
            if(!File.Exists("info.xml")) return;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("info.xml");

            XmlElement xRoot = xDoc.DocumentElement;
            if (xRoot != null)
            {
                string nameTexture = "";
                float x = 0;
                float y = 0;
                float sizeX = 0;
                float sizeY = 0;

                TypeUI typeUI = TypeUI.BUTTON;
                string fileSound = "";
                string fileScript = "";

                string text = "";

                int layer = 0;

                bool? loop = false;
                bool? sound3D = false;
                float volume = 0;

                int countParticle = 0;
                TypeEffect typeEffect = TypeEffect.PULSE;

                Rgb rgbInside = new Rgb();
                Rgb rgbOutSide = new Rgb();

                foreach (XmlElement xnode in xRoot)
                {
                    if (xnode.Name.Equals("outPathOutFolder")) Option.OutPathOutFolder = xnode.InnerText;
                    else if (xnode.Name.Equals("typeProject"))
                    {
                        switch(xnode.InnerText)
                        {
                            case "desktop":
                                typeProject = TypeProject.DESKTOP;
                                break;
                            case "web":
                                typeProject = TypeProject.WEB;
                                break;
                        }
                    }
                    else if (xnode.Name.Equals("pathStartFile")) Option.PathStartFile = xnode.InnerText;
                    else if (xnode.Name.Equals("maxLayerScript")) maxLayerScript = int.Parse(xnode.InnerText);
                    else if (xnode.Name.Equals("scene"))
                    {
                        XmlNode attrScene = xnode.Attributes.GetNamedItem("name");
                        //Console.WriteLine(attrScene.Value);

                        foreach (XmlNode childnodeScene in xnode.ChildNodes)
                        {
                            
                            if (childnodeScene.Name.Equals("ui"))
                            {
                                //Console.WriteLine("asdasdasd");
                                XmlNode attr = childnodeScene.Attributes.GetNamedItem("name"); 

                                foreach (XmlNode childnode in childnodeScene.ChildNodes)
                                {
                                    if (childnode.Name.Equals("x"))
                                    {
                                        x = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("y"))
                                    {
                                        y = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("sizeX"))
                                    {
                                        sizeX = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("sizeY"))
                                    {
                                        sizeY = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("type"))
                                    {
                                        switch (childnode.InnerText)
                                        {
                                            case "button":
                                                typeUI = TypeUI.BUTTON;
                                                break;
                                            case "sound":
                                                typeUI = TypeUI.SOUND;
                                                break;
                                            case "text":
                                                typeUI = TypeUI.TEXT;
                                                break;
                                            case "image":
                                                typeUI = TypeUI.IMAGE;
                                                break;
                                            case "effect":
                                                typeUI = TypeUI.EFFECT;
                                                break;
                                            case "bossbar":
                                                typeUI = TypeUI.BOSSBAR;
                                                break;
                                            case "script":
                                                typeUI = TypeUI.SCRIPT;
                                                break;
                                        }
                                    }
                                    else if (childnode.Name.Equals("nameTexture"))
                                    {
                                        nameTexture = childnode.InnerText;
                                    }
                                    else if (childnode.Name.Equals("fileSound"))
                                    {
                                        fileSound = childnode.InnerText;
                                    }
                                    else if (childnode.Name.Equals("fileScript"))
                                    {
                                        fileScript = childnode.InnerText;
                                    }
                                    else if (childnode.Name.Equals("layer"))
                                    {
                                        layer = int.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("text"))
                                    {
                                        text = childnode.InnerText;
                                    }
                                    else if (childnode.Name.Equals("loop"))
                                    {
                                        loop = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("sound3D"))
                                    {
                                        sound3D = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("volume"))
                                    {
                                        volume = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("countParticle"))
                                    {
                                        countParticle = int.Parse(childnode.InnerText);
                                    }
                                    //
                                    /*XElement rgbRInside = new XElement("rgbRInside", ui.GetRgbInside().R);
                                    XElement rgbGInside = new XElement("rgbGInside", ui.GetRgbInside().G);
                                    XElement rgbBInside = new XElement("rgbBInside", ui.GetRgbInside().B);
                                    XElement rgbROutside = new XElement("rgbROutside", ui.GetRgbOutside().R);
                                    XElement rgbGOutside = new XElement("rgbGOutside", ui.GetRgbOutside().G);
                                    XElement rgbBOutside = new XElement("rgbBOutside", ui.GetRgbOutside().B);*/
                                    else if (childnode.Name.Equals("rgbRInside"))
                                    {
                                        rgbInside.R = byte.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("rgbGInside"))
                                    {
                                        rgbInside.G = byte.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("rgbBInside"))
                                    {
                                        rgbInside.B = byte.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("rgbROutside"))
                                    {
                                        rgbOutSide.R = byte.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("rgbGOutside"))
                                    {
                                        rgbOutSide.G = byte.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("rgbBOutside"))
                                    {
                                        rgbOutSide.B = byte.Parse(childnode.InnerText);
                                    }
                                    //
                                    else if (childnode.Name.Equals("typeEffect"))
                                    {
                                        switch (childnode.InnerText)
                                        {
                                            case "pulse":
                                                typeEffect = TypeEffect.PULSE;
                                                break;
                                        }
                                    }
                                }
                                UI ui = new UI(attr.Value, typeUI);
                                ui.NameTexture = nameTexture;
                                ui.Position = new Vector(x, y);
                                ui.SourseSound = fileSound;
                                //Console.WriteLine("scritp sourse = " + fileScript);
                                ui.SourseScript = fileScript;
                                ui.Size = new Vector(sizeX, sizeY);
                                ui.Text = text;
                                ui.CountParticle = countParticle;
                                ui.TypeEffect = typeEffect;
                                ui.Loop = loop;
                                ui.Sound3D = sound3D;
                                ui.Volume = volume;
                                ui.Layer = layer;
                                ui.SetRgbOutside(rgbOutSide);
                                ui.SetRgbInside(rgbInside);

                                GetScene(attrScene.Value).Uis.Add(ui);
                                //Console.WriteLine($"in scene {GetScene(attrScene.Value).Name} ui: {GetScene(attrScene.Value).Uis.Count}");
                            }
                        }
                    }
                }
            }
        }

        private void LoadInfoGame()
        {
            if (!File.Exists("assets\\infoGame.xml")) return;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("assets\\infoGame.xml");

            XmlElement xRoot = xDoc.DocumentElement;
            if (xRoot != null)
            {
                string tag = "";
                string nameTexture = "";
                float x = 0;
                float y = 0;
                float sizeX = 0;
                float sizeY = 0;
                float rotate = 0;
                int columns = 0;
                int rows = 0;
                int columnPos = 0;
                int rowPos = 0;
                bool? point = false;
                bool? resourse = false;
                bool? smooth = false;
                bool? repited = false;

                bool? collider = false;
                bool? fixedRotation = false;
                bool? fixedRotationTexture = false;
                bool? trigger = false;
                string typeCollider = "square";

                string anim = "";
                Vector sizeAddCollider = new Vector(0, 0);
                Vector posAddCollider = new Vector(0, 0);
                float angle = 0;
                bool? splitSprite = false;
                bool? splitSpritePixel = false;
                float mass = 0;

                foreach (XmlElement xnode in xRoot)
                {
                    if (xnode.Name.Equals("title")) Option.TitleGame = xnode.InnerText;
                    else if (xnode.Name.Equals("icon")) Option.IconGame = xnode.InnerText;
                    else if (xnode.Name.Equals("colorBackgroundR")) Option.ColorBackground[0] = byte.Parse(xnode.InnerText);
                    else if (xnode.Name.Equals("colorBackgroundG")) Option.ColorBackground[1] = byte.Parse(xnode.InnerText);
                    else if (xnode.Name.Equals("colorBackgroundB")) Option.ColorBackground[2] = byte.Parse(xnode.InnerText);
                    else if (xnode.Name.Equals("widhtScreen")) Option.WidhtScreen = int.Parse(xnode.InnerText);
                    else if (xnode.Name.Equals("heightScreen")) Option.HeightScreen = int.Parse(xnode.InnerText);
                    else if (xnode.Name.Equals("fullScreen")) Option.DebugMode = xnode.InnerText.Equals("true");
                    else if (xnode.Name.Equals("deviceConnect")) Option.DeviceConnect = xnode.InnerText.Equals("true");
                    else if (xnode.Name.Equals("maxLayer")) maxLayer = int.Parse(xnode.InnerText);
                    else if (xnode.Name.Equals("nameSceneStart")) Option.NameSceneStart = xnode.InnerText;
                    else if (xnode.Name.Equals("sizeMouseTexture")) Option.SizeMouseTexture = xnode.InnerText;
                    else if (xnode.Name.Equals("mouseInGame")) Option.MouseInGame = xnode.InnerText.Equals("true");
                    else if (xnode.Name.Equals("scene"))
                    {
                        XmlNode attrScene = xnode.Attributes.GetNamedItem("name");
                        Scene scene = new Scene(attrScene.Value);
                        //Console.WriteLine("new scene");
                        //int indexScene = 0;

                        foreach (XmlNode childnodeScene in xnode.ChildNodes)
                        {
                            //Console.WriteLine(childnodeScene.Name);
                            if (childnodeScene.Name.Equals("gameObject"))
                            {
                                XmlNode attrName = childnodeScene.Attributes.GetNamedItem("name");
                                XmlNode attrLayer = childnodeScene.Attributes.GetNamedItem("layer");

                                foreach (XmlNode childnode in childnodeScene.ChildNodes)
                                {
                                    if (childnode.Name.Equals("x"))
                                    {
                                        x = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("y"))
                                    {
                                        y = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("tag"))
                                    {
                                        tag = childnode.InnerText;
                                    }
                                    else if (childnode.Name.Equals("sizeX"))
                                    {
                                        sizeX = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("sizeY"))
                                    {
                                        sizeY = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("rotate"))
                                    {
                                        rotate = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("nameTexture"))
                                    {
                                        nameTexture = childnode.InnerText + ".png";
                                    }
                                    else if (childnode.Name.Equals("point"))
                                    {
                                        point = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("resourse"))
                                    {
                                        resourse = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("smooth"))
                                    {
                                        smooth = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("repited"))
                                    {
                                        repited = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("collider"))
                                    {
                                        collider = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("animation"))
                                    {
                                        anim = childnode.InnerText;
                                    }
                                    else if (childnode.Name.Equals("sizeAddColliderX"))
                                    {
                                        sizeAddCollider.X = int.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("sizeAddColliderY"))
                                    {
                                        sizeAddCollider.Y = int.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("posAddColliderX"))
                                    {
                                        posAddCollider.X = int.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("posAddColliderY"))
                                    {
                                        posAddCollider.Y = int.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("trigger"))
                                    {
                                        trigger = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("angle"))
                                    {
                                        angle = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("typeCollider"))
                                    {
                                        typeCollider = childnode.InnerText;
                                    }
                                    else if (childnode.Name.Equals("fixedRotation"))
                                    {
                                        fixedRotation = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("fixedRotationTexture"))
                                    {
                                        fixedRotationTexture = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("mass"))
                                    {
                                        mass = float.Parse(childnode.InnerText, CultureInfo.InvariantCulture);
                                    }
                                    else if (childnode.Name.Equals("splitSprite"))
                                    {
                                        splitSprite = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("splitSpritePixel"))
                                    {
                                        splitSpritePixel = childnode.InnerText.Equals("true");
                                    }
                                    else if (childnode.Name.Equals("splitSpriteColumns"))
                                    {
                                        columns = int.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("splitSpriteRows"))
                                    {
                                        rows = int.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("splitSpriteRowPos"))
                                    {
                                        rowPos = int.Parse(childnode.InnerText);
                                    }
                                    else if (childnode.Name.Equals("splitSpriteColumnPos"))
                                    {
                                        columnPos = int.Parse(childnode.InnerText);
                                    }
                                }
                                GameObject gameObject = new GameObject(attrName.Value);
                                gameObject.Tag = tag;
                                gameObject.NameTexture = nameTexture;
                                gameObject.Position = new Vector(x, y);
                                gameObject.Size = new Vector(sizeX, sizeY);
                                gameObject.Rotate = rotate;
                                gameObject.Point = point;
                                gameObject.Resourse = resourse;
                                gameObject.Smooth = smooth;
                                gameObject.Repited = repited;

                                gameObject.Collider = collider;
                                //gameObject.Physics = physics;
                                gameObject.Trigger = trigger;
                                gameObject.FixedRotation = fixedRotation;
                                gameObject.FixedRotationTexture = fixedRotationTexture;
                                gameObject.Mass = mass;
                                gameObject.SizeAdditionCollider = sizeAddCollider;
                                gameObject.PosAdditionCollider = posAddCollider;
                                gameObject.Angle = angle;
                                gameObject.TypeCollider = typeCollider;

                                gameObject.SplitSprite = splitSprite;
                                gameObject.SplitSpritePixel = splitSpritePixel;
                                gameObject.SplitSpriteSize = new Vector(rows, columns);
                                gameObject.SplitSpritePos = new Vector(rowPos, columnPos);
                                gameObject.Animation = anim;

                                gameObject.Layer = int.Parse(attrLayer.Value);

                                scene.GameObjects.Add(gameObject);
                            }
                        }
                        scenes.Add(scene);
                    }
                }
            }
        }

        private void updateGameObjectsOnScene()
        {
            BitmapImage texture = null;

            //int point = 0;
            int i = 0;
            int k = 0;
            int l = 0;
            int j = 0;
            int o = 0;
            foreach (UIElement child in canvas.Children)
            {
                /*if(child is Label)
                {
                    Canvas.SetBottom((Image)child, 335 - gameObjects[i].Position.Y - ((Image)child).Height);
                    Canvas.SetRight((Image)child, 728 - gameObjects[i].Position.X - ((Image)child).Width);
                }*/
                try
                {
                    //if (i == gameObjects.Count) { point = i; break; }
                    if (GetScene().GameObjects[i].NameTexture == null) texture = GetBitmapImageFromResourse("noneTexture.png");
                    else if (GetScene().GameObjects[i].Point.Value) texture = GetBitmapImageFromResourse("point.png");
                    else if (GetScene().GameObjects[i].Resourse.Value) texture = GetBitmapImageFromResourse("noneTexture2.png");
                    else if (GetScene().GameObjects[i].Trigger.Value && GetScene().GameObjects[i].Collider.Value) {
                        texture = GetBitmapImageFromResourse("whilePixel.png");
                    }
                    else texture = GetBitmapImageFromAssets(GetScene().GameObjects[i].NameTexture);
                    ((Image)child).Source = texture;
                    ((Image)child).Width = GetScene().GameObjects[i].Size.X * zoomCamera;
                    ((Image)child).Height = GetScene().GameObjects[i].Size.Y * zoomCamera;
                    RotateTransform rotateTransform = new RotateTransform(GetScene().GameObjects[i].Rotate);
                    rotateTransform.CenterX = GetScene().GameObjects[i].Size.X * zoomCamera / 2;
                    rotateTransform.CenterY = GetScene().GameObjects[i].Size.Y * zoomCamera / 2;
                    ((Image)child).RenderTransform = rotateTransform;
                    Canvas.SetBottom((Image)child, heightCanvas - GetScene().GameObjects[i].Position.Y * zoomCamera - ((Image)child).Height + posCamera.Y);
                    Canvas.SetRight((Image)child, widthCanvas - GetScene().GameObjects[i].Position.X * zoomCamera - ((Image)child).Width + posCamera.X);
                    i++;
                    continue;
                } catch/*(ArgumentOutOfRangeException e)*/ { }

                try
                {
                    while (!GetScene().Uis[k].TypeUI.Equals(TypeUI.BUTTON))
                    {
                        k++;
                    }

                    if (GetScene().Uis[k].TypeUI.Equals(TypeUI.BUTTON) && !(child is Label)) {
                        /*if (uis[k].NameTexture == "") texture = GetBitmapImageFromResourse("noneTexture.png");
                        else texture = GetBitmapImageFromAssets(uis[k].NameTexture);*/
                        texture = GetBitmapImageFromAssets(GetScene().Uis[k].NameTexture);
                        ((Image)child).Source = texture;
                        ((Image)child).Width = GetScene().Uis[k].Size.X * zoomCamera;
                        ((Image)child).Height = GetScene().Uis[k].Size.Y * zoomCamera;
                        //Console.WriteLine("test 2 " + ((Image)child).Width);
                        Canvas.SetBottom((Image)child, heightCanvas - GetScene().Uis[k].Position.Y * zoomCamera - ((Image)child).Height + posCamera.Y);
                        Canvas.SetRight((Image)child, widthCanvas - GetScene().Uis[k].Position.X * zoomCamera - ((Image)child).Width + posCamera.X);
                        k++;
                        continue;
                    }
                }
                catch (ArgumentOutOfRangeException e) { }

                if (child is Label)
                {
                    try
                    {
                        while (!GetScene().Uis[l].TypeUI.Equals(TypeUI.TEXT))
                        {
                            l++;
                        }
                    }
                    catch (ArgumentOutOfRangeException e) { }

                    try
                    {
                        ((Label)child).FontSize = GetScene().Uis[l].Size.X * zoomCamera;
                    }
                    catch (ArgumentException e) { }
                    ((Label)child).Content = GetScene().Uis[l].Text;
                    ((Label)child).Margin = new Thickness((double)(GetScene().Uis[l].Position.X * zoomCamera - GetScene().Uis[l].Size.X / 10 + posCamera.X * -1),
                        (double)(GetScene().Uis[l].Position.Y * zoomCamera - GetScene().Uis[l].Size.X / 10 + posCamera.Y * -1),
                        0, 0);
                    l++;
                    continue;
                }

                // images
                try
                {
                    while (!GetScene().Uis[j].TypeUI.Equals(TypeUI.IMAGE))
                    {
                        //Console.WriteLine(j);
                        j++;
                    }

                    /*Console.WriteLine(uis[j].TypeUI.Equals(TypeUI.IMAGE) + " " + j + " x: " + uis[j].Size.X + " name: " + uis[j].Name +
                        " is image: " + (child is Image));*/

                    if (GetScene().Uis[j].TypeUI.Equals(TypeUI.IMAGE))
                    {
                        //Console.WriteLine("detext");
                        /*if (uis[j].NameTexture == "") texture = GetBitmapImageFromResourse("noneTexture.png");
                        else texture = GetBitmapImageFromAssets(uis[j].NameTexture);*/
                        texture = GetBitmapImageFromAssets(GetScene().Uis[j].NameTexture);
                        ((Image)child).Source = texture;
                        ((Image)child).Width = GetScene().Uis[j].Size.X * zoomCamera;
                        ((Image)child).Height = GetScene().Uis[j].Size.Y * zoomCamera;
                        //Console.WriteLine(((Image)child).Width + " uid: " + ((Image)child).Uid);
                        //Console.WriteLine("test 2 " + ((Image)child).Width);
                        Canvas.SetBottom((Image)child, heightCanvas - GetScene().Uis[j].Position.Y * zoomCamera - ((Image)child).Height + posCamera.Y);
                        Canvas.SetRight((Image)child, widthCanvas - GetScene().Uis[j].Position.X * zoomCamera - ((Image)child).Width + posCamera.X);
                        j++;
                        //Console.WriteLine(j);
                    }
                }
                catch (ArgumentOutOfRangeException e) { }

                // effect
                try
                {
                    while (!GetScene().Uis[o].TypeUI.Equals(TypeUI.EFFECT))
                    {
                        //Console.WriteLine(j);
                        o++;
                    }

                    /*Console.WriteLine(uis[j].TypeUI.Equals(TypeUI.IMAGE) + " " + j + " x: " + uis[j].Size.X + " name: " + uis[j].Name +
                        " is image: " + (child is Image));*/

                    if (GetScene().Uis[o].TypeUI.Equals(TypeUI.EFFECT))
                    {
                        //Console.WriteLine("detext");
                        /*if (uis[j].NameTexture == "") texture = GetBitmapImageFromResourse("noneTexture.png");
                        else texture = GetBitmapImageFromAssets(uis[j].NameTexture);*/
                        texture = GetBitmapImageFromResourse("effect.png");
                        ((Image)child).Source = texture;
                        ((Image)child).Width = sizeEffectTexture * zoomCamera;
                        ((Image)child).Height = sizeEffectTexture * zoomCamera;
                        //Console.WriteLine(((Image)child).Width + " uid: " + ((Image)child).Uid);
                        //Console.WriteLine("test 2 " + ((Image)child).Width);
                        Canvas.SetBottom((Image)child, heightCanvas - GetScene().Uis[o].Position.Y * zoomCamera - ((Image)child).Height + posCamera.Y);
                        Canvas.SetRight((Image)child, widthCanvas - GetScene().Uis[o].Position.X * zoomCamera - ((Image)child).Width + posCamera.X);
                        j++;
                    }
                }
                catch (ArgumentOutOfRangeException e) { }
                //if (!(child is Image)) continue;
                //Console.WriteLine(child.Uid);
                if(child.Uid.Equals("sceneCamView"))
                {
                    //Console.WriteLine(((Image)child).Uid);
                    //Console.WriteLine("asddasdaadsasd12331213 2");
                    ((Image)child).Width = widthCanvas * zoomCamera;
                    ((Image)child).Height = heightCanvas * zoomCamera;
                    Canvas.SetBottom((Image)child, heightCanvas - ((Image)child).Height + posCamera.Y);
                    Canvas.SetRight((Image)child, widthCanvas - ((Image)child).Width + posCamera.X);
                }
                if(child.Uid.Equals("frameTexture"))
                {
                    //Console.WriteLine(((Image)child).Width);
                    //Console.WriteLine("asddasdaadsasd12331213");
                    try
                    {
                        GameObject gameObject = findGameObject(textBlockSelectNow.Text);
                        if (gameObject != null) {
                            ((Image)child).Width = gameObject.Size.X * zoomCamera;
                            ((Image)child).Height = gameObject.Size.Y * zoomCamera;
                            Canvas.SetBottom((Image)child, heightCanvas - gameObject.Position.Y * zoomCamera - ((Image)child).Height + posCamera.Y);
                            Canvas.SetRight((Image)child, widthCanvas - gameObject.Position.X * zoomCamera - ((Image)child).Width + posCamera.X);
                        } else
                        {
                            UI ui = findUI(textBlockSelectNow.Text);
                            ((Image)child).Width = ui.Size.X * zoomCamera * (ui.TypeUI == TypeUI.TEXT ? 0 : 1);
                            ((Image)child).Height = ui.Size.Y * zoomCamera * (ui.TypeUI == TypeUI.TEXT ? 0 : 1);
                            Canvas.SetBottom((Image)child, heightCanvas - ui.Position.Y * zoomCamera - ((Image)child).Height + posCamera.Y);
                            Canvas.SetRight((Image)child, widthCanvas - ui.Position.X * zoomCamera - ((Image)child).Width + posCamera.X);
                        }
                    }
                    catch { }
                }

                /*if (k == uis.Count) { break; }
                if (!uis[k].TypeUI.Equals(TypeUI.BUTTON))
                {
                    Console.WriteLine("test 0 " + uis[k].Name);
                    k++;
                    continue;
                }
                Console.WriteLine("test 1 " + uis[k].Name);
                */
            }

            /*int k = 0;
            foreach (UIElement child in canvas.Children)
            {
                Console.WriteLine(i + " = " + ( point + uis.Count));
                if (i == point + uis.Count) break;
                if (!uis[k].TypeUI.Equals(TypeUI.BUTTON))
                {
                    Console.WriteLine("test 0");
                    i++;
                    k++;
                    continue;
                }
                Console.WriteLine("test 1");
                if (uis[k].NameTexture == null) texture = GetBitmapImageFromResourse("noneTexture.png");
                else texture = GetBitmapImageFromAssets(uis[k].NameTexture);
                ((Image)child).Width = uis[k].Size;
                ((Image)child).Height = uis[k].Size;
                Canvas.SetBottom((Image)child, 335 - uis[k].Position.Y - ((Image)child).Height);
                Canvas.SetRight((Image)child, 728 - uis[k].Position.X - ((Image)child).Width);
                Console.WriteLine("test 2");
                k++;
                i++;
            }*/
        }
        
        
        private BitmapImage GetBitmapImageFromAssets(string nameResourse)
        {
            try
            {
                return new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "assets\\" + nameResourse));
            } catch(DirectoryNotFoundException e) {
                return GetBitmapImageFromResourse("noneTexture.png");
            }
        }

        private BitmapImage GetBitmapImageFromResourse(string nameResourse)
        {
            return new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "resourse\\" + nameResourse));
        }

        public void addUI(string name, TypeUI typeUI)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "ui " + GetScene().Uis.Count;
            //textBlock.PreviewMouseDown += clickEvent;

            listUI.Items.Add(textBlock);
            GetScene().Uis.Add(new UI(name, typeUI));
        }

        /*private void visibilityButtonUIEditor(Visibility visibility)
        {
            labelXUI.Visibility = visibility;
            labelYUI.Visibility = visibility;
            yUI.Visibility = visibility;
            xUI.Visibility = visibility;
            imageUI.Visibility = visibility;
            sizeUIX.Visibility = visibility;
            sizeUIY.Visibility = visibility;
            labelSizeUI.Visibility = visibility;
            addTextureUIButton.Visibility = visibility;
        }

        private void visibilitySoundUIEditor(Visibility visibility)
        {
            buttonFindSoundFile.Visibility = visibility;
        }*/
        

        private void SetUIEditor(TypeUI typeUI)
        {
            labelXUI.Visibility = Visibility.Hidden;
            labelYUI.Visibility = Visibility.Hidden;
            yUI.Visibility = Visibility.Hidden;
            xUI.Visibility = Visibility.Hidden;
            imageUI.Visibility = Visibility.Hidden;
            sizeUIX.Visibility = Visibility.Hidden;
            sizeUIY.Visibility = Visibility.Hidden;
            labelSizeUI.Visibility = Visibility.Hidden;
            addTextureUIButton.Visibility = Visibility.Hidden;
            labelTextUI.Visibility = Visibility.Hidden;
            textUI.Visibility = Visibility.Hidden;
            buttonFindSoundFile.Visibility = Visibility.Hidden;
            labelTypeEffectUI.Visibility = Visibility.Hidden;
            labelCountParticleUI.Visibility = Visibility.Hidden;
            countUI.Visibility = Visibility.Hidden;
            typeEffect.Visibility = Visibility.Hidden;
            labelVolume.Visibility = Visibility.Hidden;
            volume.Visibility = Visibility.Hidden;
            checkBoxLoop.Visibility = Visibility.Hidden;
            checkBox3DSound.Visibility = Visibility.Hidden;

            labelRgbRInside.Visibility = Visibility.Hidden;
            labelRgbGInside.Visibility = Visibility.Hidden;
            labelRgbBInside.Visibility = Visibility.Hidden;
            labelRgbROutside.Visibility = Visibility.Hidden;
            labelRgbGOutside.Visibility = Visibility.Hidden;
            labelRgbBOutside.Visibility = Visibility.Hidden;
            rgbRInside.Visibility = Visibility.Hidden;
            rgbGInside.Visibility = Visibility.Hidden;
            rgbBInside.Visibility = Visibility.Hidden;
            rgbROutside.Visibility = Visibility.Hidden;
            rgbGOutside.Visibility = Visibility.Hidden;
            rgbBOutside.Visibility = Visibility.Hidden;
            selectRgbInside.Visibility = Visibility.Hidden;
            selectRgbOutside.Visibility = Visibility.Hidden;
            labelLayerScript.Visibility = Visibility.Hidden;

            openScriptButton.Visibility = Visibility.Hidden;
            layerScript.Visibility = Visibility.Hidden;
            //checkBoxGlobalScript_Copy.Visibility = Visibility.Hidden;

            if(typeUI.Equals(TypeUI.NONE))
            {
                SetVisibleEditor(Visibility.Hidden, VisibilityEditor.Ui);
                SetVisibleEditor(Visibility.Hidden, VisibilityEditor.Object);
            }
            else if (typeUI.Equals(TypeUI.BUTTON) || typeUI.Equals(TypeUI.IMAGE))
            {
                labelXUI.Visibility = Visibility.Visible;
                labelYUI.Visibility = Visibility.Visible;
                yUI.Visibility = Visibility.Visible;
                xUI.Visibility = Visibility.Visible;
                imageUI.Visibility = Visibility.Visible;
                sizeUIX.Visibility = Visibility.Visible;
                sizeUIY.Visibility = Visibility.Visible;
                labelSizeUI.Visibility = Visibility.Visible;
                addTextureUIButton.Visibility = Visibility.Visible;
            }
            else if(typeUI.Equals(TypeUI.SOUND))
            {
                buttonFindSoundFile.Visibility = Visibility.Visible;
                labelVolume.Visibility = Visibility.Visible;
                volume.Visibility = Visibility.Visible;
                checkBoxLoop.Visibility = Visibility.Visible;
                checkBox3DSound.Visibility = Visibility.Visible;
            }
            else if(typeUI.Equals(TypeUI.TEXT))
            {
                labelXUI.Visibility = Visibility.Visible;
                labelYUI.Visibility = Visibility.Visible;
                yUI.Visibility = Visibility.Visible;
                xUI.Visibility = Visibility.Visible;
                sizeUIX.Visibility = Visibility.Visible;
                labelSizeUI.Visibility = Visibility.Visible;
                labelTextUI.Visibility = Visibility.Visible;
                textUI.Visibility = Visibility.Visible;
            }
            else if(typeUI.Equals(TypeUI.EFFECT))
            {
                labelXUI.Visibility = Visibility.Visible;
                labelYUI.Visibility = Visibility.Visible;
                yUI.Visibility = Visibility.Visible;
                xUI.Visibility = Visibility.Visible;
                sizeUIX.Visibility = Visibility.Visible;
                labelSizeUI.Visibility = Visibility.Visible;
                labelTypeEffectUI.Visibility = Visibility.Visible;
                labelCountParticleUI.Visibility = Visibility.Visible;
                countUI.Visibility = Visibility.Visible;
                typeEffect.Visibility = Visibility.Visible;
                imageUI.Visibility = Visibility.Visible;
                addTextureUIButton.Visibility = Visibility.Visible;
            }
            else if(typeUI.Equals(TypeUI.BOSSBAR))
            {
                labelRgbRInside.Visibility = Visibility.Visible;
                labelRgbGInside.Visibility = Visibility.Visible;
                labelRgbBInside.Visibility = Visibility.Visible;
                labelRgbROutside.Visibility = Visibility.Visible;
                labelRgbGOutside.Visibility = Visibility.Visible;
                labelRgbBOutside.Visibility = Visibility.Visible;
                rgbRInside.Visibility = Visibility.Visible;
                rgbGInside.Visibility = Visibility.Visible;
                rgbBInside.Visibility = Visibility.Visible;
                rgbROutside.Visibility = Visibility.Visible;
                rgbGOutside.Visibility = Visibility.Visible;
                rgbBOutside.Visibility = Visibility.Visible;
                selectRgbInside.Visibility = Visibility.Visible;
                selectRgbOutside.Visibility = Visibility.Visible;

                labelXUI.Visibility = Visibility.Visible;
                labelYUI.Visibility = Visibility.Visible;
                yUI.Visibility = Visibility.Visible;
                xUI.Visibility = Visibility.Visible;
                sizeUIX.Visibility = Visibility.Visible;
                sizeUIY.Visibility = Visibility.Visible;
                labelSizeUI.Visibility = Visibility.Visible;
            }
            else if(typeUI.Equals(TypeUI.SCRIPT))
            {
                openScriptButton.Visibility = Visibility.Visible;
                layerScript.Visibility = Visibility.Visible;
                labelLayerScript.Visibility = Visibility.Visible;
                //checkBoxGlobalScript_Copy.Visibility = Visibility.Visible;
            }

            /*Visibility buttonUI = typeUI.Equals(TypeUI.BUTTON) ? Visibility.Visible : Visibility.Hidden;
            labelXUI.Visibility = buttonUI;
            labelYUI.Visibility = buttonUI;
            yUI.Visibility = buttonUI;
            xUI.Visibility = buttonUI;
            imageUI.Visibility = buttonUI;
            sizeUIX.Visibility = buttonUI;
            sizeUIY.Visibility = buttonUI;
            labelSizeUI.Visibility = buttonUI;
            addTextureUIButton.Visibility = buttonUI;

            if (!typeUI.Equals(TypeUI.BUTTON)) {
                //Visibility imageUI_visible = typeUI.Equals(TypeUI.IMAGE) ? Visibility.Visible : Visibility.Hidden;
                labelXUI.Visibility = Visibility.Visible;
                labelYUI.Visibility = Visibility.Visible;
                yUI.Visibility = Visibility.Visible;
                xUI.Visibility = Visibility.Visible;
                imageUI.Visibility = Visibility.Visible;
                sizeUIX.Visibility = Visibility.Visible;
                sizeUIY.Visibility = Visibility.Visible;
                labelSizeUI.Visibility = Visibility.Visible;
                addTextureUIButton.Visibility = Visibility.Visible;
            }

            Visibility soundUI = typeUI.Equals(TypeUI.SOUND) ? Visibility.Visible : Visibility.Hidden;
            buttonFindSoundFile.Visibility = soundUI;

            if (typeUI.Equals(TypeUI.BUTTON))
            {
                textUI.Visibility = Visibility.Hidden;
                labelTextUI.Visibility = Visibility.Hidden;
                return;
            }
            Visibility textUI_Visible = typeUI.Equals(TypeUI.TEXT) ? Visibility.Visible : Visibility.Hidden;
            labelXUI.Visibility = textUI_Visible;
            labelYUI.Visibility = textUI_Visible;
            yUI.Visibility = textUI_Visible;
            xUI.Visibility = textUI_Visible;
            sizeUIX.Visibility = textUI_Visible;
            labelSizeUI.Visibility = textUI_Visible;
            labelTextUI.Visibility = textUI_Visible;
            textUI.Visibility = textUI_Visible;*/
        }

        private void SetVisibleEditor(Visibility visibility, VisibilityEditor visibilityEditor)
        {
            switch(visibilityEditor)
            {
                case VisibilityEditor.Ui:
                    buttonDeleteUI.Visibility = visibility;
                    addTextureUIButton.Visibility = visibility;
                    imageUI.Visibility = visibility;
                    labelNameUI.Visibility = visibility;
                    nameUI.Visibility = visibility;
                    sortListEditor.Visibility = visibility;
                    labelXUI.Visibility = visibility;
                    labelYUI.Visibility = visibility;
                    xUI.Visibility = visibility;
                    yUI.Visibility = visibility;
                    labelSizeUI.Visibility = visibility;
                    sizeUIX.Visibility = visibility;
                    sizeUIY.Visibility = visibility;
                    buttonCloneUI.Visibility = visibility;
                    labelTextUI.Visibility = visibility;
                    //checkBoxGlobalScript_Copy.Visibility = visibility;
                    textUI.Visibility = visibility;
                    countUI.Visibility = visibility;
                    labelCountParticleUI.Visibility = visibility;
                    labelTypeEffectUI.Visibility = visibility;
                    typeEffect.Visibility = visibility;
                    typeEffect.Visibility = visibility;
                    labelRgbBInside.Visibility = visibility;
                    labelRgbRInside.Visibility = visibility;
                    labelRgbGInside.Visibility = visibility;
                    labelRgbROutside.Visibility = visibility;
                    labelRgbGOutside.Visibility = visibility;
                    labelRgbBOutside.Visibility = visibility;
                    rgbRInside.Visibility = visibility;
                    rgbGInside.Visibility = visibility;
                    rgbBInside.Visibility = visibility;
                    rgbROutside.Visibility = visibility;
                    rgbGOutside.Visibility = visibility;
                    rgbBOutside.Visibility = visibility;
                    selectRgbInside.Visibility = visibility;
                    selectRgbOutside.Visibility = visibility;
                    buttonFindSoundFile.Visibility = visibility;
                    openScriptButton.Visibility = visibility;
                    labelVolume.Visibility = visibility;
                    volume.Visibility = visibility;
                    labelLayerScript.Visibility = visibility;
                    if(typeProject != TypeProject.WEB)
                    {
                        checkBoxLoop.Visibility = visibility;
                    }
                    break;
                case VisibilityEditor.Object:
                    nameObjectLabel.Visibility = visibility;
                    nameObject.Visibility = visibility;
                    listShapesCollider.Visibility = visibility;
                    tagObject.Visibility = visibility;
                    checkBoxSmooth.Visibility = visibility;
                    checkBoxRepited.Visibility = visibility;
                    rotateObject.Visibility = visibility;
                    rotateObjectLabel.Visibility = visibility;
                    tagObjectLabel.Visibility = visibility;
                    checkBoxFixedRotation.Visibility = visibility;
                    checkBoxFixedRotationTexture.Visibility = visibility;
                    labelAngle.Visibility = visibility;
                    angle.Visibility = visibility;
                    labelAnimation.Visibility = visibility;
                    animation.Visibility = visibility;
                    labelPosCollider.Visibility = visibility;
                    layerLabel.Visibility = visibility;
                    layer.Visibility = visibility;
                    buttonCloneObject.Visibility = visibility;
                    buttonAddTextureName.Visibility = visibility;
                    imageObject.Visibility = visibility;
                    yObjectLabel.Visibility = visibility;
                    xObjectLabel.Visibility = visibility;
                    xObject.Visibility = visibility;
                    yObject.Visibility = visibility;
                    sizeLabel.Visibility = visibility;
                    sizeObjectY.Visibility = visibility;
                    sizeObjectX.Visibility = visibility;
                    checkBoxResourse.Visibility = visibility;
                    checkBoxCollider.Visibility = visibility;
                    checkBoxPoint.Visibility = visibility;
                    checkBoxSplitSprite.Visibility = visibility;
                    checkBoxSplitSpritePixels.Visibility = visibility;
                    checkBoxTrigger.Visibility = visibility;
                    labelMass.Visibility = visibility;
                    physicsMass.Visibility = visibility;
                    labelSizeCollider.Visibility = visibility;
                    sizeColliderX.Visibility = visibility;
                    sizeColliderY.Visibility = visibility;
                    posColliderX.Visibility = visibility;
                    posColliderY.Visibility = visibility;
                    columnPos.Visibility = visibility;
                    columnPosLabel.Visibility = visibility;
                    columns.Visibility = visibility;
                    columnsLabel.Visibility = visibility;
                    rowPos.Visibility = visibility;
                    rowPosLabel.Visibility = visibility;
                    rows.Visibility = visibility;
                    rowsLabel.Visibility = visibility;
                    buttonDeleteObjectName.Visibility = visibility;
                    buttonXUpName.Visibility = visibility;
                    buttonXDownName.Visibility = visibility;
                    buttonYUpName.Visibility = visibility;
                    buttonYDownName.Visibility = visibility;
                    break;
            }
        }

        private void loadAllFile()
        {
            DirectoryInfo d2 = new DirectoryInfo(Option.OutPathOutFolder);
            foreach (FileInfo file in d2.GetFiles())
            {
                file.Delete();
            }

            //DirectoryInfo d = new DirectoryInfo("assets");

            

            // export all files
            foreach (string _file in Directory.EnumerateFiles("assets", "*", SearchOption.AllDirectories))
            {
                FileInfo file = new FileInfo(_file);
                if (typeProject.Equals(TypeProject.DESKTOP)) {
                    string nameFile = RemoveEndNameFile(file.Name);
                    if (nameFile.Equals("")) continue;
                    string path = Option.OutPathOutFolder + "\\" + nameFile + ".bubla";

                    if (!file.Name.EndsWith(".ttf") && !file.Name.EndsWith(".ogg") && !file.Name.EndsWith(".png") && !file.Name.EndsWith(".lua") && !file.Name.Equals("save.bubla") && !file.Name.EndsWith(".tmx") && !file.Name.EndsWith(".tsx")) {
                        Encryption.Encod(file.FullName, path);
                    }
                    else if(!file.Name.EndsWith(".lua") && !File.Exists(path)) file.CopyTo(path);
                } else if (typeProject.Equals(TypeProject.WEB))
                {
                    string path = Option.OutPathOutFolder + "\\" + file.Name;
                    if (file.Name.EndsWith(".xml"))
                    {
                        FileStream fsRead = new FileStream(file.FullName, FileMode.Open);

                        string textRead = "let xmltext = \'";
                        int data = 0;
                        int scip = 0;
                        bool deleteTab = false;
                        bool newLine = false;
                        while ((data = fsRead.ReadByte()) != -1)
                        {
                            if (scip < 3)
                            {
                                scip++;
                                continue;
                            }
                            if ((char)(byte)data == '\n')
                            {
                                newLine = true;
                            }
                            if ((char)(byte)data == '<') {
                                deleteTab = false;
                                if (newLine)
                                {
                                    textRead += "\'<";
                                    newLine = false;
                                    continue;
                                }
                                textRead += "\' + \'";
                            }
                            else if ((char)(byte)data == '>')
                            {
                                deleteTab = true;
                                if (newLine)
                                {
                                    textRead += "\' +";
                                    newLine = false;
                                    continue;
                                }
                                textRead += ">\' + \'";
                                continue;
                            }
                            if (deleteTab && (char)(byte)data == ' ') continue;
                            textRead += (char)(byte)data;
                            //textRead += $"+ '{(char)(byte)data}' ";
                        }
                        fsRead.Close();
                        textRead += "\';";
                        string textReadFinal = "";

                        char[] textReadChars = textRead.ToCharArray();
                        for (int i = 0; i < textReadChars.Length ;i++)
                        {
                            if (textReadChars[i] == '\n')
                            {
                                textReadChars[i - 1] = '`';
                                textReadChars[i - 2] = '`';
                                textReadChars[i - 3] = '`';
                            }
                        }
                        foreach(char _char in textReadChars)
                        {
                            textReadFinal += _char;
                        }
                        textReadFinal = textReadFinal.Replace("`", "").Replace("\\", "\\\\");

                        FileStream fsWrite = new FileStream(path.Replace(".xml", ".js"), FileMode.Create);

                        byte[] bytesWrite = Encoding.UTF8.GetBytes(textReadFinal);
                        for (int i = 0; i < bytesWrite.Length; i++)
                        {
                            fsWrite.WriteByte(bytesWrite[i]);
                        }
                        fsWrite.Close();
                        continue;
                    }
                    file.CopyTo(path);
                }
            }
        }
        /*private void Element_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            imageObject.CaptureMouse();
        }

        private void Element_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            imageObject.ReleaseMouseCapture();
        }*/

        private void SetImageUI(UI ui)
        {
            try
            {
                imageUI.Source = GetBitmapImageFromAssets(ui.NameTexture);
            } catch(DirectoryNotFoundException e) { }
        }

        private static Scene GetScene()
        {
            return scenes[sceneSelectNow];
        }

        private static Scene GetScene(string nameScene)
        {
            foreach(Scene scene in scenes)
            {
                if (scene.Name.Equals(nameScene))
                {
                    //Console.WriteLine($"i founded scene {scene.Name}");
                    return scene;
                }
            }
            return null;
        }

        private void AddImageCamera()
        {
            BitmapImage bitmapImage = GetBitmapImageFromResourse("sceneCam.png");
            Image tem = new Image();
            tem.Stretch = Stretch.Fill;
            tem.Uid = "sceneCamView";
            RenderOptions.SetBitmapScalingMode(tem, BitmapScalingMode.NearestNeighbor);
            tem.Source = bitmapImage;

            canvas.Children.Add(tem);
            Canvas.SetBottom(tem, heightCanvas - tem.Height);
            Canvas.SetRight(tem, widthCanvas - tem.Width);

            updateGameObjectsOnScene();
        }

        private void AddImageFrame()
        {
            BitmapImage bitmapImage = GetBitmapImageFromResourse("frame.png");
            Image tem = new Image();
            tem.Stretch = Stretch.Fill;
            tem.Uid = "frameTexture";
            RenderOptions.SetBitmapScalingMode(tem, BitmapScalingMode.NearestNeighbor);
            tem.Source = bitmapImage;

            canvas.Children.Add(tem);
            Canvas.SetBottom(tem, widthCanvas - tem.Height);
            Canvas.SetRight(tem, heightCanvas - tem.Width);

            updateGameObjectsOnScene();
        }

        private static string StringToByteToString(string text)
        {
            byte[] nameFileByte = Encoding.UTF8.GetBytes(text);
            string nameFileFinal = "";
            foreach (byte _byte in nameFileByte)
            {
                nameFileFinal += _byte.ToString();
            }
            return nameFileFinal;
        }

        private static string RemoveEndNameFile(string text)
        {
            string nameFile = "";
            if (text.EndsWith(".png")) nameFile = text.Replace(".png", "");
            else if (text.EndsWith(".ogg")) nameFile = text.Replace(".ogg", "");
            else if (text.EndsWith(".ttf")) nameFile = text.Replace(".ttf", "");
            else if (text.EndsWith(".xml")) nameFile = text.Replace(".xml", "");
            else if (text.EndsWith(".tsx")) nameFile = text.Replace(".tsx", "");
            else if (text.EndsWith(".tmx")) nameFile = text.Replace(".tmx", "");
            //else if (text.EndsWith(".cs")) nameFile = text.Replace(".cs", "");
            else if (text.EndsWith(".txt")) nameFile = text.Replace(".txt", "");
            else if (text.EndsWith(".lua")) nameFile = text.Replace(".lua", "");
            else if (text.EndsWith(".bubla")) nameFile = text.Replace(".bubla", "");
            return nameFile;
        }

        private static string ReadFile(string path)
        {
            StreamReader fsRead = new StreamReader(File.Open(path, FileMode.Open));

            string text = "";
            int data;
            while ((data = fsRead.Read()) != -1)
            {
                text += (char)data;
            }
            text += "\n";
            fsRead.Close();

            return text;
        }
    }
}

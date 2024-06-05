using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_Winforms.utils
{
    internal class Option
    {
        private static string titleGame = "no name";
        private static string nameSceneStart = "mainScene";
        private static string iconGame = null;
        private static string imageMouse = null;
        private static string outPathOutFolder = null;
        private static string pathStartFile = null;
        private static string stackScript = null;
        private static byte[] colorBackground = new byte[3];
        private static int widhtScreen = 0;
        private static int heightScreen = 0;
        private static string sizeMouseTexture = "0";
        private static bool debug = false;
        private static bool deviceConnect = false;
        private static bool mouseInGame = true;
        //private static string fontPath = null;

        public static bool DebugMode
        {
            get { return debug; }
            set { debug = value; }
        }

        public static string StackScript
        {
            get { return stackScript; }
            set { stackScript = value; }
        }
        public static bool DeviceConnect
        {
            get { return deviceConnect; }
            set { deviceConnect = value; }
        }
        public static bool MouseInGame
        {
            get { return mouseInGame; }
            set { mouseInGame = value; }
        }

        public static string TitleGame
        {
            get { return titleGame; }
            set { titleGame = value; }
        }

        public static string NameSceneStart
        {
            get { return nameSceneStart; }
            set { nameSceneStart = value; }
        }
        public static string IconGame
        {
            get { return iconGame; }
            set { iconGame = value; }
        }
        public static string ImageMouse
        {
            get { return imageMouse; }
            set { imageMouse = value; }
        }
        public static string OutPathOutFolder
        {
            get { return outPathOutFolder; }
            set { outPathOutFolder = value; }
        }
        public static string PathStartFile
        {
            get { return pathStartFile; }
            set { pathStartFile = value; }
        }
        public static int WidhtScreen
        {
            get { return widhtScreen; }
            set { widhtScreen = value; }
        }
        public static int HeightScreen
        {
            get { return heightScreen; }
            set { heightScreen = value; }
        }
        public static string SizeMouseTexture
        {
            get { return sizeMouseTexture; }
            set { sizeMouseTexture = value; }
        }
        public static byte[] ColorBackground
        {
            get { return colorBackground; }
            set { colorBackground = value; }
        }
        /*public static string FontPath
        {
            get { return fontPath; }
            set { fontPath = value; }
        }*/
    }
}

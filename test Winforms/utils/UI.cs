using System.Windows;
using System;
using bubla.utils;
using test_Winforms.utils;
using System.Drawing;
using test_Winforms.Properties;

namespace Bubla.utils
{
    public class UI
    {
        private string name;
        private string nameTexture;
        private TypeUI typeUI;
        private Vector position;
        private Vector size;

        // for script
        private string sourseScript;
        private int layer;
        //private bool? globalScript;

        // for sound
        private bool? loop;
        private string sourseSound;
        private float volume;
        private bool? sound3D;

        // for label
        private string text;

        // for effect
        private int countParticle;
        private TypeEffect typeEffect;

        private Rgb rgbInside;
        private Rgb rgbOutSide;

        public UI(string name, TypeUI typeUI)
        {
            this.name = name;
            this.text = "text";
            this.typeUI = typeUI;
            sourseSound = "";
            sourseScript = "";
            nameTexture = "";
            position = new Vector(0, 0);
            size = new Vector(0, 0);
            countParticle = 0;
            loop = false;
            sound3D = false;
            layer = 0;
            rgbInside = new Rgb();
            rgbOutSide = new Rgb();
        }

        public Vector Position
        {
            get { return position; }
            set { position = value; }
        }

        public Rgb GetRgbInside()
        {
            return rgbInside;
        }

        public Rgb GetRgbOutside()
        {
            return rgbOutSide;
        }

        public void SetRgbInside(Rgb rgb)
        {
            rgbInside = rgb;
        }

        public void SetRgbOutside(Rgb rgb)
        {
            rgbOutSide = rgb;
        }

        public void SetPositionX(float x)
        {
            position.X = x;
        }

        public void SetPositionY(float y)
        {
            position.Y = y;
            Console.WriteLine(position.Y);
        }

        public Vector Size
        {
            get { return size; }
            set { size = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Layer
        {
            get { return layer; }
            set { layer = value; }
        }

        public bool? Loop
        {
            get { return loop; }
            set { loop = value; }
        }

        public bool? Sound3D
        {
            get { return sound3D; }
            set { sound3D = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string NameTexture
        {
            get { return nameTexture; }
            set { nameTexture = value; }
        }

        public string SourseSound
        {
            get { return sourseSound; }
            set { sourseSound = value; }
        }

        public string SourseScript
        {
            get { return sourseScript; }
            set { sourseScript = value; }
        }

        public TypeUI TypeUI
        {
            get { return typeUI; }
            set { typeUI = value; }
        }

        public TypeEffect TypeEffect
        {
            get { return typeEffect; }
            set { typeEffect = value; }
        }

        public int CountParticle
        {
            get { return countParticle; }
            set { countParticle = value; }
        }

        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public UI Clone()
        {
            UI ui = new UI(name + new Random().Next(999999), typeUI);
            ui.NameTexture = nameTexture;
            ui.Position = Position;
            ui.SourseSound = SourseSound;
            //Console.WriteLine("scritp sourse = " + fileScript);
            ui.SourseScript = SourseScript;
            ui.Size = Size;
            ui.Text = text;
            ui.CountParticle = countParticle;
            ui.TypeEffect = typeEffect;
            ui.Loop = loop;
            ui.Sound3D = sound3D;
            ui.Volume = volume;
            ui.Layer = layer;
            ui.SetRgbOutside(rgbOutSide);
            ui.SetRgbInside(rgbInside);

            return ui;
        }
    }
}

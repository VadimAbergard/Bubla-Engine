using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace test_Winforms.utils
{
    public class GameObject
    {
        private string name;
        private string tag;
        private float rotate;
        private string nameTexture;
        private string animation;
        private Vector position;
        private Vector size;
        private Vector sizeAdditionCollider;
        private Vector posAdditionCollider;
        private Vector splitSpriteSize;
        private Vector splitSpritePos;
        private bool? point;
        private bool? resourse;
        private bool? smooth;
        private bool? repited;
        private bool? textureFullScreen;

        private string typeCollider;
        private bool? collider;
        private bool? trigger;
        private bool? fixedRotation;
        private bool? fixedRotationTexture;
        private float angle;
        private bool? splitSprite;
        private bool? splitSpritePixel;
        private float mass;

        private int layer;

        public GameObject(string name)
        {
            this.name = name;
            position = new Vector(0, 0);
            size = new Vector(0, 0);
            sizeAdditionCollider = new Vector(0, 0);
            posAdditionCollider = new Vector(0, 0);
            point = false;
            resourse = false;
            smooth = false;
            repited = false;
            collider = false;
            trigger = false;
            fixedRotation = false;
            fixedRotationTexture = false;
            splitSprite = false;
            splitSpritePixel = false;
        }

        public Vector Position
        {
            get { return position; }
            set { position = value; }
        }

        public int Layer
        {
            get { return layer; }
            set { layer = value; }
        }

        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public float Rotate
        {
            get { return rotate; }
            set { rotate = value; }
        }

        public string Animation
        {
            get { return animation; }
            set { animation = value; }
        }

        public Vector SplitSpriteSize
        {
            get { return splitSpriteSize; }
            set { splitSpriteSize = value; }
        }

        public Vector SplitSpritePos
        {
            get { return splitSpritePos; }
            set { splitSpritePos = value; }
        }

        public void SetPositionX(float x)
        {
            position.X = x;
        }

        public void SetPositionY(float y)
        {
            position.Y = y;
        }

        public void SetSplitSpriteSizeX(int x)
        {
            splitSpriteSize.X = x;
        }

        public void SetSplitSpriteSizeY(int y)
        {
            splitSpriteSize.Y = y;
        }

        public void SetSizeAdditonColliderX(int x)
        {
            sizeAdditionCollider.X = x;
        }

        public void SetSizeAdditonColliderY(int y)
        {
            sizeAdditionCollider.Y = y;
        }

        public void SetPosAdditonColliderX(int x)
        {
            posAdditionCollider.X = x;
        }

        public void SetPosAdditonColliderY(int y)
        {
            posAdditionCollider.Y = y;
        }

        public void SetSplitSpritePosX(int x)
        {
            splitSpritePos.X = x;
        }

        public void SetSplitSpritePosY(int y)
        {
            splitSpritePos.Y = y;
        }

        public Vector Size
        {
            get { return size; }
            set { size = value; }
        }

        public Vector SizeAdditionCollider
        {
            get { return sizeAdditionCollider; }
            set { sizeAdditionCollider = value; }
        }

        public Vector PosAdditionCollider
        {
            get { return posAdditionCollider; }
            set { posAdditionCollider = value; }
        }

        public string NameTexture
        {
            get { return nameTexture; }
            set { nameTexture = value; }
        }

        public string TypeCollider
        {
            get { return typeCollider; }
            set { typeCollider = value; }
        }

        public bool? TextureFullScreen
        {
            get { return textureFullScreen; }
            set { textureFullScreen = value; }
        }

        public bool? SplitSpritePixel
        {
            get { return splitSpritePixel; }
            set { splitSpritePixel = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public bool? Point
        {
            get { return point; }
            set { point = value; }
        }

        public bool? Resourse
        {
            get { return resourse; }
            set { resourse = value; }
        }

        public bool? Smooth
        {
            get { return smooth; }
            set { smooth = value; }
        }

        public bool? Repited
        {
            get { return repited; }
            set { repited = value; }
        }

        public bool? Collider
        {
            get { return collider; }
            set { collider = value; }
        }

        public bool? SplitSprite
        {
            get { return splitSprite; }
            set { splitSprite = value; }
        }

        public bool? Trigger
        {
            get { return trigger; }
            set { trigger = value; }
        }

        public bool? FixedRotation
        {
            get { return fixedRotation; }
            set { fixedRotation = value; }
        }

        public bool? FixedRotationTexture
        {
            get { return fixedRotationTexture; }
            set { fixedRotationTexture = value; }
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public GameObject Clone()
        {
            GameObject gameObject = new GameObject(name);
            gameObject.NameTexture = nameTexture;
            gameObject.Position = position;
            gameObject.Size = size;
            gameObject.Point = point;
            gameObject.Resourse = resourse;

            gameObject.Collider = collider;
            gameObject.Trigger = trigger;
            gameObject.Mass = mass;
            gameObject.SizeAdditionCollider = SizeAdditionCollider;
            gameObject.PosAdditionCollider = PosAdditionCollider;

            gameObject.SplitSprite = splitSprite;
            gameObject.SplitSpritePixel = splitSpritePixel;
            gameObject.SplitSpriteSize = SplitSpriteSize;
            gameObject.SplitSpritePos = SplitSpritePos;

            gameObject.Layer = layer;

            gameObject.Tag = tag;
            gameObject.Rotate = rotate;
            gameObject.Smooth = smooth;
            gameObject.Repited = repited;

            gameObject.FixedRotation = fixedRotation;
            gameObject.FixedRotationTexture = fixedRotationTexture;
            gameObject.Angle = angle;
            gameObject.TypeCollider = typeCollider;

            gameObject.Animation = animation;

            return gameObject;
        }
    }
}

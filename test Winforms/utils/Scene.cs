using Bubla.utils;
using System.Collections.Generic;

namespace test_Winforms.utils
{
    public class Scene
    {
        private string name;
        private List<GameObject> gameObjects;
        private List<UI> uis;

        public Scene(string name) 
        { 
            this.name = name;
            gameObjects = new List<GameObject>();
            uis = new List<UI>();
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<GameObject> GameObjects
        {
            get { return gameObjects; }
            set { gameObjects = value; }
        }

        public List<UI> Uis
        {
            get { return uis; }
        }
    }
}

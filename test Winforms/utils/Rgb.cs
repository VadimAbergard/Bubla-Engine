using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_Winforms.utils
{
    public class Rgb
    {
        private byte r;
        private byte g;
        private byte b;

        public Rgb(byte r = 0, byte g = 0, byte b = 0)
        {
            this.r = r; 
            this.g = g;
            this.b = b;
        }

        public byte R
        {
            get { return r; }
            set { r = value; }
        }

        public byte G
        {
            get { return g; }
            set { g = value; }
        }

        public byte B
        {
            get { return b; }
            set { b = value; }
        }
    }
}

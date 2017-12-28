using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opengl
{
    class Texture2D
    {
        public int Id { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Texture2D(int id, int width, int height)
        {
            Id = id;
            Width = width;
            Height = height;
        }
    }
}

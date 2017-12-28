using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;


namespace opengl
{
    class Program
    {
        static void Main(string[] args)
        {
            Game window = new Game(680, 480);
            window.Run();
        }
    }
}

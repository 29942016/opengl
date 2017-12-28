using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;
using System.IO;
using System.Xml;

namespace opengl.Engine
{
    public struct Level
    {
        public Point PlayerStartPosition;

        public string Filename { get; private set; }
        public Block this[int x, int y]
        {
            get { return _Grid[x, y]; }
            set { _Grid[x, y] = value; }
        }
        public int Width
        {
            get { return _Grid.GetLength(0); }
        }
        public int Height
        {
            get { return _Grid.GetLength(1); }
        }

        private Block[,] _Grid;

        /// <summary>
        /// Load a default level
        /// </summary>
        public Level(int width, int height)
        {
            _Grid = new Block[width, height];
            Filename = "none";
            PlayerStartPosition = new Point(1, 1);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        _Grid[x, y] = new Block(BlockType.Solid, x, y);
                    }
                    else
                    {
                        _Grid[x, y] = new Block(BlockType.Empty, x, y);
                    }
        }

        /// <summary>
        /// Load a level from a filepath (.tmx)
        /// </summary>
        public Level(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(stream);

                    int width = int.Parse(doc.DocumentElement.GetAttribute("width"));
                    int height = int.Parse(doc.DocumentElement.GetAttribute("height"));

                    _Grid = new Block[width, height];
                    Filename = filePath;
                    PlayerStartPosition = new Point(1, 1);

                    XmlNode tileLayer = doc.DocumentElement.SelectSingleNode("layer[@name='Tile Layer 1']");
                    XmlNodeList tiles = tileLayer.SelectSingleNode("data").SelectNodes("tile");

                    int x = 0,
                        y = 0;

                    for (int i = 0; i < tiles.Count; i++)
                    {
                        int gid = int.Parse(tiles[i].Attributes["gid"].Value);

                        switch (gid)
                        {
                            case 1033:
                                _Grid[x, y] = new Block(BlockType.Solid, x, y);
                                break;
                            case 210:
                                _Grid[x, y] = new Block(BlockType.Hover, x, y);
                                break;
                            case 365:
                                _Grid[x, y] = new Block(BlockType.Object, x, y);
                                break;
                                _Grid[x, y] = new Block(BlockType.Empty, x, y);
                                break;
                        }


                        x++;
                        if (x >= width)
                        {
                            x = 0;
                            y++;
                        }
                    }

                    XmlNode objectGroup = doc.DocumentElement.SelectSingleNode("objectgroup[@name='Object Layer 1']");
                    XmlNodeList objects = objectGroup.SelectNodes("object");

                    for (int i = 0; i < objects.Count; i++)
                    {
                        int xPos = int.Parse(objects[i].Attributes["x"].Value);
                        int yPos = int.Parse(objects[i].Attributes["y"].Value);

                        switch (objects[i].Attributes["name"].Value)
                        {
                            case "playerStartPos":
                                PlayerStartPosition = new Point(((int) xPos / 128), (int)(yPos / 128));
                                break;
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"error loading level - {e.InnerException}");

                int width = 20,
                   height = 20;

                _Grid = new Block[width, height];
                Filename = "none";
                PlayerStartPosition = new Point(1, 1);

                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                        {
                            _Grid[x, y] = new Block(BlockType.Solid, x, y);
                        }
                        else
                        {
                            _Grid[x, y] = new Block(BlockType.Empty, x, y);
                        }
            }
        }
    }

    public struct Block
    {
        public BlockType Type { get; private set; }
        public int X          { get; private set; }
        public int Y          { get; private set; }

        public bool IsSolid    { get; private set; }
        public bool IsObject   { get; private set; }
        public bool IsHover { get; private set; }

        public Block(BlockType blocktype, int x, int y)
        {
            X = x;
            Y = y;
            Type = blocktype;

            IsObject = false;
            IsSolid = false;
            IsHover = false;

            switch (blocktype)
            {
                case BlockType.Object:
                    IsObject = true;
                    break;
                case BlockType.Solid:
                    IsSolid = true;
                    break;
                case BlockType.Hover:
                    IsHover = true;
                    break;
            }

        }


    }

    public enum BlockType
    {
        Solid,
        Hover,
        Object,
        Empty
    }
}

using opengl.Engine;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using opengl.Engine;

namespace opengl
{
    class Game : GameWindow
    {
        public static int GRIDSIZE = 64,
                          TILESIZE = 16;

        private Texture2D _TileSet;
        private View _View;
        private Level _Level;
        private TextRenderer _TextRenderer;
        private Player _Player;

        public Game(int width, int height) : base(width, height)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            _View = new View(Vector2.Zero, 1.0, 0);
            _TextRenderer = new TextRenderer(Width, Height);
            Input.Initialize(this);
        }

        #region Public Methods

        #endregion

        #region Private Methods

        private void RenderMap()
        {
            GL.Enable(EnableCap.Blend);
            GL.BindTexture(TextureTarget.Texture2D, _TileSet.Id);

            for (int x = 0; x < _Level.Width; x++)
            {
                for (int y = 0; y < _Level.Height; y++)
                {
                    RectangleF source = new RectangleF(0, 0, 0, 0);

                    switch (_Level[x, y].Type)
                    {
                        case BlockType.Solid:
                            source = new RectangleF(1 * TILESIZE, 0 * TILESIZE, TILESIZE, TILESIZE);
                            break;
                        case BlockType.Hover:
                            source = new RectangleF(2 * TILESIZE, 0 * TILESIZE, TILESIZE, TILESIZE);
                            break;
                        case BlockType.Object:
                            source = new RectangleF(3 * TILESIZE, 0 * TILESIZE, TILESIZE, TILESIZE);
                            break;
                    }

                    SpriteBatch.Draw(_TileSet, new Vector2(x * GRIDSIZE, y * GRIDSIZE), new Vector2((float)GRIDSIZE / TILESIZE), Color.White, Vector2.Zero, source);
                }
            }

            GL.End();
            GL.Disable(EnableCap.Blend);
        }

        private void RenderPlayer()
        {
            GL.Enable(EnableCap.Blend);

            _Player.Draw();

            GL.Disable(EnableCap.Blend);
        }

        private void RenderUI()
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.DarkGray);
            GL.Vertex2(-1.0f, -1.0f);   // BL
            GL.Vertex2(1.0f, -1.0f);    // BR
            GL.Vertex2(1.0f, -0.55f);   // TR
            GL.Vertex2(-1.0f, -0.55f);  // TL

            GL.End();
        }

        private void RenderFPS(FrameEventArgs e)
        {
            GL.Enable(EnableCap.Blend);

            _TextRenderer.DrawString((1f / e.Time).ToString("0."),
                InternalResources.FONT_MONO,
                InternalResources.BRUSH_WHITE,
                new Point(0, 0));

            GL.BindTexture(TextureTarget.Texture2D, _TextRenderer.TextureId);

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1f, -1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1f, -1f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1f, 1f);
            GL.End();

            GL.Disable(EnableCap.Blend);
        }

        #endregion

        #region Overrides

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _TileSet = ContentPipe.LoadTexture("Levels/FF1-16x16.png");
            _Level = new Level("Content\\Levels\\new.tmx");
            _Player = new Player(new Vector2(_Level.PlayerStartPosition.X + 0.5f, _Level.PlayerStartPosition.Y + 0.5f) * GRIDSIZE);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _Player.Update();

            if (Input.MouseDown(OpenTK.Input.MouseButton.Left))
            {
                Vector2 pos = new Vector2(Mouse.X, Mouse.Y) - (new Vector2(this.Width, this.Height)) / 2f;
                pos = _View.ToWorld(pos);
                _View.SetPosition(pos, View.TweenType.QuarticOut, 60);
            }
            if (Input.KeyDown(OpenTK.Input.Key.Right))
            {
                _View.SetPosition(_View.PositionGoTo + new Vector2(5, 0), View.TweenType.QuarticOut, 5);
            }
            if (Input.KeyDown(OpenTK.Input.Key.Up))
            {
                _View.SetPosition(_View.PositionGoTo + new Vector2(0, -5), View.TweenType.QuarticOut, 5);
            }
            if (Input.KeyDown(OpenTK.Input.Key.Left))
            {
                _View.SetPosition(_View.PositionGoTo + new Vector2(-5, 0), View.TweenType.QuarticOut, 5);
            }
            if (Input.KeyDown(OpenTK.Input.Key.Down))
            {
                _View.SetPosition(_View.PositionGoTo + new Vector2(0, 5), View.TweenType.QuarticOut, 5);
            }

            _View.Update();
           
            Input.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            SpriteBatch.Begin(Width, Height);

            _View.ApplyTransform();
            RenderMap();
            RenderPlayer();

            _View.ApplyUiTransform();
            RenderUI();
            RenderFPS(e);

            SwapBuffers();
        }

        #endregion
    }
}

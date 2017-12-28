using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using System.Drawing;

namespace opengl.Engine
{
    class Player
    {
        public Vector2 Position, Velocity;

        public RectangleF ColRec
        {
            get
            {
                return new RectangleF(Position.X - _Size.X / 2f, Position.Y - _Size.Y / 2f, _Size.X, _Size.Y);
            }
        }
        public RectangleF DrawRec
        {
            get
            {
                return new RectangleF(ColRec.X - 5, ColRec.Y, ColRec.Width + 10, ColRec.Height);
            }
        }

        private Vector2 _Size;

        private Texture2D _PlayerSprite;
        private bool _Climbing, _FacingRight, _OnLadder, _Grounded;

        public Player(Vector2 startPos)
        {
            Position = startPos;
            Velocity = Vector2.Zero;
            _Climbing = false;
            _FacingRight = false;
            _OnLadder = false;
            _Grounded = false;
            _Size = new Vector2(40, 80);
            _PlayerSprite = ContentPipe.LoadTexture("player.png");
        }

        public void Update()
        {
            HandleInput();

            Position.Y += Velocity.Y;

            ResolveCollision();
        }

        public void HandleInput()
        {
            if (Input.KeyDown(Key.W))
                Velocity.Y = -0.2f;
            else
                Velocity.Y = 0;

        }

        public void ResolveCollision()
        {

        }

        public void Draw()
        {
            SpriteBatch.DrawSprite(_PlayerSprite, DrawRec);
            //if (_Climbing)
            //{


            //    SpriteBatch.Draw(_PlayerSprite,
            //        this.Position,
            //        new Vector2(DrawRec.Width / _PlayerSprite.Width, DrawRec.Height / _PlayerSprite.Height),
            //        Color.White,
            //        new Vector2(_PlayerSprite.Width / 4f, _PlayerSprite.Height / 2f),
            //        new RectangleF(0/*_PlayerSprite.Width / 2f*/, 0, _PlayerSprite.Width / 2f, _PlayerSprite.Height));
            //}
            //else
            //{
            //    SpriteBatch.Draw(_PlayerSprite,
            //        this.Position,
            //        new Vector2(DrawRec.Width / _PlayerSprite.Width, DrawRec.Height / _PlayerSprite.Height),
            //        Color.White,
            //        new Vector2(_PlayerSprite.Width / 4f, _PlayerSprite.Height / 2f),
            //        new RectangleF(0, 0, _PlayerSprite.Width / 2f, _PlayerSprite.Height));
            //}
        }
    }
}

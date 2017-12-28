using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace opengl
{
    class View
    {
        public enum TweenType
        {
            Instant,
            Linear,
            QuadraticInOut,
            CubicInOut,
            QuarticOut
        }

        public Vector2 Position { get; private set; }
        /// <summary>
        /// + = clockwise
        /// - = counterclockwise
        /// </summary>
        public Double Rotation;
        /// <summary>
        /// 1 = no zoom
        /// 2 = 2x zoom
        /// </summary>
        public double Zoom;

        public Vector2 PositionGoTo, PositionFrom;
        private TweenType _TweenType;
        private int currentStep, tweenStep;

        public View(Vector2 startPosition, double startZoom = 1.0, double startRotation = 0.0)
        {
            this.Position = startPosition;
            this.Zoom = startZoom;
            this.Rotation = startRotation;
        }

        #region Public Methods

        public Vector2 ToWorld(Vector2 input)
        {
            input /= (float)Zoom;

            Vector2 dx = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            Vector2 dy = new Vector2((float)Math.Cos(Rotation + MathHelper.PiOver2), (float)Math.Sin(Rotation + MathHelper.PiOver2));

            return (Position + dx * input.X + dy * input.Y);
        }

        public void Update()
        {
            if (currentStep < tweenStep)
            {
                currentStep++;

                switch (_TweenType)
                {
                    case TweenType.Linear:
                        Position = PositionFrom + (PositionGoTo - PositionFrom) * GetLinear((float)currentStep / tweenStep);
                        break;
                    case TweenType.QuadraticInOut:
                        Position = PositionFrom + (PositionGoTo - PositionFrom) * GetQuadratic((float)currentStep / tweenStep);
                        break;
                    case TweenType.CubicInOut:
                        Position = PositionFrom + (PositionGoTo - PositionFrom) * GetCubic((float)currentStep / tweenStep);
                        break;
                    case TweenType.QuarticOut:
                        Position = PositionFrom + (PositionGoTo - PositionFrom) * GetQuartic((float)currentStep / tweenStep);
                        break;
                }
            }
            else
            {
                Position = PositionGoTo;
            }

        }

        public void ApplyTransform()
        {
            Matrix4 transform = Matrix4.Identity;
            transform = Matrix4.Mult(transform, Matrix4.CreateTranslation(-Position.X, -Position.Y, 0));
            transform = Matrix4.Mult(transform, Matrix4.CreateRotationZ(-(float)Rotation));
            transform = Matrix4.Mult(transform, Matrix4.CreateScale((float)Zoom, (float)Zoom, 1.0f));

            GL.MultMatrix(ref transform);
        }

        public void ApplyUiTransform()
        {
            GL.LoadIdentity();
        }

        public void SetPosition(Vector2 newPosition)
        {
            Position = newPosition;
            PositionFrom = newPosition;
            PositionGoTo = newPosition;
            _TweenType = TweenType.Instant;
            currentStep = 0;
            tweenStep = 0;
        }

        public void SetPosition(Vector2 newPosition, TweenType type, int steps)
        {
            PositionFrom = Position;
            Position = newPosition;
            PositionGoTo = newPosition;
            _TweenType = type;
            currentStep = 0;
            tweenStep = steps;
        }

        #endregion

        #region Private Methods

        private float GetLinear(float t)
        {
            return t;
        }

        private float GetQuadratic(float t)
        {
            return (t * t) / ((2 * t * t) - (2 * t) + 1);
        }

        private float GetCubic(float t)
        {
            return (t * t * t) / ((3 * t * t) - (3 * t) + 1);
        }

        private float GetQuartic(float t)
        {
            return -((t - 1) * (t - 1) * (t - 1) * (t -1)) + 1;
        }

        #endregion
    }
}

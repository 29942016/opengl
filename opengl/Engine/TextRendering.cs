using System;
using System.Drawing;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace opengl.Engine
{
        public class TextRenderer
        {
            private Bitmap _Bmp;
            private Graphics _Gfx;
            private int _TextureId;
            private Rectangle _DirtyRegion;
            private bool _Disposed;

            #region Constructors

            /// <summary>
            /// Constructs a new instance.
            /// </summary>
            /// <param name="width">The width of the backing store in pixels.</param>
            /// <param name="height">The height of the backing store in pixels.</param>
            public TextRenderer(int width, int height)
            {
                if (width <= 0)
                    throw new ArgumentOutOfRangeException("width");
                if (height <= 0)
                    throw new ArgumentOutOfRangeException("height ");
                if (GraphicsContext.CurrentContext == null)
                    throw new InvalidOperationException("No GraphicsContext is current on the calling thread.");

                _Bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                _Gfx = Graphics.FromImage(_Bmp);
                _Gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                _TextureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, _TextureId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            }

            #endregion

            #region Public Members

            /// <summary>
            /// Clears the backing store to the specified color.
            /// </summary>
            /// <param name="color">A <see cref="System.Drawing.Color"/>.</param>
            public void Clear(Color color)
            {
                _Gfx.Clear(color);
                _DirtyRegion = new Rectangle(0, 0, _Bmp.Width, _Bmp.Height);
            }

            /// <summary>
            /// Draws the specified string to the backing store.
            /// </summary>
            /// <param name="text">The <see cref="System.String"/> to draw.</param>
            /// <param name="font">The <see cref="System.Drawing.Font"/> that will be used.</param>
            /// <param name="brush">The <see cref="System.Drawing.Brush"/> that will be used.</param>
            /// <param name="point">The location of the text on the backing store, in 2d pixel coordinates.
            /// The origin (0, 0) lies at the top-left corner of the backing store.</param>
            public void DrawString(string text, Font font, Brush brush, PointF point)
            {
                Clear(Color.Transparent);
                _Gfx.DrawString(text, font, brush, point);

                SizeF size = _Gfx.MeasureString(text, font);
                _DirtyRegion = Rectangle.Round(RectangleF.Union(_DirtyRegion, new RectangleF(point, size)));
                _DirtyRegion = Rectangle.Intersect(_DirtyRegion, new Rectangle(0, 0, _Bmp.Width, _Bmp.Height));
            }

            /// <summary>
            /// Gets a <see cref="System.Int32"/> that represents an OpenGL 2d texture handle.
            /// The texture contains a copy of the backing store. Bind this texture to TextureTarget.Texture2d
            /// in order to render the drawn text on screen.
            /// </summary>
            public int TextureId
            {
                get
                {
                    UploadBitmap();
                    return _TextureId;
                }
            }

            #endregion

            #region Private Members

            // Uploads the dirty regions of the backing store to the OpenGL texture.
            void UploadBitmap()
            {
                if (_DirtyRegion != RectangleF.Empty)
                {
                    System.Drawing.Imaging.BitmapData data = _Bmp.LockBits(_DirtyRegion,
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.BindTexture(TextureTarget.Texture2D, _TextureId);
                    GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                        _DirtyRegion.X, _DirtyRegion.Y, _DirtyRegion.Width, _DirtyRegion.Height,
                        PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                    _Bmp.UnlockBits(data);

                    _DirtyRegion = Rectangle.Empty;
                }
            }

            #endregion

            #region IDisposable Members

            void Dispose(bool manual)
            {
                if (!_Disposed)
                {
                    if (manual)
                    {
                        _Bmp.Dispose();
                        _Gfx.Dispose();
                        if (GraphicsContext.CurrentContext != null)
                            GL.DeleteTexture(_TextureId);
                    }

                    _Disposed = true;
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            ~TextRenderer()
            {
                Console.WriteLine("[Warning] Resource leaked: {0}.", typeof(TextRenderer));
            }

            #endregion
        }
    }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using System.Drawing;
using System.Drawing.Imaging;

namespace opengl.Engine
{
    public static class RenderHelper
    {
        private static SolidBrush _TextBrush = new SolidBrush(Color.Red);

        public static void DrawString(string text)
        {
            int texture = GL.GenTexture();

            using (Bitmap bmp = new Bitmap(250, 250))
            {
                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.Clear(Color.Transparent);
                g.DrawString(text, SystemFonts.DefaultFont, _TextBrush, 0, 0);


                Rectangle bounds = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData lockData = bmp.LockBits(bounds, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, bounds.X, bounds.Y, bounds.Width, bounds.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, lockData.Scan0);

                bmp.UnlockBits(lockData);
            }
        }
    }
}

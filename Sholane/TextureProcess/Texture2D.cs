using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace Sholane.TextureProcess
{
    internal class Texture2D : IDisposable
    {
        private int id, width, height, bufferID;
        private float[] coordinates;
        internal Texture2D (string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Файл не был найден.\nВведённый путь: \"{path}\"");

            Bitmap bmp = new Bitmap(path);
            width = bmp.Width;
            height = bmp.Height;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            bmp.UnlockBits(data);

            coordinates = new float[]
            {
                0.0f, 0.0f, // Левый нижний
                1.0f, 0.0f, // Правый нижний
                1.0f, 1.0f, // Правый верхний
                0.0f, 1.0f, // Левый верхний
                
                
            };
            bufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, coordinates.Length*sizeof(float), coordinates, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        internal void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);
        }
        internal void UnBind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        public void Dispose()
        {
            GL.DeleteBuffer(bufferID);
            GL.DeleteTexture(id);
        }
    }
}
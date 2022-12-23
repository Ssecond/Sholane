using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace Sholane.TextureProcess
{
    internal class Texture : IDisposable
    {
        private int id, bufferID;
        private int width, height;
        private float[] coordinates;

        private int columns, rows;
        private double frameTimer;
        private double time;
        private int framesCount = 1;
        private int currentFrame;
        internal Texture (string path, int columns = 1, int rows = 1)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Файл не был найден.\nВведённый путь: \"{path}\"");

            this.columns = columns; 
            this.rows = rows;
            this.currentFrame = columns;
            this.framesCount = columns * rows;

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
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            bmp.UnlockBits(data);

            coordinates = new float[]
            {
                // First two actually upper, last - bottom, coz I needed to rotate the image.
                0.0f, 1 - 1.0f / rows, // Bottom left
                1.0f / columns, 1 - 1.0f / rows, // Bottom right
                1.0f / columns, 1.0f, // Upper right
                0.0f, 1.0f, // Upper left
            };
            bufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, coordinates.Length*sizeof(float), coordinates, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        internal void Update(double time)
        { 
            this.time = time;
        }
        private void UpdateCoordinates(int currentFrame)
        {
            int rowNumber = currentFrame / columns;
            int columnNumber = currentFrame - rowNumber * columns;
            float stepWidth = 1.0f / columns;
            float stepHeight = 1.0f / rows;
            coordinates = new float[]
            {
                stepWidth * columnNumber, 1.0f - (rowNumber + 1) * stepHeight, // Bottom left
                stepWidth * columnNumber + stepWidth, 1.0f - (rowNumber + 1) * stepHeight, // Bottom right
                stepWidth * columnNumber + stepWidth, 1.0f - rowNumber * stepHeight,  // Upper right
                stepWidth * columnNumber, 1.0f - rowNumber * stepHeight, // Upper left
            };
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, coordinates.Length * sizeof(float), coordinates, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        internal void Bind(double timePerFrame)
        {
            if (timePerFrame > 0 && framesCount > 1)
            {
                UpdateCoordinates(currentFrame);
                frameTimer += time;
                if (frameTimer > timePerFrame)
                {
                    currentFrame++;
                    if (currentFrame > framesCount - 1)
                        currentFrame = 0;
                    frameTimer = 0;
                }
            }
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
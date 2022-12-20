using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Sholane.TextureProcess
{
    internal class Background : IDisposable
    {
        private int vertexBufferID;
        private float[] vertexData;
        private Texture2D texture;

        public Background(int width, int height, string path)
        {
            texture = new Texture2D(path);
            vertexData = new float[]
            {
                0.0f, 0.0f,
                width, 0.0f,
                width, height,
                0.0f, height,
            };

            vertexBufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        internal void Resize(int width, int height)
        {
            vertexData = new float[]
            {
                0.0f, 0.0f,
                width, 0.0f,
                width, height,
                0.0f, height,
            };
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        internal void Draw()
        {
            EnableStates();
            texture.Bind();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);
            GL.VertexPointer(2, VertexPointerType.Float, 0, 0);
            GL.DrawArrays(PrimitiveType.Quads, 0, vertexData.Length);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            texture.UnBind();
            DisableStates();
        }
        private void EnableStates()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
        }
        private void DisableStates()
        {
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

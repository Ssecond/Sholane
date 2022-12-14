using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Sholane.TextureProcess;

namespace Sholane.GameObjects
{
    internal class Entity : IDisposable
    {
        private int vertexBufferID, width, height;
        private float[] vertexData;
        private Texture texture;
        private Vector2 position;
        private BufferUsageHint bufferUsageHint;

        internal Vector2 Position { get => position; }
        internal int Width { get => width; }
        internal int Height { get => height; }
        internal float Speed { get; }

        internal Entity(int width, int height, Texture texture, Vector2 position, BufferUsageHint bufferUsageHint = BufferUsageHint.StaticDraw, float speed = 0.0f)
        {
            this.texture = texture;
            this.position = position;
            this.width = width;
            this.height = height;
            this.bufferUsageHint = bufferUsageHint;
            this.Speed = speed;
            vertexData = new float[]
            {
                position.X + 0.0f,  position.Y + 0.0f, // Upper left
                position.X + width, position.Y + 0.0f, // Upper right
                position.X + width, position.Y + height, // Bottom right
                position.X + 0.0f,  position.Y + height, // Bottom left
            };

            vertexBufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, bufferUsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        internal void Move(Vector2 vector)
        {
            position += vector;
            vertexData = new float[]
            {
                position.X + 0.0f,  position.Y + 0.0f, // Upper left
                position.X + width, position.Y + 0.0f, // Upper right
                position.X + width, position.Y + height, // Bottom right
                position.X + 0.0f,  position.Y + height, // Bottom left
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, bufferUsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        internal void Update(double time)
        {
            texture.Update(time);
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
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, bufferUsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        internal void Draw(double timePerFrame = 0)
        {
            EnableStates();
            texture.Bind(timePerFrame);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferID);
            GL.VertexPointer(2, VertexPointerType.Float, 0, 0);
            GL.DrawArrays(PrimitiveType.Quads, 0, vertexData.Length);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            texture.UnBind();
            DisableStates();
        }
        private void EnableStates()
        {
            GL.Enable(EnableCap.Blend);// Подключаем режим отображения текстур
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
        }
        private void DisableStates()
        {
            GL.Disable(EnableCap.Blend);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
        }
        public void Dispose()
        {
            texture.Dispose();
            GL.DeleteBuffer(vertexBufferID);
        }
    }
}

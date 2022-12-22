using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Sholane.TextureProcess;

namespace Sholane.GameObjects
{
    internal delegate void MouseDelegate();
    internal class Button : Entity
    {
        private int width, height;
        private Vector2 position;

        internal MouseDelegate OnMouseDown;
        internal Button(int width, int height, Texture2D texture, Vector2 position, BufferUsageHint bufferUsageHint = BufferUsageHint.StaticDraw, float speed = 0) : base(width, height, texture, position, bufferUsageHint, speed)
        {
            this.width = width;
            this.height = height;
            this.position = position;
        }
        internal bool IsPointInFigure(Vector2 point)
        {
            if (point.X < position.X)
                return false;
            if (point.Y < position.Y)
                return false;
            if (point.X > position.X + width)
                return false;
            if (point.Y > position.Y + height)
                return false;
            return true;
        }
    }
}
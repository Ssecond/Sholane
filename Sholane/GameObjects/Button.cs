using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Sholane.TextureProcess;

namespace Sholane.GameObjects
{
    internal delegate void MouseDelegate();
    internal class Button : Entity
    {
        internal MouseDelegate OnMouseDown;
        internal Button(int width, int height, Texture texture, Vector2 position, BufferUsageHint bufferUsageHint = BufferUsageHint.StaticDraw, float speed = 0) : base(width, height, texture, position, bufferUsageHint, speed) {}
        internal bool IsPointInFigure(Vector2 point)
        {
            if (point.X < Position.X)
                return false;
            if (point.Y < Position.Y)
                return false;
            if (point.X > Position.X + Width)
                return false;
            if (point.Y > Position.Y + Height)
                return false;
            return true;
        }
    }
}
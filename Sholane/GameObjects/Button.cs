using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Sholane.GameObjects
{
    internal delegate void MouseDelegate(MouseButtonEventArgs e);
    internal class Button
    {
        private float x, y;
        private float width, height;
        internal MouseDelegate OnMouseDown;
        internal Button(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        internal bool IsPointInFigure(Vector2 point)
        {
            if (point.X < x)
                return false;
            if (point.Y < y)
                return false;
            if (point.X > x + width)
                return false;
            if (point.Y > y + height)
                return false;
            return true;
        }
    }
}
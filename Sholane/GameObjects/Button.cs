using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Sholane.GameObjects
{
    delegate void MouseDelegate(MouseButtonEventArgs e);
    internal class Button
    {
        float x, y, width, height;
        Color4 color;
        public MouseDelegate OnMouseDown;
        public Button(float x, float y, float width, float height, Color4 color)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.color = color;
        }
        public void Draw()
        {
            GL.Color4(color);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x, y + height);
            GL.Vertex2(x + width, y + height);
            GL.Vertex2(x + width, y);
            GL.End();
        }
        public bool IsPointInFigure(float pointX, float pointY)
        {
            if (pointX < x) return false;
            if (pointY < y) return false;
            if (pointX > x + width) return false;
            if (pointY > y + height) return false;
            return true;
        }
        public bool IsPointInFigure(Vector2 point)
        {
            return IsPointInFigure(point.X, point.Y);
        }
    }
}
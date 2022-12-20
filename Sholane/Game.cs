using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sholane.TextureProcess;

namespace Sholane
{
    internal class Game : GameWindow
    {
        const float deltaMove = 0.05f;
        private Matrix4 ortho;
        private Entity rocket, background;
        private List<Entity> planes = new List<Entity>();
        private List<Entity> platforms = new List<Entity>();
        private double lag = 0, TIME_PER_FRAME = 0.001;
        private Keys lastKeyboardState;
        internal Game(GameWindowSettings gSettings, NativeWindowSettings nSettings) : base(gSettings, nSettings)
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.Texture2D);

            rocket = new Entity(33, 60, "Content\\Rocket.png", new Vector2(nSettings.Size.Y / 2 - 15, nSettings.Size.X - 60), BufferUsageHint.DynamicDraw);
            planes.Add(new Entity(60, 28, "Content\\Plane.png", Vector2.Zero, BufferUsageHint.DynamicDraw));
            background = new Entity(nSettings.Size.X, nSettings.Size.Y, "Content\\Sky.jpg", Vector2.Zero);
            platforms.Add(new Entity(103, 10, "Content\\Platform.png", Vector2.Zero, BufferUsageHint.DynamicDraw));
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Random random = new Random();
            for (int x = 0; x < 5; x++)
                planes.Add(new Entity(60, 28, "Content\\Plane.png", new Vector2(0, random.Next(0, this.Size.Y - 28)), BufferUsageHint.DynamicDraw));
            for (int x = 0; x < 5; x++)
                platforms.Add(new Entity(103, 10, "Content\\Platform.png", new Vector2(random.Next(0, this.Size.X - 103), random.Next(0, this.Size.Y - 10)), BufferUsageHint.DynamicDraw));
        }
        protected override void OnUnload()
        {
            base.OnUnload();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, this.Size.X, this.Size.Y);

            ortho = Matrix4.CreateOrthographicOffCenter(0, this.Size.X, this.Size.Y, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref ortho);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            background.Resize(this.Size.X, this.Size.Y);
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            lag += args.Time;
            if (lag > TIME_PER_FRAME)
            {
                while (lag > TIME_PER_FRAME)
                {
                    MoveEntities(lastKeyboardState);
                    lag -= TIME_PER_FRAME;
                }
            }
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            background.Draw();
            rocket.Draw();
            PlaceObjectsOnMap();
            GL.LoadIdentity();
            GL.End();
            SwapBuffers();
        }
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            lastKeyboardState = e.Key;
        }
        private void MoveEntities(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                case Keys.W:
                    if (!OutsideBoarder(rocket.Position.X, rocket.Position.Y - deltaMove))
                        rocket.Move(new Vector2(0.0f, -deltaMove));
                    break;

                case Keys.Left:
                case Keys.A:
                    if (!OutsideBoarder(rocket.Position.X - deltaMove, rocket.Position.Y))
                        rocket.Move(new Vector2(-deltaMove, 0.0f));
                    break;

                case Keys.Down:
                case Keys.S:
                    if (!OutsideBoarder(rocket.Position.X, rocket.Position.Y + deltaMove + 60))
                        rocket.Move(new Vector2(0.0f, deltaMove));
                    break;

                case Keys.Right:
                case Keys.D:
                    if (!OutsideBoarder(rocket.Position.X + deltaMove + 33, rocket.Position.Y))
                        rocket.Move(new Vector2(deltaMove, 0.0f));
                    break;

                case Keys.Escape:
                    Close();
                    break;
            }
            for (int i = 0; i < planes.Count;i++)
                if (!OutsideBoarder(planes[i].Position.X + deltaMove + 60, planes[i].Position.Y + 28))
                    planes[i].Move(new Vector2(deltaMove, deltaMove));

            for (int i = 0; i < platforms.Count; i++)
                if (!OutsideBoarder(platforms[i].Position.X + deltaMove, platforms[i].Position.Y + 10))
                    platforms[i].Move(new Vector2(0.0f, deltaMove));
        }
        private bool OutsideBoarder(float x, float y)
        {
            if (x < 0 || y < 0 || x > this.Size.X || y > this.Size.Y)
                return true;
            return false;
        }
        private void PlaceObjectsOnMap()
        {
            foreach (Entity plane in planes)
                plane.Draw();
            foreach (Entity platform in platforms)
                platform.Draw();
        }
    }
}

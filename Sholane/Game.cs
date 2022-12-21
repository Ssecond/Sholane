using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sholane.TextureProcess;
using System.Diagnostics;
using System.Drawing;

namespace Sholane
{
    internal class Game : GameWindow
    {
        private const int offsetY = 50;
        private const int offsetX = 35;
        private Matrix4 ortho;
        private Entity rocket, background, gameEndScreen, gamePauseScreen;
        private List<Entity> planes;
        private List<Entity> platforms;
        private double lag = 0, TIME_PER_FRAME = 0.001;
        private Keys lastKeyboardState = Keys.W;
        private bool gamePaused = false;
        private bool gameEnd = false;

        private static int score;
        
        internal Game(GameWindowSettings gSettings, NativeWindowSettings nSettings) : base(gSettings, nSettings)
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.Texture2D);
        }
        private void Initialize()
        {
            score = 0;
            this.Title = "Очки: " + score;
            rocket = new Entity(33, 60, "Content\\Rocket.png", new Vector2(this.Size.X / 2 - 15, this.Size.Y - 60), BufferUsageHint.DynamicDraw, 0.08f);
            background = new Entity(this.Size.X, this.Size.Y, "Content\\Sky.jpg", Vector2.Zero);
            gameEndScreen = new Entity(this.Size.X, this.Size.Y, "Content\\GameOver.png", Vector2.Zero);
            gamePauseScreen = new Entity(this.Size.X, this.Size.Y, "Content\\Pause.png", Vector2.Zero);
            planes = new List<Entity>();
            platforms = new List<Entity>();
            Random random = new Random();
            for (int i = 0; i < 4; i++)
            {
                int y = random.Next(0, this.Size.Y - 28 - 60);
                while (i != 0 && Math.Abs(planes[i - 1].Position.Y - y) < offsetY)
                    y = random.Next(0, this.Size.Y - 28 - 60);

                planes.Add(new Entity(60, 28, "Content\\Plane.png", new Vector2(0, y), BufferUsageHint.DynamicDraw, 0.03f));
            }
            for (int i = 0; i < 5; i++)
            {
                int y = random.Next(0, this.Size.Y - 10 - 120);
                while (i != 0 && Math.Abs(planes[i - 1].Position.Y - y) < offsetY)
                    y = random.Next(0, this.Size.Y - 10 - 120);

                int x = random.Next(0, this.Size.X - 103);
                while (i != 0 && Math.Abs(planes[i - 1].Position.Y - y) < offsetX)
                    x = random.Next(0, this.Size.X - 103);

                platforms.Add(new Entity(103, 10, "Content\\Platform.png", new Vector2(x, y), BufferUsageHint.DynamicDraw, 0.05f));
            }
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            Initialize();
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
            if (lastKeyboardState == Keys.Escape)
                Close();
            else if (lastKeyboardState == Keys.P)
            {
                gamePaused = !gamePaused;
                lastKeyboardState = Keys.Unknown;
            }

            if (!gamePaused && !gameEnd)
            {
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
            else if (lastKeyboardState == Keys.R)
            {
                gameEnd = !gameEnd;
                Initialize();
                lastKeyboardState = Keys.Unknown;
            }
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            if (!gameEnd)
                if (!gamePaused)
                {
                    GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
                    PlaceObjectsOnMap();
                    GL.LoadIdentity();
                    GL.End();
                }
                else
                {
                    gamePauseScreen.Draw();
                }
            else
                gameEndScreen.Draw();
            SwapBuffers();
        }
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            lastKeyboardState = e.Key;
        }
        private Rectangle getBounds(Entity entity)
        {
            return new Rectangle((int)entity.Position.X, (int)entity.Position.Y, entity.Width, entity.Height);
        }
        private void MoveEntities(Keys key)
        {
            for (int i = 0; i < planes.Count; i++)
                if (getBounds(rocket).IntersectsWith(getBounds(planes[i])))
                {
                    planes.Remove(planes[i]);
                    planes.Add(new Entity(60, 28, "Content\\Plane.png", new Vector2(0, new Random().Next(0, this.Size.Y - 28 - 60)), BufferUsageHint.DynamicDraw, 0.03f));
                    rocket.Move(-rocket.Position + new Vector2(this.Size.X / 2 - 15, this.Size.Y - 60));
                    this.Title = "Очки: " + ++score;
                    break;
                }
            for (int i = 0; i < platforms.Count; i++)
                if (getBounds(rocket).IntersectsWith(getBounds(platforms[i])))
                {
                    gameEnd = true;
                    platforms.Remove(platforms[i]);
                    break;
                }
            switch (key)
            {
                case Keys.Up:
                case Keys.W:
                    if (!OutsideBoarder(rocket.Position.X, rocket.Position.Y - rocket.Speed))
                        rocket.Move(new Vector2(0.0f, -rocket.Speed));
                    break;

                case Keys.Left:
                case Keys.A:
                    if (!OutsideBoarder(rocket.Position.X - rocket.Speed, rocket.Position.Y))
                        rocket.Move(new Vector2(-rocket.Speed, 0.0f));
                    break;

                case Keys.Down:
                case Keys.S:
                    if (!OutsideBoarder(rocket.Position.X, rocket.Position.Y + rocket.Speed + rocket.Height))
                        rocket.Move(new Vector2(0.0f, rocket.Speed));
                    break;

                case Keys.Right:
                case Keys.D:
                    if (!OutsideBoarder(rocket.Position.X + rocket.Speed + rocket.Width, rocket.Position.Y))
                        rocket.Move(new Vector2(rocket.Speed, 0.0f));
                    break;
            }
            for (int i = 0; i < planes.Count; i++)
                if (!OutsideBoarder(planes[i].Position.X + planes[i].Speed, planes[i].Position.Y))
                    planes[i].Move(new Vector2(planes[i].Speed, planes[i].Speed));
                else
                {
                    if (planes[i].Position.X + planes[i].Speed >= this.Size.X)
                        planes[i].Move(new Vector2(-this.Size.X, 0.0f));
                    else
                        planes[i].Move(new Vector2(0.0f, -this.Size.Y));
                }

            for (int i = 0; i < platforms.Count; i++)
                if (!OutsideBoarder(platforms[i].Position.X + platforms[i].Speed, platforms[i].Position.Y))
                    platforms[i].Move(new Vector2(0.0f, platforms[i].Speed));
                else
                {
                    Random random = new Random();
                    platforms[i].Move(new Vector2(random.Next(-1, 1), -this.Size.Y));
                }
        }
        private bool OutsideBoarder(float x, float y)
        {
            if (x < 0 || y < 0 || x > this.Size.X || y > this.Size.Y)
                return true;
            return false;
        }
        private void PlaceObjectsOnMap()
        {
            background.Draw();
            rocket.Draw();
            foreach (Entity plane in planes)
                plane.Draw();
            foreach (Entity platform in platforms)
                platform.Draw();
        }
    }
}

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sholane.GameObjects;
using Sholane.TextureProcess;
using System.Drawing;

namespace Sholane
{
    internal class Game : GameWindow
    {
        private const int offsetY = 50;
        private const int offsetX = 35;
        private int buttonsWidth = 200;
        private int buttonsHeight = 100;
        private Matrix4 ortho;
        private Entity rocket, gameBackground;
        private List<Entity> planes;
        private List<Entity> platforms;
        private double lag = 0, TIME_PER_FRAME = 0.001;
        private Keys lastKeyboardState = Keys.W;
        private bool gameNotStarted = true;

        private Entity gameMainMenuScreen;
        Button start, exit;

        private static int score;
        
        internal Game(GameWindowSettings gSettings, NativeWindowSettings nSettings) : base(gSettings, nSettings)
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.Texture2D);
        }
        private void InitializeGame()
        {
            score = 0;
            this.Title = "Очки: " + score;
            rocket = new Entity(33, 60, new Texture2D("Content\\RocketSprite.png", true, 2, 1), new Vector2(this.Size.X / 2 - 15, this.Size.Y - 60), BufferUsageHint.DynamicDraw, 0.08f);
            gameBackground = new Entity(this.Size.X, this.Size.Y, new Texture2D("Content\\BackGroundSprite.png", true, 11, 2), Vector2.Zero);
            planes = new List<Entity>();
            platforms = new List<Entity>();
            Random random = new Random();
            int planeHeight = 28;
            int planeWidth = 60;
            for (int i = 0; i < 5; i++)
            {
                int y = random.Next(0, this.Size.Y - planeHeight - rocket.Height*2);
                while (i != 0 && Math.Abs(planes[i - 1].Position.Y - y) < offsetY)
                    y = random.Next(0, this.Size.Y - planeHeight - rocket.Height*2);

                int x = random.Next(0, this.Size.X - planeWidth);
                while (i != 0 && Math.Abs(planes[i - 1].Position.Y - y) < offsetX)
                    x = random.Next(0, this.Size.X - planeWidth);

                planes.Add(new Entity(planeWidth, planeHeight, new Texture2D("Content\\Plane.png"), new Vector2(x, y), BufferUsageHint.DynamicDraw, 0.03f));
            }
            int platformHeight = 10;
            int platformWidth = 103;
            for (int i = 0; i < 5; i++)
            {
                int y = random.Next(0, this.Size.Y - platformHeight - rocket.Height*2);
                while (i != 0 && Math.Abs(planes[i - 1].Position.Y - y) < offsetY)
                    y = random.Next(0, this.Size.Y - platformHeight - rocket.Height*2);

                int x = random.Next(0, this.Size.X - platformWidth);
                while (i != 0 && Math.Abs(planes[i - 1].Position.Y - y) < offsetX)
                    x = random.Next(0, this.Size.X - platformWidth);

                platforms.Add(new Entity(platformWidth, platformHeight, new Texture2D("Content\\Platform.png"), new Vector2(x, y), BufferUsageHint.DynamicDraw, 0.05f));
            }
        }
        private void InitializeMainMenu()
        {
            gameMainMenuScreen = new Entity(this.Size.X, this.Size.Y, new Texture2D("Content\\MainMenuBackground.png"), Vector2.Zero);
            start = new Button(buttonsWidth, buttonsHeight, new Texture2D("Content\\StartButton.png"), new Vector2(this.Size.X / 2 - buttonsWidth / 2, this.Size.Y / 2 - 100));
            start.OnMouseDown += StartGame;
            exit = new Button(buttonsWidth, buttonsHeight, new Texture2D("Content\\ExitButton.png"), new Vector2(this.Size.X / 2 - buttonsWidth / 2, this.Size.Y / 2 + 100));
            exit.OnMouseDown += ExitGame;
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            InitializeMainMenu();
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
            
            if (gameBackground != null)
                gameBackground.Resize(this.Size.X, this.Size.Y);
            gameMainMenuScreen.Resize(this.Size.X, this.Size.Y);
            start = new Button(buttonsWidth, buttonsHeight, new Texture2D("Content\\StartButton.png"), new Vector2(this.Size.X / 2 - buttonsWidth / 2, this.Size.Y / 2 - 100));
            start.OnMouseDown += StartGame;
            exit = new Button(buttonsWidth, buttonsHeight, new Texture2D("Content\\ExitButton.png"), new Vector2(this.Size.X / 2 - buttonsWidth / 2, this.Size.Y / 2 + 100));
            exit.OnMouseDown += ExitGame;
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            if (lastKeyboardState == Keys.Escape)
            {
                gameNotStarted = !gameNotStarted;
                lastKeyboardState = Keys.Unknown;
            }

            if (!gameNotStarted)
            {
                lag += args.Time;
                if (lag > TIME_PER_FRAME)
                {
                    while (lag > TIME_PER_FRAME)
                    {
                        MoveEntities(lastKeyboardState);
                        gameBackground.Update(lag);
                        rocket.Update(lag);
                        lag -= TIME_PER_FRAME;
                    }
                }
            }
            else if (lastKeyboardState == Keys.Enter)
            {
                gameNotStarted = !gameNotStarted;
                InitializeGame();
                lastKeyboardState = Keys.Unknown;
            }
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            if (!gameNotStarted)
                {
                    GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
                    PlaceObjectsOnMap();
                    GL.LoadIdentity();
                    GL.End();
                }
            else
            {
                gameMainMenuScreen.Draw();
                start.Draw();
                exit.Draw();
            }
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
                    Random random = new Random();
                    planes.Add(new Entity(60, 28, new Texture2D("Content\\Plane.png"), new Vector2(0, random.Next(0, this.Size.Y - 28 - 60)), BufferUsageHint.DynamicDraw, 0.03f));
                    rocket.Move(-rocket.Position + new Vector2(this.Size.X / 2 - 15, this.Size.Y - 60));
                    this.Title = "Очки: " + ++score;
                    break;
                }
            for (int i = 0; i < platforms.Count; i++)
                if (getBounds(rocket).IntersectsWith(getBounds(platforms[i])))
                {
                    gameNotStarted = true;
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
                    int x = random.Next(0, this.Size.X - 103);
                    platforms.Remove(platforms[i]);
                    platforms.Add(new Entity(103, 10, new Texture2D("Content\\Platform.png"), new Vector2(x, 0.0f), BufferUsageHint.DynamicDraw, 0.05f));
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
            gameBackground.Draw(0.015f); // 0.015f
            rocket.Draw(0.035f);
            foreach (Entity plane in planes)
                plane.Draw();
            foreach (Entity platform in platforms)
                platform.Draw();
        }
        private void StartGame()
        {
            gameNotStarted = false;
            InitializeGame();
        }
        private void ExitGame()
        {
            this.Close();
        }
        private Vector2 cursorPosition;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (start.IsPointInFigure(cursorPosition))
                start.OnMouseDown.Invoke();
            else if (exit.IsPointInFigure(cursorPosition))
                exit.OnMouseDown.Invoke();
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            cursorPosition = new Vector2(e.Position.X, e.Position.Y);
        }
    }
}

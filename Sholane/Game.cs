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
        private const int OFFSET = 80;
        private int buttonsWidth = 200;
        private int buttonsHeight = 100;
        private Matrix4 ortho;
        private Entity rocket, gameBackground;
        private List<Entity> planes;
        private List<Entity> platforms;
        private List<Entity> meteors;
        private List<Entity> goodStars;
        private List<Entity> badStars;
        private Keys lastKeyboardState;
        private bool gameNotStarted = true;

        private Entity gameMainMenuScreen;
        private Button start, exit;

        private delegate void GameEvent();
        private event GameEvent MeteorsFall;
        private event GameEvent StarsAppearing;

        private static int score;

        internal Game(GameWindowSettings gSettings, NativeWindowSettings nSettings) : base(gSettings, nSettings)
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.Texture2D);
        }
        private void InitializeGame()
        {
            if (gameBackground != null)
            {
                gameBackground.Dispose();
                rocket.Dispose();
                foreach (Entity plane in planes)
                    plane.Dispose();
                foreach (Entity platform in platforms)
                    platform.Dispose();
            }

            score = 0;
            this.Title = "Очки: " + score;
            rocket = new Entity(33, 60, new Texture("Content\\RocketSprite.png", 2, 1), new Vector2(this.Size.X / 2 - 15, this.Size.Y - 60), BufferUsageHint.DynamicDraw, 0.08f);
            gameBackground = new Entity(this.Size.X, this.Size.Y, new Texture("Content\\BackGroundSprite.png", 11, 2), Vector2.Zero);

            planes = new List<Entity>();
            int planeHeight = 28;
            int planeWidth = 60;
            while (planes.Count < 5)
                planes.Add(new Entity(planeWidth, planeHeight, new Texture("Content\\Plane.png"), genRandCoords(planes, planeHeight, planeWidth), BufferUsageHint.DynamicDraw, 0.03f));


            platforms = new List<Entity>();
            int platformHeight = 10;
            int platformWidth = 103;
            while (platforms.Count < 4)
                platforms.Add(new Entity(platformWidth, platformHeight, new Texture("Content\\Platform.png"), genRandCoords(platforms, platformHeight, platformWidth), BufferUsageHint.DynamicDraw, 0.05f));

            meteors = new List<Entity>();
            MeteorsFall = spawnMeteors;

            goodStars = new List<Entity>();
            badStars = new List<Entity>();
            StarsAppearing = spawnStars;
        }
        private void spawnMeteors()
        {
            int meteorHeight = 52;
            int meteormWidth = 13;
            Random random = new Random();
            while (meteors.Count < random.Next(0, 5 - meteors.Count))
                meteors.Add(new Entity(meteormWidth, meteorHeight, new Texture("Content\\Meteor.png"), genRandCoords(meteors, meteormWidth), BufferUsageHint.DynamicDraw, 0.2f));
        }
        private void spawnStars()
        {
            Random random = new Random();
            int starsSize = random.Next(15, 24);
            if (goodStars.Count <= 3)
            {
                while (goodStars.Count < random.Next(0, 3 - goodStars.Count))
                    goodStars.Add(new Entity(starsSize, starsSize, new Texture("Content\\GoodStar.png"), genRandCoords(goodStars, starsSize, starsSize), BufferUsageHint.DynamicDraw, 0.015f));
            }
            if (badStars.Count == 0)
            {
                badStars.Add(new Entity(starsSize, starsSize, new Texture("Content\\BadStar.png"), genRandCoords(badStars, starsSize, starsSize), BufferUsageHint.DynamicDraw, 0.015f));
            }
        }
        private Vector2 genRandCoords(List<Entity> entities, int height, int width)
        {
            Random random = new Random();
            int x, y;
            do
            {
                y = random.Next(0, this.Size.Y - height - rocket.Height * 2);
                x = random.Next(0, this.Size.X - width);
            }
            while (!allEntitiesFarAway(entities, x, y));
            return new Vector2(x, y);
        }
        private Vector2 genRandCoords(List<Entity> entities, int width)
        {
            Random random = new Random();
            int x;
            do
            {
                x = random.Next(0, this.Size.X - width);
            }
            while (!allEntitiesFarAway(entities, x, 0));
            return new Vector2(x, 0);
        }
        private bool allEntitiesFarAway(List<Entity> entities, int x, int y)
        {
            Vector2 goalPostion = new Vector2(x, y);
            foreach (Entity entity in entities)
                if (Vector2.Distance(goalPostion, entity.Position) < OFFSET)
                    return false;
            return true;
        }
        private void InitializeMainMenu()
        {
            gameMainMenuScreen = new Entity(this.Size.X, this.Size.Y, new Texture("Content\\MainMenuBackground.png"), Vector2.Zero);
            start = new Button(buttonsWidth, buttonsHeight, new Texture("Content\\StartButton.png"), new Vector2(this.Size.X / 2 - buttonsWidth / 2, this.Size.Y / 2 - 100));
            start.OnMouseDown += StartGame;
            exit = new Button(buttonsWidth, buttonsHeight, new Texture("Content\\ExitButton.png"), new Vector2(this.Size.X / 2 - buttonsWidth / 2, this.Size.Y / 2 + 100));
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

            start.Move(-start.Position);
            start.Move(new Vector2(this.Size.X / 2 - buttonsWidth / 2, this.Size.Y / 2 - 100));
            exit.Move(-exit.Position);
            exit.Move(new Vector2(this.Size.X / 2 - buttonsWidth / 2, this.Size.Y / 2 + 100));
        }
        private double lag = 0, timePerFrame = 0.001;
        private double eventsTime = 0;
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            if (gameBackground != null && lastKeyboardState == Keys.Escape)
            {
                gameNotStarted = !gameNotStarted;
                lastKeyboardState = Keys.Unknown;
            }

            if (!gameNotStarted)
            {
                lag += args.Time;
                if (lag >= timePerFrame)
                {
                    eventsTime += lag;
                    Random random = new Random();
                    //if (eventsTime >= random.Next(5, 20))
                    //{
                    //    MeteorsFall.Invoke();
                    //    eventsTime = 0;
                    //}
                    //else if (eventsTime >= random.Next(10, 20))
                    //{
                    //    StarsAppearing.Invoke();
                    //    eventsTime = 0;
                    //}

                    if (eventsTime >= random.Next(5, 10))
                    {
                        StarsAppearing.Invoke();
                        eventsTime = 0;
                    }
                    while (lag >= timePerFrame)
                    {
                        MoveEntities();
                        gameBackground.Update(lag);
                        rocket.Update(lag);
                        lag -= timePerFrame;
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
                GL.Clear(ClearBufferMask.ColorBufferBit);
                PlaceObjectsOnMap();
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
        private void checkCollision()
        {
            for (int i = 0; i < planes.Count; i++)
                if (getBounds(rocket).IntersectsWith(getBounds(planes[i])))
                {
                    rocket.Move(-rocket.Position);
                    rocket.Move(new Vector2(this.Size.X / 2 - 15, this.Size.Y - 60));
                    this.Title = "Очки: " + ++score;

                    planes[i].Move(-planes[i].Position);
                    planes[i].Move(genRandCoords(planes, planes[i].Height, planes[i].Width));
                    break;
                }
            for (int i = 0; i < platforms.Count; i++)
                if (getBounds(rocket).IntersectsWith(getBounds(platforms[i])))
                {
                    gameNotStarted = true;
                    break;
                }
            for (int i = 0; i < meteors.Count; i++)
                if (getBounds(rocket).IntersectsWith(getBounds(meteors[i])))
                {
                    rocket.Move(-rocket.Position);
                    rocket.Move(new Vector2(this.Size.X / 2 - 15, this.Size.Y - 60));
                    this.Title = "Очки: " + --score;

                    meteors[i].Dispose();
                    meteors.RemoveAt(i);
                }
                else
                {
                    Entity platform = platforms.Find(platform => getBounds(platform).IntersectsWith(getBounds(meteors[i])));
                    if (platform != null)
                    {
                        platform.Move(-platform.Position);
                        platform.Move(genRandCoords(platforms, platform.Width));

                        meteors[i].Dispose();
                        meteors.RemoveAt(i);
                    }
                    else
                    {
                        Entity plane = planes.Find(plane => getBounds(plane).IntersectsWith(getBounds(meteors[i])));
                        if (plane != null)
                        {
                            plane.Move(-plane.Position);
                            plane.Move(genRandCoords(planes, plane.Width));

                            meteors[i].Dispose();
                            meteors.RemoveAt(i);
                        }
                    }
                }
            for (int i = 0; i < goodStars.Count; i++)
                if (getBounds(rocket).IntersectsWith(getBounds(goodStars[i])))
                {
                    score += 3;
                    this.Title = "Очки: " + score;

                    goodStars[i].Dispose();
                    goodStars.RemoveAt(i);
                    break;
                }
            for (int i = 0; i < badStars.Count; i++)
                if (getBounds(rocket).IntersectsWith(getBounds(badStars[i])))
                {
                    score -= 5;
                    this.Title = "Очки: " + score;

                    badStars[i].Dispose();
                    badStars.RemoveAt(i);
                    break;
                }
        }
        private bool IsOutsideBoarder(float x, float y)
        {
            if (x < 0 || y < 0 || x > this.Size.X || y > this.Size.Y)
                return true;
            return false;
        }
        private void MoveEntities()
        {
            checkCollision();
            switch (lastKeyboardState)
            {
                case Keys.Up:
                case Keys.W:
                    if (!IsOutsideBoarder(rocket.Position.X, rocket.Position.Y - rocket.Speed))
                        rocket.Move(new Vector2(0.0f, -rocket.Speed));
                    break;

                case Keys.Left:
                case Keys.A:
                    if (!IsOutsideBoarder(rocket.Position.X - rocket.Speed, rocket.Position.Y))
                        rocket.Move(new Vector2(-rocket.Speed, 0.0f));
                    break;

                case Keys.Down:
                case Keys.S:
                    if (!IsOutsideBoarder(rocket.Position.X, rocket.Position.Y + rocket.Speed + rocket.Height))
                        rocket.Move(new Vector2(0.0f, rocket.Speed));
                    break;

                case Keys.Right:
                case Keys.D:
                    if (!IsOutsideBoarder(rocket.Position.X + rocket.Speed + rocket.Width, rocket.Position.Y))
                        rocket.Move(new Vector2(rocket.Speed, 0.0f));
                    break;
            }

            for (int i = 0; i < planes.Count; i++)
                if (!IsOutsideBoarder(planes[i].Position.X + planes[i].Speed, planes[i].Position.Y))
                    planes[i].Move(new Vector2(planes[i].Speed, planes[i].Speed));
                else
                {
                    if (planes[i].Position.X + planes[i].Speed >= this.Size.X)
                        planes[i].Move(new Vector2(-this.Size.X, 0.0f));
                    else
                        planes[i].Move(new Vector2(0.0f, -this.Size.Y));
                }

            for (int i = 0; i < platforms.Count; i++)
                if (!IsOutsideBoarder(platforms[i].Position.X + platforms[i].Speed, platforms[i].Position.Y))
                    platforms[i].Move(new Vector2(0.0f, platforms[i].Speed));
                else
                {
                    platforms[i].Move(-platforms[i].Position);
                    platforms[i].Move(genRandCoords(platforms, platforms[i].Width));
                }

            for (int i = 0; i < meteors.Count; i++)
                if (!IsOutsideBoarder(meteors[i].Position.X + meteors[i].Speed, meteors[i].Position.Y))
                    meteors[i].Move(new Vector2(0.0f, meteors[i].Speed));
                else
                {
                    meteors[i].Dispose();
                    meteors.RemoveAt(i);
                }

            for (int i = 0; i < goodStars.Count; i++)
                if (!IsOutsideBoarder(goodStars[i].Position.X + goodStars[i].Speed, goodStars[i].Position.Y))
                    goodStars[i].Move(new Vector2(0.0f, goodStars[i].Speed));
                else
                {
                    goodStars[i].Dispose();
                    goodStars.RemoveAt(i);
                }
            for (int i = 0; i < badStars.Count; i++)
                if (!IsOutsideBoarder(badStars[i].Position.X + badStars[i].Speed, badStars[i].Position.Y))
                    badStars[i].Move(new Vector2(0.0f, badStars[i].Speed));
                else
                {
                    badStars[i].Dispose();
                    badStars.RemoveAt(i);
                }
        }
        private void DrawList(List<Entity> entities)
        {
            foreach (Entity entity in entities)
                entity.Draw();
        }
        private void PlaceObjectsOnMap()
        {
            gameBackground.Draw(0.015f);
            rocket.Draw(0.0045f);
            DrawList(goodStars);
            DrawList(badStars);
            DrawList(planes);
            DrawList(platforms);
            DrawList(meteors);
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
            if (gameNotStarted)
                if (start.IsPointInFigure(cursorPosition))
                    start.OnMouseDownEvent();
                else if (exit.IsPointInFigure(cursorPosition))
                    exit.OnMouseDownEvent();
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            cursorPosition = new Vector2(e.Position.X, e.Position.Y);
        }
    }
}

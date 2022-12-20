using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sholane.TextureProcess;
using System.Drawing;

namespace Sholane
{
    internal class Game : GameWindow
    {
        Matrix4 ortho;
        Entity entity, background;
        internal Game(GameWindowSettings gSettings, NativeWindowSettings nSettings) : base(gSettings, nSettings)
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.Texture2D);

            entity = new Entity(33, 60, "Content\\Rocket.png", Vector2.Zero, BufferUsageHint.DynamicDraw);
            background = new Entity(nSettings.Size.X, nSettings.Size.Y, "Content\\Sky.jpg", Vector2.Zero);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
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
        double lag = 0, TIME_PER_FRAME = 0.001;
        Keys lastKeyboardState;
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            lag += args.Time;
            if (lag > TIME_PER_FRAME)
            {
                while (lag > TIME_PER_FRAME)
                {
                    KeyPressedMoveTheEntity(lastKeyboardState);
                    lag -= TIME_PER_FRAME;
                }
            }
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            background.Draw();
            entity.Draw();
            GL.LoadIdentity();
            GL.End();

            SwapBuffers();
        }
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            lastKeyboardState = e.Key;
        }
        private void KeyPressedMoveTheEntity(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                case Keys.W:
                    entity.Move(new Vector2(-0.0f, -0.1f));
                    break;

                case Keys.Left:
                case Keys.A:
                    entity.Move(new Vector2(-0.1f, 0.0f));
                    break;

                case Keys.Down:
                case Keys.S:
                    entity.Move(new Vector2(0.0f, 0.1f));
                    break;

                case Keys.Right:
                case Keys.D:
                    entity.Move(new Vector2(0.1f, 0.0f));
                    break;

                case Keys.Escape:
                    Close();
                    break;
            }
        }
    }
}

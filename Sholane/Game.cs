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
        Entity entity;
        internal Game(GameWindowSettings gSettings, NativeWindowSettings nSettings) : base(gSettings, nSettings)
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.Texture2D);

            entity = new Entity(40, 50, "Content\\StartButton.png", Vector2.Zero);
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

            GL.Viewport(0, 0, 40, 50);

            ortho = Matrix4.CreateOrthographicOffCenter(0, 40, 50, 0, -1, 1);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref ortho);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            entity.Resize(40, 50);
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);


            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            if (KeyboardState.IsKeyDown(Keys.A))
                entity.Move(new Vector2(-0.1f, 0.0f));
            if (KeyboardState.IsKeyDown(Keys.S))
                entity.Move(new Vector2(0.0f, 0.1f));
            if (KeyboardState.IsKeyDown(Keys.W))
                entity.Move(new Vector2(-0.0f, -0.1f));
            if (KeyboardState.IsKeyDown(Keys.D))
                entity.Move(new Vector2(0.1f, 0.0f));
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.Black);

            entity.Draw();
            GL.LoadIdentity();
            GL.End();

            SwapBuffers();
        }
    }
}

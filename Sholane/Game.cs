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
        Background background;
        internal Game(GameWindowSettings gSettings, NativeWindowSettings nSettings) : base(gSettings, nSettings)
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.Texture2D);

            background = new Background(nSettings.Size.X, nSettings.Size.Y, "Content\\StartButton.png");
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
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.Black);

            background.Draw();
            GL.LoadIdentity();
            GL.End();

            SwapBuffers();
        }
    }
}

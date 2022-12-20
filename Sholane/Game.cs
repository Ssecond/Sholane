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
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.PointSmooth);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.Enable(EnableCap.Multisample);
            GL.BlendFunc(BlendingFactor.Src1Alpha, BlendingFactor.OneMinusSrcAlpha);
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.CullFace(CullFaceMode.Back);

            background = new Background(nSettings.Size.X, nSettings.Size.Y, "Content\\StartButton.png");
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // texture = new Texture2D(@"Content/StartButton.png");
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
            //GL.BindTexture(TextureTarget.Texture2D, texture.ID);
            //GL.Begin(PrimitiveType.Quads);

            //#region poly
            ////GL.Color3(Color.Red);
            //GL.TexCoord2(1.0f, 0.0f);
            //GL.Vertex2(0.0f, 0.0f);

            ////GL.Color3(Color.Blue);
            //GL.TexCoord2(0.0f, 0.0f);
            //GL.Vertex2(1.0f, 0.0f);

            ////GL.Color3(Color.Orange);
            //GL.TexCoord2(1.0f, 1.0f);
            //GL.Vertex2(1.0f, -1.0f);

            //// GL.Color3(Color.Green);
            //GL.TexCoord2(0.0f, 1.0f);
            //GL.Vertex2(0.0f, -1.0f);
            //#endregion
            GL.End();

            SwapBuffers();
        }
    }
}

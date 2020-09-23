using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTKTransparency;
using Boolean = System.Boolean;

namespace OpenTKTesting
{
    class Game : GameWindow
    {
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);


        int VertexBufferObject;
        int VertexArrayObject;
        private Shader shader;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            IntPtr initialStyle = WindowsInterop.GetWindowLongPtr(new HandleRef(this,this.WindowInfo.Handle), -20);

            const int WS_EX_LAYERED = 0x00080000;
            const int WS_EX_TRANSPARENT = 0x00000020;
            const int WS_EX_TOPMOST = 0x00000008;

            if (WindowsInterop.SetWindowLongPtr(new HandleRef(this,this.WindowInfo.Handle), -20, new IntPtr(initialStyle.ToInt32() | WS_EX_TRANSPARENT | WS_EX_LAYERED)) == IntPtr.Zero) { Console.WriteLine("failure"); }
            if(WindowsInterop.SetLayeredWindowAttributes(this.WindowInfo.Handle, 0x000000FF, 255, 0x00000001 | 0x00000002) == false) { Console.WriteLine("setting failure"); }

            WindowsInterop.SetWindowPos(this.WindowInfo.Handle,HWND_TOPMOST,this.X,this.Y,this.Width,this.Height,SWP.NOSIZE);
            this.WindowBorder = WindowBorder.Hidden;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles,0,3);

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (e.Mouse.IsButtonDown(MouseButton.Left))
            {

            }

            base.OnMouseMove(e);
        }

        protected override void OnLoad(EventArgs e)
        { 
            GL.ClearColor(1.0f,0.0f,0.0f,1.0f);
            shader = new Shader("shader.vert", "shader.frag");

            float[] vertices = {
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                0.5f, -0.5f, 0.0f, //Bottom-right vertex
                0.0f,  0.5f, 0.0f  //Top vertex
            };

            VertexBufferObject = GL.GenBuffer();
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer,VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer,vertices.Length * sizeof(float),vertices,BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            shader.Use();

            Context.SwapBuffers();
            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer,0);
            GL.DeleteBuffer(VertexBufferObject);
            shader.Dispose();

            base.OnUnload(e);
        }
    }
}

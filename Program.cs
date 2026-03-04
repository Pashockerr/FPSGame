using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using System.Drawing;
using StbImageSharp;

namespace FPSGame
{
    public class Game
    {
        private static IWindow? _window;
        private static GL? _gl;
        private static uint _vao;
        private static uint _vbo;
        private static uint _ebo;
        private static uint _program;
        private static float[] _vertices =
        {
            1.0f,  1.0f, 0.0f,   1.0f, 0.0f,
            1.0f, -1.0f, 0.0f,   1.0f, 1.0f,
            -1.0f, -1.0f, 0.0f,   0.0f, 1.0f,
            -1.0f,  1.0f, 0.0f,   0.0f, 0.0f
        };
        private static uint[] _indices =
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };
        private static byte[] _textureData = new byte[1053*874*4];
        private static uint _texture;
        public static void Main(string[] args)
        {
            WindowOptions options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "Game"
            };
            _window = Window.Create(options);
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Run();
        }

        private static unsafe void OnLoad()
        {
            IInputContext inputContext = _window!.CreateInput();
            for(int i = 0; i < inputContext.Keyboards.Count; ++i)
            {
                inputContext.Keyboards[i].KeyDown += KeyDown;
            }

            _window!.FramebufferResize += (size) =>
            {
                _gl!.Viewport(size);
            };

            _gl = _window.CreateOpenGL();
            _gl.ClearColor(Color.Black);
            _vao = _gl.GenVertexArray();
            _gl.BindVertexArray(_vao);
            _vbo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            fixed (float* buf = _vertices)
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (_vertices.Length * sizeof(float)), buf, BufferUsageARB.DynamicDraw);
            _ebo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
            fixed (uint* buf = _indices)
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (_indices.Length * sizeof(uint)), buf, BufferUsageARB.DynamicDraw);

            string vertSource = File.ReadAllText("./Shaders/main.vert");
            string fragSource = File.ReadAllText("./Shaders/main.frag");

            // Vertex compile
            uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
            _gl.ShaderSource(vertexShader, vertSource);
            _gl.CompileShader(vertexShader);

            _gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int) GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + _gl.GetShaderInfoLog(vertexShader));

            
            //Fragment compile
            uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(fragmentShader, fragSource);

            _gl.CompileShader(fragmentShader);

            _gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int) GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + _gl.GetShaderInfoLog(fragmentShader));
            
            _program = _gl.CreateProgram();
            _gl.AttachShader(_program, vertexShader);
            _gl.AttachShader(_program, fragmentShader);

            _gl.LinkProgram(_program);

            _gl.GetProgram(_program, ProgramPropertyARB.LinkStatus, out int lStatus);
            if (lStatus != (int) GLEnum.True)
                throw new Exception("Program failed to link: " + _gl.GetProgramInfoLog(_program));
            _gl.DetachShader(_program, vertexShader);
            _gl.DetachShader(_program, fragmentShader);
            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);

            const uint positionLoc = 0;
            _gl.EnableVertexAttribArray(positionLoc);
            _gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
            const uint texCoordLoc = 1;
            _gl.EnableVertexAttribArray(texCoordLoc);
            _gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float)));
            
            _gl.BindVertexArray(0);
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

            _texture = _gl.GenTexture();
            _gl.ActiveTexture(TextureUnit.Texture0);
            _gl.BindTexture(TextureTarget.Texture2D, _texture);

            ImageResult result = ImageResult.FromMemory(File.ReadAllBytes("./Textures/test.jpg"), ColorComponents.RedGreenBlueAlpha);
            result.Data.CopyTo(_textureData, 0);
            fixed (byte* ptr = _textureData)
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, 1053,
                    874, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
            _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
            _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
            _gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        private static void OnUpdate(double deltaTime)
        {
        }

        private static unsafe void OnRender(double deltaTime)
        {
            _gl!.Clear(ClearBufferMask.ColorBufferBit);
            _gl.UseProgram(_program);
            _gl.ActiveTexture(TextureUnit.Texture0);
            _gl.BindTexture(TextureTarget.Texture2D, _texture);
            fixed (byte* ptr = _textureData)
                _gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, 1053, 874, 
                    PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

            int location = _gl.GetUniformLocation(_program, "uTexture");
            _gl.Uniform1(location, 0);
            _gl.BindVertexArray(_vao);
            _gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*) 0);
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            if(key == Key.Escape)
                _window!.Close();
        }
    }
}
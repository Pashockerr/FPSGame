using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;

public static class Engine
{
    private static bool _is_init = false;
    private static Map? _current_map;
    private static Renderer? _renderer;
    private static Configuration? _config;
    private static Vector2D<double> _pos = new Vector2D<double>(5.0, 5.0);
    private static double _angle = 0.0;
    private static Vector2D<double> _userInput = new Vector2D<double>(0, 0);
    private static IsKeyPressed? _keyPressed;
    private static int fps_timer;

    public delegate bool IsKeyPressed(Key key);

    public static void Init(Configuration config, IsKeyPressed keyPressed)
    {
        _config = config;
        _current_map = new Map("./Maps/default_map.json");
        _renderer = new Renderer(config);
        _is_init = true;
        _keyPressed = keyPressed;
    }

    public static void Tick(double deltaTime)
    {
        if(!_is_init) throw new Exception("Didn't call the Init() method!");
        
        var inp = new Vector2D<double>();
        if(_keyPressed(Key.W))
        {
            inp.Y += 1.0;
        }
        if(_keyPressed(Key.A))
        {
            inp.X -= 1.0;
        }
        if(_keyPressed(Key.S))
        {
            inp.Y -= 1.0;
        }
        if(_keyPressed(Key.D))
        {
            inp.X += 1.0;
        }
        _userInput = inp;

        if(_userInput.Y != 0)
        {
            var direction = new Vector2D<double>(Math.Cos(_angle), Math.Sin(_angle));
            _pos += direction * deltaTime * _userInput.Y;
        }
        if(_userInput.X != 0)
        {
            _angle += _userInput.X * deltaTime;
        }
        fps_timer++;
        if(fps_timer >= 100)
        {
            fps_timer = 0;
            Console.WriteLine($"{1/deltaTime} tps");
        }
    }

    public static byte[] Render()
    {
        byte[] result = new byte[_config.TextureResolution.X * _config.TextureResolution.Y * 4];
        const int THREADS_NUMBER = 32;
        Parallel.For(0, THREADS_NUMBER, (i) =>
        {
            byte[] part = _renderer!.RenderViewportTexture(_current_map!, _pos, _angle, i, THREADS_NUMBER);
            int x_shift = (_config.TextureResolution.X / THREADS_NUMBER) * i;
            for(int y = 0; y < _config.TextureResolution.Y; ++y)
            {
                for(int x = 0; x < _config.TextureResolution.X / THREADS_NUMBER; ++x)
                {
                    result[(y * _config.TextureResolution.X + x + x_shift) * 4] = part[(y * _config.TextureResolution.X / THREADS_NUMBER + x) * 4];
                    result[(y * _config.TextureResolution.X + x + x_shift) * 4 + 1] = part[(y * _config.TextureResolution.X / THREADS_NUMBER + x) * 4 + 1];
                    result[(y * _config.TextureResolution.X + x + x_shift) * 4 + 2] = part[(y * _config.TextureResolution.X / THREADS_NUMBER + x) * 4 + 2];
                    result[(y * _config.TextureResolution.X + x + x_shift) * 4 + 3] = part[(y * _config.TextureResolution.X / THREADS_NUMBER + x) * 4 + 3];
                }
            }
        });

        return result;  // TODO: fix texture stitching
    }

    public static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        
    }
}
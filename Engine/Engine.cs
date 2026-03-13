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
        Console.WriteLine($"{1/deltaTime} tps");
    }

    public static byte[] Render()
    {
        byte[] result = new byte[_config.TextureResolution.X * _config.TextureResolution.Y * 4];
        byte[] parts = new byte[_config.TextureResolution.X * _config.TextureResolution.Y * 4];
        byte[] part0 = _renderer!.RenderViewportTexture(_current_map!, _pos, _angle, 1, 2);
        byte[] part1 = _renderer!.RenderViewportTexture(_current_map!, _pos, _angle, 2, 2);
        Console.WriteLine(part0.Length);
        Console.WriteLine(part1.Length);
        part0.CopyTo(parts, 0);
        part1.CopyTo(parts, parts.Length / 2 - 1);

        int partSize = parts.Length / 2;
        int mainX = 0;
        int mainY = 0;

        for(int p = 0; p < 2; ++p)
        {
            for(int x = 0; x < _config.TextureResolution.X / 2; ++x)
            {
                for(int y = 0; y < _config.TextureResolution.Y / 2; ++y)
                {
                    result[(mainY * _config.TextureResolution.X + mainX) * 4] = parts[p * partSize + y * _config.TextureResolution.X / 2 + x];
                    result[(mainY * _config.TextureResolution.X + mainX) * 4 + 1] = parts[p * partSize + y * _config.TextureResolution.X / 2 + x + 1];
                    result[(mainY * _config.TextureResolution.X + mainX) * 4 + 2] = parts[p * partSize + y * _config.TextureResolution.X / 2 + x + 2];
                    result[(mainY * _config.TextureResolution.X + mainX) * 4 + 3] = parts[p * partSize + y * _config.TextureResolution.X / 2 + x + 3];
                    ++mainY;
                }
                ++mainX;
            }
        }

        return result;  // TODO: fix texture stitching
    }

    public static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        
    }
}
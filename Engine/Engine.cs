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
        return _renderer!.RenderViewportTexture(_current_map!, _pos, _angle);
    }

    public static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        
    }
}
using ILGPU;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;

public class Engine
{
    private Map _current_map;
    private Renderer _renderer;
    private Configuration _config;
    private Vector2D<double> _pos = new Vector2D<double>(5.0, 5.0);
    private double _angle = 0.0;
    private Vector2D<double> _userInput = new Vector2D<double>(0, 0);
    private IsKeyPressed _keyPressed;
    private int fps_timer;
    private int _texture_width;
    private int _texture_height;
    private int _texture_part_width;
    private Context _context;

    public delegate bool IsKeyPressed(Key key);

    public Engine(Configuration config, IsKeyPressed keyPressed)
    {
        _config = config;
        _current_map = new Map("./Maps/default_map.json");
        _renderer = new Renderer(config);
        _keyPressed = keyPressed;
        _texture_height = _config.TextureResolution.Y;
        _texture_width = _config.TextureResolution.X;
        _texture_part_width = _texture_width / _config.ThreadCount;
        _context = Context.Create(builder => builder.AllAccelerators());
        foreach(var accelerator in _context.Devices)
        {
            Console.WriteLine(accelerator);
        }
    }

    public void Tick(double deltaTime)
    {
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

    public byte[] Render()
    {
        byte[] result = new byte[_config.TextureResolution.X * _config.TextureResolution.Y * 4];
        Parallel.For(0, _config.ThreadCount, (i) =>
        {
            byte[] part = _renderer!.RenderViewportTexture(_current_map!, _pos, _angle, i, _config.ThreadCount);
            int x_shift = (_config.TextureResolution.X / _config.ThreadCount) * i;
            for(int y = 0; y < _texture_height; ++y)
            {
                for(int x = 0; x < _texture_part_width; ++x)
                {
                    result[(y * _texture_width + x + x_shift) * 4] = part[(y * _texture_width / _config.ThreadCount + x) * 4];
                    result[(y * _texture_width + x + x_shift) * 4 + 1] = part[(y * _texture_width / _config.ThreadCount + x) * 4 + 1];
                    result[(y * _texture_width + x + x_shift) * 4 + 2] = part[(y * _texture_width / _config.ThreadCount + x) * 4 + 2];
                    result[(y * _texture_width + x + x_shift) * 4 + 3] = part[(y * _texture_width / _config.ThreadCount + x) * 4 + 3];
                }
            }
        });

        return result;
    }

    public static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        
    }
}
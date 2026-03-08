using System.Numerics;
using Silk.NET.Maths;

public static class Engine
{
    private static bool _is_init = false;
    private static Map? _current_map;
    private static Raycaster? _raycaster;
    private static int _texture_width;
    private static int _texture_height;
    public static void Init(int textureWidth, int textureHeight, double viewportHeight, double viewportDistance)
    {
        _texture_width = textureWidth;
        _texture_height = textureHeight;

        _current_map = new Map("./Maps/default_map.json");
        _raycaster = new Raycaster(500, 5.0, 3.14/2, _texture_width, viewportHeight, viewportDistance);

        _is_init = true;
    }
    public static void Tick()
    {
        if(!_is_init) throw new Exception("Didn't call the Init() method!");

    }


    public static byte[] Render(Vector2D<double> position, double angle)
    {
        int[] viewportTexture = new int[_texture_width * _texture_height]; 
        var hitResults = _raycaster!.CastSector(_current_map!, position, angle);
        for(int tX = 0; tX < _texture_width; ++tX)
        {
            if(hitResults[tX].Tile == Tile.EMPTY)   // Didn't hit tile, so 0-length column
            {
                continue;
            }
            double distance = (hitResults[tX].Position - position).Length;
            // Console.WriteLine(distance);
            double fisheyeCorrection = Math.Cos(_raycaster.Fov / 2 - (_raycaster.Fov / _texture_width) * tX);
            Console.WriteLine(fisheyeCorrection);
            double viewportProjHeight = 1 * (_raycaster.ViewportDistance / (distance + _raycaster.ViewportDistance)) / fisheyeCorrection; // / ((distance + _raycaster.ViewportDistance) / _raycaster.ViewportDistance);     // Tile height is 1 unit;
            int columnH = (int)(_texture_height * viewportProjHeight);
            //Console.WriteLine(viewportProjHeight);

            int sY = (_texture_height - columnH) / 2;
            for(int dY = 0; dY < columnH; ++dY)
            {
                byte gamma = (byte)(viewportProjHeight * 255);
                viewportTexture[(sY + dY) * _texture_width + tX] = ColorToInt(gamma, gamma, gamma, 255);
            }
        }

        byte[] glTexture = new byte[_texture_height * _texture_width * 4];
        // Console.WriteLine(glTexture.Length);
        for(int i = 0; i < viewportTexture.Length; ++i)
        {
            var colors = IntToColor(viewportTexture[i]);
            glTexture[i * 4] = colors[0];
            glTexture[i * 4 + 1] = colors[1];
            glTexture[i * 4 + 2] = colors[2];
            glTexture[i * 4 + 3] = colors[3];
        }

        return glTexture;
    }

    public static int ColorToInt(byte r, byte g, byte b, byte a)
    {
        int res = 0;
        res += r << 24;
        res += g << 16;
        res += b << 8;
        res += a;

        return res;
    }

    // RGBA: int 0x FF FF FF FF
    //              R  G  B  A   
    public static byte[] IntToColor(int color)
    {
        byte[] result = new byte[4];
        result[0] = (byte)(color >> 24);
        result[1] = (byte)(color >> 16);
        result[2] = (byte)(color >> 8);
        result[3] = (byte)color;

        return result;
    }
}
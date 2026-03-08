using System.Runtime.CompilerServices;
using Silk.NET.Maths;

public class Renderer(Configuration configuration)
{
    private const double FOCAL_LENGTH = .1;
    private const double VIEWPORT_HEIGHT = .25;
    private const bool FISHEYE_CORRECTION = true;
    private int _textureWidth = configuration.TextureResolution.X;
    private int _textureHeight = configuration.TextureResolution.Y;
    private Raycaster _raycaster = new Raycaster(configuration.RaySteps, configuration.ViewDistance, configuration.Fov, configuration.RayCount, VIEWPORT_HEIGHT, FOCAL_LENGTH, FISHEYE_CORRECTION);
    public byte[] RenderViewportTexture(Map map, Vector2D<double> position, double angle)
    {
        byte[] viewportTexture = new byte[_textureWidth * _textureHeight * 4]; 
        var hitResults = _raycaster!.CastSector(map!, position, angle);
        for(int tX = 0; tX < _textureWidth; ++tX)
        {
            int hRX = hitResults.Length * tX / _textureWidth;
            if(hitResults[hRX].Tile == Tile.EMPTY)   // Didn't hit tile, so 0-length column
            {
                continue;
            }
            double distance = (hitResults[hRX].Position - position).Length;
            double fisheyeCorrection = Math.Cos(_raycaster.Fov / 2 - (_raycaster.Fov / _textureWidth) * tX);
            double viewportProjHeight = (_raycaster.ViewportDistance / (distance + _raycaster.ViewportDistance) / fisheyeCorrection); // / ((distance + raycaster.ViewportDistance) / raycaster.ViewportDistance);     // Tile height is 1 unit;
            int columnH = (int)(_textureHeight * viewportProjHeight / VIEWPORT_HEIGHT);
            columnH = columnH > _textureHeight ? _textureHeight : columnH;

            var clampedColumnH = Math.Clamp(columnH, 0, _textureHeight);
            int sY = (_textureHeight - clampedColumnH) / 2;
            for(int dY = 0; dY < clampedColumnH; ++dY)
            {
                byte gamma = (byte)(viewportProjHeight * 255);
                viewportTexture[((sY + dY) * _textureWidth + tX) * 4] = gamma;
                viewportTexture[((sY + dY) * _textureWidth + tX) * 4 + 1] = gamma;
                viewportTexture[((sY + dY) * _textureWidth + tX) * 4 + 2] = gamma;
                viewportTexture[((sY + dY) * _textureWidth + tX) * 4 + 3] = 255;
            }
        }
        return viewportTexture;
    }
}
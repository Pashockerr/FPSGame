using System.Runtime.CompilerServices;
using Silk.NET.Core;
using Silk.NET.Maths;

public class Renderer(Configuration configuration)
{
    private const double FOCAL_LENGTH = .1;
    private const double VIEWPORT_HEIGHT = .25;
    private const bool FISHEYE_CORRECTION = true;
    private const double TEXTURE_EPSILON = .01;    // TODO: maybe automatic calculation based on step length
    private int _textureWidth = configuration.TextureResolution.X;
    private int _textureHeight = configuration.TextureResolution.Y;
    private Raycaster _raycaster = new Raycaster(configuration.RaySteps, configuration.ViewDistance, configuration.Fov, configuration.RayCount, VIEWPORT_HEIGHT, FOCAL_LENGTH, FISHEYE_CORRECTION);
    public byte[] RenderViewportTexture(Map map, Vector2D<double> position, double angle, int thread, int totalThreads)
    {
        byte[] viewportTexture = new byte[_textureWidth * _textureHeight * 4 / totalThreads]; 
        var hitResults = _raycaster!.CastSectorPart(map!, position, angle, thread, totalThreads);
        for(int tX = 0; tX < _textureWidth / totalThreads; ++tX)
        {
            int hRX = hitResults.Length * tX / (_textureWidth / totalThreads);
            if(hitResults[hRX].Tile == Tile.EMPTY)   // Didn't hit tile, so 0-length column
            {
                continue;
            }
            double distance = (hitResults[hRX].Position - position).Length;

            var hitPos = hitResults[hRX].Position;
            var tile = hitResults[hRX].Tile;
            // Get texture coordinates
            double xFrac = hitPos.X - Math.Truncate(hitPos.X);
            double yFrac = hitPos.Y - Math.Truncate(hitPos.Y);
            TileSide side = TileSide.LEFT;

            double u = 0;

            double lFrac = Math.Abs(xFrac);
            double rFrac = Math.Abs(1 - xFrac);
            double bFrac = Math.Abs(yFrac);
            double tFrac = Math.Abs(1 - yFrac);

            // Left side. U = yFrac
            if (lFrac < TEXTURE_EPSILON && lFrac - tFrac < 0 && lFrac - bFrac < 0)
            {
                u = yFrac;
            }
            // Right side. U = 1 - yFrac
            else if (rFrac < TEXTURE_EPSILON && rFrac - tFrac < 0 && rFrac - bFrac < 0)
            {
                u = 1 - yFrac; // Mirror
                side = TileSide.RIGHT;
            }
            // Bottom side. U = xFrac
            else if (bFrac < TEXTURE_EPSILON && bFrac - lFrac < 0 && bFrac - rFrac < 0)
            {
                u = 1 - xFrac;
                side = TileSide.BOTTOM;
            }
            // Top side. U = 1 - xFrac
            else if(tFrac < TEXTURE_EPSILON && tFrac - lFrac < 0 && tFrac - rFrac < 0)
            {
                u = xFrac;
                side = TileSide.TOP;
            }

            double fisheyeCorrection = Math.Cos(_raycaster.Fov / 2 - (_raycaster.Fov / _textureWidth) * tX);
            double viewportProjHeight = (_raycaster.ViewportDistance / (distance + _raycaster.ViewportDistance) / fisheyeCorrection); // / ((distance + raycaster.ViewportDistance) / raycaster.ViewportDistance);     // Tile height is 1 unit;
            int columnH = (int)(_textureHeight * viewportProjHeight / VIEWPORT_HEIGHT);
            columnH = columnH > _textureHeight ? _textureHeight : columnH;

            var clampedColumnH = Math.Clamp(columnH, 0, _textureHeight);
            int sY = (_textureHeight - clampedColumnH) / 2;
            for(int dY = 0; dY < clampedColumnH; ++dY)
            {
                Texture tileTexture = tile.Texture;
                int uI = 0;
                int vI = 0;
                // Console.WriteLine(dY);
                // Console.WriteLine(clampedColumnH);
                switch (side)
                {
                    case TileSide.LEFT:
                        uI = (int)(tileTexture.Width * (u / 2.0));
                        vI = (int)(tileTexture.Height * ((double)dY / columnH / 2.0));   
                        break;
                    case TileSide.RIGHT:
                        uI = (int)(tileTexture.Width * (u / 2.0 + .5));
                        vI = (int)(tileTexture.Height * ((double)dY / columnH / 2.0));
                        break;
                    case TileSide.BOTTOM:
                        uI = (int)(tileTexture.Width * (u / 2.0));
                        vI = (int)(tileTexture.Height * ((double)dY / columnH / 2.0 + .5));   
                        break;
                    case TileSide.TOP:
                        uI = (int)(tileTexture.Width * (u / 2.0 + .5));
                        vI = (int)(tileTexture.Height * ((double)dY / columnH / 2.0 + .5));   
                        break;
                }

                byte r = tileTexture.Bytes[(vI * tileTexture.Width + uI) * 4];
                byte g = tileTexture.Bytes[(vI * tileTexture.Width + uI) * 4 + 1];
                byte b = tileTexture.Bytes[(vI * tileTexture.Width + uI) * 4 + 2];
                byte a = tileTexture.Bytes[(vI * tileTexture.Width + uI) * 4 + 3];
                byte gamma = (byte)(viewportProjHeight * 255);
                viewportTexture[((sY + dY) * _textureWidth / totalThreads + tX) * 4] = r;
                viewportTexture[((sY + dY) * _textureWidth / totalThreads + tX) * 4 + 1] = g;
                viewportTexture[((sY + dY) * _textureWidth / totalThreads + tX) * 4 + 2] = b;
                viewportTexture[((sY + dY) * _textureWidth / totalThreads + tX) * 4 + 3] = a;
            }
        }
        return viewportTexture;
    }
}
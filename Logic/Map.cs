using System.Numerics;
using Silk.NET.Maths;

public class Map(uint width, uint height, Tile[] tiles)
{
    public readonly uint Width = width;
    public readonly uint Height = height;
    private Tile[] _tiles = tiles;

    public Tile GetTile(int x, int y)
    {
        return _tiles[y * Height + x];
    }

    public Tile GetTile(Vector2D<double> position)
    {
        int x = (int)Math.Truncate(position.X);
        int y = (int)Math.Truncate(position.Y);
        return _tiles[y * Height + x];
    }
}
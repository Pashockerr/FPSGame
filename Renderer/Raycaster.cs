using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using Silk.NET.Maths;

public class Raycaster(int steps, double rayLength, double fov, int rayCount)
{
    private int _steps = steps;
    private double _rayLength = rayLength;
    private double _fov = fov;
    private int _rayCount = rayCount;

    public MapHitResult CastRay(Map map, Vector2D<double> position, double angle)
    {
        double stepLength = _rayLength / _steps;
        Vector2D<double> direction = new()
        {
            X = Math.Cos(angle),
            Y = Math.Sin(angle)
        };

        var hitResult = new MapHitResult
        {
            Position = position + direction*_rayLength,
            Tile = Tile.EMPTY
        };
        for(int step = 0; step < _steps; ++step)
        {
            var hitTest = position + direction*stepLength*step;
            var tile = map.GetTile(hitTest);
            if(tile != Tile.EMPTY)
            {
                return hitResult with
                {
                    Position = hitTest,
                    Tile = tile
                };
            }
        }
        return hitResult;
    }

    public MapHitResult[] CastSector(Map map, Vector2D<double> position, double angle)
    {
        var startAngle = angle - _fov / 2;
        var dA = _fov / _rayCount;
        MapHitResult[] result = new MapHitResult[_rayCount];
        int i = 0;
        for(double deltaAngle = 0; deltaAngle < _fov; deltaAngle += dA)
        {
            result[i] = CastRay(map, position, startAngle + deltaAngle);
            ++i;
        }
        return result;
    }
}
using Silk.NET.Maths;

public class Raycaster(int steps, double rayLength, double fov, int rayCount, double viewportHeight, double focalLength, bool fisheyeCorrection)
{
    private int _steps = steps;
    private double _rayLength = rayLength;
    private double _fov = fov;
    private int _rayCount = rayCount;
    private double _viewportHeight = viewportHeight;
    private double _focalLength = focalLength;
    private bool _fisheyeCorrection = fisheyeCorrection;

    public double ViewportHeight { get { return _viewportHeight;} }
    public double ViewportDistance { get { return _focalLength;} }
    public double Fov { get { return _fov; } }
    public double RayCount { get { return _rayCount; } }

    public MapHitResult CastRay(Map map, Vector2D<double> position, double angle, double correctionCoeffitient)
    {
        double rL = _rayLength / correctionCoeffitient;
        double stepLength = rL / _steps;
        Vector2D<double> direction = new()
        {
            X = Math.Cos(angle),
            Y = Math.Sin(angle)
        };

        var hitResult = new MapHitResult
        {
            Position = position + direction*rL,
            Tile = Tile.EMPTY
        };
        for(int step = 0; step < _steps; ++step)
        {
            var hitTest = position + direction*stepLength*step;
            var tile = map.GetTile(hitTest);
            if(tile != Tile.EMPTY)
            {
                return new MapHitResult
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
        double correctionCoeffitient = 1.0;
        for(double deltaAngle = dA; deltaAngle < _fov; deltaAngle += dA)
        {
            if (_fisheyeCorrection)
            {
                correctionCoeffitient = Math.Cos(dA - _fov / 2);
            }
            result[i++] = CastRay(map, position, startAngle + deltaAngle, correctionCoeffitient);
        }
        return result;
    }
}
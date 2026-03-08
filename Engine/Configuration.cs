using System.Text.Json;
using Silk.NET.Maths;

public class Configuration
{
    private Vector2D<int> _textureResolution;
    private double _fov;
    private double _viewDistance;
    private int _rayCount;
    private int _raySteps;

    public Vector2D<int> TextureResolution
    {
        get { return _textureResolution; } 
        set
        {
            if(value.X < 0 || value.Y < 0)
                throw new InvalidDataException("Texture dimensions shouldn't be negative!");
            _textureResolution = value;
        }
    }
    public double Fov
    {
        get { return _fov; }
        set
        {
            if(value < 0)
                throw new InvalidDataException("Fov shouldn't be negative!");
            _fov = value / 180.0 * Math.PI;
        }
    }
    public double ViewDistance
    {
        get { return _viewDistance; }
        set
        {
            if(value < 0)
                throw new InvalidDataException("ViewDistance shouldn't be negative!");
            _viewDistance = value;
        }
    }
    public int RayCount
    {
        get { return _rayCount; }
        set
        {
            if(value < 0)
                throw new InvalidDataException("RayCount shouldn't be negative!");
            _rayCount = value;
        }
    }
    public int RaySteps
    {
        get { return _raySteps; }
        set
        {
            if(value < 0)
                throw new InvalidDataException("RaySteps shouldn't be negative!");
            _raySteps = value;
        }
    }

    public Configuration(string filePath)
    {
        string content = File.ReadAllText(filePath);
        var doc = JsonDocument.Parse(content);
        ConfigurationDto cD = doc.Deserialize<ConfigurationDto>() ?? throw new Exception("Failed to load configuration from file");

        TextureResolution = new Vector2D<int>(cD.TextureResolution[0], cD.TextureResolution[1]);
        Fov = cD.Fov;
        ViewDistance = cD.ViewDistance;
        RayCount = cD.RayCount;
        RaySteps = cD.RaySteps;
    }

    public Configuration(Vector2D<int> textureResolution, double fov, double viewDistance, int rayCount, int raySteps)
    {
        TextureResolution = textureResolution;
        Fov = fov;
        ViewDistance = viewDistance;
        RayCount = rayCount;
        RaySteps = raySteps;
    }
}
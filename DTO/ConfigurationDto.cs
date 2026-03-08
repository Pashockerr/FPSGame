public class ConfigurationDto
{
    public int[] TextureResolution { get; set; } = [800, 600];
    public double Fov { get; set; }
    public double ViewDistance { get; set; }
    public int RayCount { get; set; }
    public int RaySteps { get; set; }
    public double FocalLength { get; set; }
    public double ViewportHeight { get; set; }
}
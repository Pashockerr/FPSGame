using StbImageSharp;

public static class TextureManager
{
    public static Texture EMPTY = new()
    {
        Width = 1,
        Height = 1,
        Bytes = [0, 0, 0, 0]
    };
    public static Texture LoadTexture(string path)
    {
        var result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
        var tex = new Texture()
        {
            Width = result.Width,
            Height = result.Height,
            Bytes = result.Data
        };
        return tex;
    }
}
using System.Numerics;
using System.Text.Json;
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

    public void LoadFromFile(string path)
    {
        string content = File.ReadAllText(path);
        var doc = JsonDocument.Parse(content);
        MapDto mapDto = doc.Deserialize<MapDto>() ?? throw new Exception("Failed to load map from file");
    
        content = File.ReadAllText(mapDto.TilesConfigPath ?? throw new Exception("TilesConfigPath shouldn't be null!"));
        doc = JsonDocument.Parse(content);
        TilesConfigDto tilesConfig = doc.Deserialize<TilesConfigDto>() ?? throw new Exception("Failed to load tiles config file");

        Dictionary<char, Tile> tiles = new();
        for(int i = 0; i < tiles.Count(); ++i)
        {
            Tile tile = new Tile
            {
                TTexture = TextureManager.LoadTexture(tilesConfig.Tiles[i].TTexturePath ?? throw new Exception("TTexturePath shouldn't be null!")),
                BTexture = TextureManager.LoadTexture(tilesConfig.Tiles[i].BTexturePath ?? throw new Exception("BTexturePath shouldn't be null!")),
                LTexture = TextureManager.LoadTexture(tilesConfig.Tiles[i].LTexturePath ?? throw new Exception("LTexturePath shouldn't be null!")),
                RTexture = TextureManager.LoadTexture(tilesConfig.Tiles[i].RTexturePath ?? throw new Exception("RTexturePath shouldn't be null!")),
            };
            tiles.Add(tilesConfig.Tiles[i].Chracter, tile);
        }

        
    }
}
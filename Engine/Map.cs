using System.Text.Json;
using Silk.NET.Maths;

public class Map
{
    public readonly int Width;
    public readonly int Height;
    private Tile[] _tiles;

    public Map(string path)
    {
        string content = File.ReadAllText(path);
        var doc = JsonDocument.Parse(content);
        MapDto mapDto = doc.Deserialize<MapDto>() ?? throw new Exception("Failed to load map from file");

        Width = mapDto.Width;
        Height = mapDto.Height;
    
        content = File.ReadAllText(mapDto.TilesConfigPath ?? throw new Exception("TilesConfigPath shouldn't be null!"));
        doc = JsonDocument.Parse(content);
        TilesConfigDto tilesConfig = doc.Deserialize<TilesConfigDto>() ?? throw new Exception("Failed to load tiles config file");

        Dictionary<char, Tile> tiles = new();
        if(tilesConfig.Tiles == null)
        {
            throw new Exception("TilesConfigDto.Tiles shouldn't be null!");
        }
        for(int i = 0; i < tilesConfig.Tiles.Count(); ++i)
        {
            Tile tile = new Tile
            {
                TTexture = TextureManager.LoadTexture(tilesConfig.Tiles[i].TTexturePath ?? throw new Exception("TTexturePath shouldn't be null!")),
                BTexture = TextureManager.LoadTexture(tilesConfig.Tiles[i].BTexturePath ?? throw new Exception("BTexturePath shouldn't be null!")),
                LTexture = TextureManager.LoadTexture(tilesConfig.Tiles[i].LTexturePath ?? throw new Exception("LTexturePath shouldn't be null!")),
                RTexture = TextureManager.LoadTexture(tilesConfig.Tiles[i].RTexturePath ?? throw new Exception("RTexturePath shouldn't be null!")),
            };
            tiles.Add(tilesConfig.Tiles[i].Character, tile);
        }
        tiles.Add(tilesConfig.EmptyChar, Tile.EMPTY);
        
        _tiles = new Tile[mapDto.Width * mapDto.Height];
        for(int y = 0; y < mapDto.Height; ++y)
        {
            for(int x = 0; x < mapDto.Width; ++x)
            {
                int index = y*mapDto.Width + x;
                _tiles[index] = tiles[mapDto.Tiles?[index] ?? throw new Exception("MapDto.Tiles shouldn't be null!")];
                Console.Write(_tiles[index] == Tile.EMPTY ? " " : "=");
            }
            Console.WriteLine();
        }
    }

    public Map(int width, int height, Tile[] tiles)
    {
        Width = width;
        Height = height;
        _tiles = tiles;
    }

    public Tile GetTile(int x, int y)
    {
        if(x > Width - 1 || x < 0 || y > Height - 1 || y < 0)
            return Tile.EMPTY;
        return _tiles[y * Width + x];
    }

    public Tile GetTile(Vector2D<double> position)
    {
        int x = (int)Math.Round(position.X);
        int y = (int)Math.Round(position.Y);
        return GetTile(x, y); 
    }
}
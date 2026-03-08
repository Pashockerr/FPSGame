public class Tile
{
    public static readonly Tile EMPTY = new();
    public Texture TTexture = TextureManager.EMPTY; // Top
    public Texture BTexture = TextureManager.EMPTY; // Bottom
    public Texture LTexture = TextureManager.EMPTY; // Left
    public Texture RTexture = TextureManager.EMPTY; // Right
}

/*

----------+X
|         TOP
|     ___________
|     |         |
| LEFT|         | RIGHT
|     |         |
|     |_________|
|        BOTTOM
+ 
Y

*/
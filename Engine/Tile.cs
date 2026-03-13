public class Tile
{
    public static readonly Tile EMPTY = new();
    public Texture Texture = TextureManager.EMPTY;
}

public enum TileSide
{
    LEFT, RIGHT, BOTTOM, TOP
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

----------+U
|     Texture UV map 
|     ___________
|     | L  | R  |
|     |_________|      
|     | B  | T  |
|     |____|____|
|              
+ 
V
*/
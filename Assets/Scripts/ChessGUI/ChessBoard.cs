using UnityEngine;
using UnityEngine.Tilemaps;


public class ChessBoard : MonoBehaviour
{
    public Tilemap Board;
    public Tile DefaultTile;
    public Color LightSquareColor = new(0.94f, 0.85f, 0.71f);
    public Color DarkSquareColor = new(0.71f, 0.53f, 0.39f);


    private void Start()
    {
        if (Board == null)
        {
            Board = GetComponent<Tilemap>();
        }

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector3Int position = new(x, y);
                Board.SetTile(position, DefaultTile);
            }
        }
        SetSquareColors(LightSquareColor, DarkSquareColor);
    }


    public void SetSquareColors(Color lightSquareColor, Color darkSquareColor)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector3Int position = new(x, y);
                Color color = (x + y) % 2 == 1 ? lightSquareColor : darkSquareColor;
                TileFlags flags = Board.GetTileFlags(position);
                Board.SetTileFlags(position, flags & ~TileFlags.LockColor);
                Board.SetColor(position, color);
                Board.SetTileFlags(position, flags);
            }
        }
    }
}

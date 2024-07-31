using Chess.Core;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ChessLegalMovesVisualizer : MonoBehaviour
{
    public Tilemap LegalMoves;
    public Tile NonCapturingTile;
    public Tile CapturingTile;
    public UnityEngine.Color Color = new(0f, 0f, 0f, 0.1f);

    public ChessSelection Selection;


    private void Update()
    {
        Clear();
        if (Selection.SelectedPiece == null) return;
        foreach (LegalMove move in Selection.SelectedPiece.LegalMoves)
        {
            Vector3Int cell = new(move.Destination.X, move.Destination.Y);
            Tile tile = move.IsCapture ? CapturingTile : NonCapturingTile;
            LegalMoves.SetTile(cell, tile);
            LegalMoves.SetTileFlags(cell, ~TileFlags.LockColor);
            LegalMoves.SetColor(cell, Color);
        }
    }


    private void Clear()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector3Int cell = new(x, y);
                LegalMoves.SetTile(cell, null);
            }
        }
    }
}

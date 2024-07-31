using Chess.Core;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ChessSelectionHighlighter : MonoBehaviour
{
    public ChessSelection Selection;
    public Tilemap Board;
    public Tile DefaultTile;
    public UnityEngine.Color HighlightColor = new(1f, 1f, 0.2f, 0.5f);


    private Vector3Int? highlightedCell = null;


    private void Start()
    {
        if (Selection == null)
        {
            Selection = GetComponent<ChessSelection>();
        }
        if (Board == null)
        {
            Board = GetComponent<Tilemap>();
        }
    }


    private void Update()
    {
        if (Selection.SelectedPiece == null)
        {
            UnHighlight();
            return;
        }
        Position position = Selection.SelectedPiece.Position;
        Vector3Int cell = new(position.X, position.Y);
        UnHighlight();
        Highlight(cell);
    }


    private void Highlight(Vector3Int cell)
    {
        Board.SetTile(cell, DefaultTile);
        TileFlags flags = Board.GetTileFlags(cell);
        Board.SetTileFlags(cell, flags & ~TileFlags.LockColor);
        Board.SetColor(cell, HighlightColor);
        Board.SetTileFlags(cell, flags);
        highlightedCell = cell;
    }

    private void UnHighlight()
    {
        if (highlightedCell == null) return;
        Vector3Int cell = (Vector3Int) highlightedCell;
        Board.SetTile(cell, null);
        highlightedCell = null;
    }
}

using Chess.Core;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ChessPieceCursor : MonoBehaviour
{
    public ChessSelection Selection;
    public ChessPieces Pieces;


    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void LateUpdate()
    {
        if (Selection.SelectedPiece == null || !Input.GetMouseButton(0))
        {
            spriteRenderer.enabled = false;
            return;
        }
        Tile tile = Pieces.PieceToTile(Selection.SelectedPiece);
        spriteRenderer.sprite = tile.sprite;
        spriteRenderer.enabled = true;
        Position position = Selection.SelectedPiece.Position;
        Vector3Int cell = new(position.X, position.Y);
        Pieces.Pieces.SetTile(cell, null);
        transform.localPosition = MousePosition();
    }


    private Vector3 MousePosition()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = transform.localPosition.z;
        return position;
    }
}

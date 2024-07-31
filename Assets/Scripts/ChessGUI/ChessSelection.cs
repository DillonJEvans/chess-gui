using Chess.Core;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ChessSelection : MonoBehaviour
{
    public ChessGame Game;
    public Tilemap Board;

    public Piece SelectedPiece { get; private set; } = null;


    private void Start()
    {
        if (Board == null)
        {
            Board = GetComponent<Tilemap>();
        }
    }


    private void Update()
    {
        Position? mousePosition = MouseToPosition();
        if (Input.GetMouseButtonDown(0))
        {
            if (mousePosition == null)
            {
                SelectedPiece = null;
            }
            else if (IsActivePiece(mousePosition))
            {
                SelectedPiece = GetPiece(mousePosition);
            }
            else if (SelectedPiece != null)
            {
                Move(SelectedPiece, mousePosition);
                SelectedPiece = null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (SelectedPiece?.Position != mousePosition)
            {
                Move(SelectedPiece, mousePosition);
                SelectedPiece = null;
            }
        }
        Debug.Log(SelectedPiece);
    }


    private bool Move(Piece piece, Position? destination)
    {
        if (piece == null || destination == null) return false;
        return Move(piece, (Position) destination);
    }

    private bool Move(Piece piece, Position destination)
    {
        foreach (LegalMove move in Game.Game.LegalMoves)
        {
            if (move.Piece == piece && move.Destination == destination)
            {
                Game.Game.Move(move);
                return true;
            }
        }
        return false;
    }


    private Piece GetPiece(Position? position)
    {
        if (position == null) return null;
        return Game.Game.GetPiece((Position) position);
    }

    private bool IsActivePiece(Position? square)
    {
        return GetPiece(square)?.Color == Game.Game.ActiveColor;
    }


    private Position? MouseToPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cell = Board.WorldToCell(mousePosition);
        try
        {
            return new(cell.x, cell.y);
        }
        catch
        {
            return null;
        }
    }
}

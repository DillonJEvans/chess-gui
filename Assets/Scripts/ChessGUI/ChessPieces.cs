using Chess.Core;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ChessPieces : MonoBehaviour
{
    public ChessGame Game;
    public Tilemap Pieces;

    [Header("White Pieces")]
    public Tile WhiteKing;
    public Tile WhiteQueen;
    public Tile WhiteRook;
    public Tile WhiteBishop;
    public Tile WhiteKnight;
    public Tile WhitePawn;

    [Header("Black Pieces")]
    public Tile BlackKing;
    public Tile BlackQueen;
    public Tile BlackRook;
    public Tile BlackBishop;
    public Tile BlackKnight;
    public Tile BlackPawn;


    #nullable enable


    private void Start()
    {
        if (Pieces == null)
        {
            Pieces = GetComponent<Tilemap>();
        }
    }


    private void Update()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece? piece = Game.Game.GetPiece(x, y);
                Tile? tile = PieceToTile(piece);
                Pieces.SetTile(new Vector3Int(x, y), tile);
            }
        }
    }


    private Tile? PieceToTile(Piece? piece)
    {
        return piece?.ColorSymbol switch
        {
            'K' => WhiteKing,
            'Q' => WhiteQueen,
            'R' => WhiteRook,
            'B' => WhiteBishop,
            'N' => WhiteKnight,
            'P' => WhitePawn,
            'k' => BlackKing,
            'q' => BlackQueen,
            'r' => BlackRook,
            'b' => BlackBishop,
            'n' => BlackKnight,
            'p' => BlackPawn,
            _   => null
        };
    }
}

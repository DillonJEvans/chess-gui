#nullable enable


using Chess.Core;
using System.Collections.Generic;


namespace Chess.Pieces
{
    public class Rook : Piece
    {
        internal Rook(Game game, Color color, Position position)
            : base(game, color, position) { }


        public override char Symbol => 'R';


        protected internal override IEnumerable<PsuedoLegalMove> GeneratePsuedoLegalMoves()
        {
            ICollection<PsuedoLegalMove> psuedoLegalMoves = new List<PsuedoLegalMove>();
            AddMovesAlongRay(psuedoLegalMoves,  0,  1); // Up
            AddMovesAlongRay(psuedoLegalMoves,  1,  0); // Right
            AddMovesAlongRay(psuedoLegalMoves,  0, -1); // Down
            AddMovesAlongRay(psuedoLegalMoves, -1,  0); // Left
            return psuedoLegalMoves;
        }
    }
}

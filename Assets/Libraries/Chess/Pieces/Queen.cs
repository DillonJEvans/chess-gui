#nullable enable


using Chess.Core;
using System.Collections.Generic;


namespace Chess.Pieces
{
    public class Queen : Piece
    {
        internal Queen(Game game, Color color, Position position)
            : base(game, color, position) { }


        public override char Symbol => 'Q';


        protected internal override IEnumerable<PsuedoLegalMove> GeneratePsuedoLegalMoves()
        {
            ICollection<PsuedoLegalMove> psuedoLegalMoves = new List<PsuedoLegalMove>();
            AddMovesAlongRay(psuedoLegalMoves,  0,  1); // Up
            AddMovesAlongRay(psuedoLegalMoves,  1,  1); // Up   Right
            AddMovesAlongRay(psuedoLegalMoves,  1,  0); //      Right
            AddMovesAlongRay(psuedoLegalMoves,  1, -1); // Down Right
            AddMovesAlongRay(psuedoLegalMoves,  0, -1); // Down
            AddMovesAlongRay(psuedoLegalMoves, -1, -1); // Down Left
            AddMovesAlongRay(psuedoLegalMoves, -1,  0); //      Left
            AddMovesAlongRay(psuedoLegalMoves, -1,  1); // Up   Left
            return psuedoLegalMoves;
        }
    }
}

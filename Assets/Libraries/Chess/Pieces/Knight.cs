#nullable enable


using Chess.Core;
using System.Collections.Generic;


namespace Chess.Pieces
{
    public class Knight : Piece
    {
        internal Knight(Game game, Color color, Position position)
            : base(game, color, position) { }


        public override char Symbol => 'N';


        protected internal override IEnumerable<PsuedoLegalMove> GeneratePsuedoLegalMoves()
        {
            ICollection<PsuedoLegalMove> psuedoLegalMoves = new List<PsuedoLegalMove>();
            AddMove(psuedoLegalMoves, -1,  2); // 2 up     1 left
            AddMove(psuedoLegalMoves,  1,  2); // 2 up     1 right
            AddMove(psuedoLegalMoves,  2,  1); // 2 right  1 up
            AddMove(psuedoLegalMoves,  2, -1); // 2 right  1 down
            AddMove(psuedoLegalMoves,  1, -2); // 2 down   1 right
            AddMove(psuedoLegalMoves, -1, -2); // 2 down   1 left
            AddMove(psuedoLegalMoves, -2, -1); // 2 left   1 down
            AddMove(psuedoLegalMoves, -2,  1); // 2 left   1 up
            return psuedoLegalMoves;
        }
    }
}

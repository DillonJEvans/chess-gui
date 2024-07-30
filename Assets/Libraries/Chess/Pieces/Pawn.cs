#nullable enable


using Chess.Core;
using System.Collections.Generic;


namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        internal Pawn(Game game, Color color, Position position)
            : base(game, color, position) { }


        public override char Symbol => 'P';


        protected internal override IEnumerable<PsuedoLegalMove> GeneratePsuedoLegalMoves()
        {
            ICollection<PsuedoLegalMove> psuedoLegalMoves = new List<PsuedoLegalMove>();

            int forward = (Color == Color.White ? 1 : -1);
            int homeRow = (Color == Color.White ? 1 : 6);
            bool hasMoved = (Position.Y != homeRow);

            if (AddNonCapturingMove(psuedoLegalMoves, 0, forward) && !hasMoved)
            {
                AddNonCapturingMove(psuedoLegalMoves, 0, 2 * forward);
            }

            AddCapturingMove(psuedoLegalMoves, -1, forward);
            AddCapturingMove(psuedoLegalMoves, 1, forward);

            return psuedoLegalMoves;
        }


        protected override bool CanCapture(int deltaX, int deltaY)
        {
            if (!Position.Add(deltaX, deltaY, out Position destination))
            {
                return false;
            }
            Piece? piece = Game.GetPiece(destination);
            return (piece != null && piece.Color != Color) || destination == Game.EnPassantTarget;
        }

        protected override bool AddMove(ICollection<PsuedoLegalMove> psuedoLegalMoves,
                                        int deltaX,
                                        int deltaY)
        {
            // If the destination is not on the board, do not add the move.
            if (!Position.Add(deltaX, deltaY, out Position destination))
            {
                return false;
            }
            // If the move does not cause the pawn to promote, just add one move.
            if (destination.Y != 0 && destination.Y != 7)
            {
                psuedoLegalMoves.Add(new PsuedoLegalMove(this, destination));
            }
            // Otherwise, add a move for each piece that the pawn can promote to.
            else
            {
                Piece[] promotionPieces =
                {
                    new Queen (Game, Color, destination),
                    new Rook  (Game, Color, destination),
                    new Bishop(Game, Color, destination),
                    new Knight(Game, Color, destination)
                };
                foreach (Piece promotionPiece in promotionPieces)
                {
                    psuedoLegalMoves.Add(new PsuedoLegalMove(this, destination, promotionPiece));
                }
            }
            return true;
        }


        public override string ToString()
        {
            return Position.ToString();
        }
    }
}

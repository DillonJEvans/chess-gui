#nullable enable


using Chess.Pieces;
using System;
using System.Collections.Generic;
using System.Text;


namespace Chess.Core
{
    /// <summary>A legal move in chess.</summary>
    public class LegalMove
    {
        /// <summary>Uses a psuedo-legal move as the basis to create a legal move.</summary>
        /// <param name="game">The game that the move is for.</param>
        /// <param name="moves">All psuedo-legal moves in the position.</param>
        /// <param name="move">The psuedo-legal move that this move will mirror.</param>
        internal LegalMove(Game game, IEnumerable<PsuedoLegalMove> moves, PsuedoLegalMove move)
        {
            // Basics.
            Piece = move.Piece;
            Origin = move.Origin;
            Destination = move.Destination;
            Promotion = move.Promotion;
            // En passant.
            IsEnPassant = (Destination == game.EnPassantTarget);
            // Capturing.
            CapturedPiece = game.GetPiece(Destination);
            if (IsEnPassant)
            {
                int capturedForward = (Piece.Color == Color.White ? -1 : 1);
                int capturedY = Destination.Y + capturedForward;
                CapturedPiece = game.GetPiece(Destination.X, capturedY);
            }
            CapturedPosition = CapturedPiece?.Position;
            // Castling.
            IsCastle = (Piece is King && Math.Abs(Origin.X - Destination.X) > 1);
            IsKingsideCastle = (IsCastle && Origin.X < Destination.X);
            IsQueensideCastle = (IsCastle && Origin.X > Destination.X);
            // Castling rook.
            if (IsCastle)
            {
                int rookX = IsQueensideCastle ? 0 : 7;
                CastlingRook = (Rook?) game.GetPiece(rookX, Origin.Y);
                CastlingRookOrigin = CastlingRook?.Position;
                int destinationX = IsQueensideCastle ? 3 : 5;
                CastlingRookDestination = new Position(destinationX, Destination.Y);
            }
            else
            {
                CastlingRook = null;
                CastlingRookOrigin = null;
                CastlingRookDestination = null;
            }
            // Check and checkmate.
            IsCheck = game.IsCheck(move);
            IsCheckmate = game.IsCheckmate(move);
            // Disambiguation.
            IsFileDistinct = true;
            IsRankDistinct = true;
            foreach (PsuedoLegalMove other in moves)
            {
                if (other.Piece == move.Piece) continue;
                if (other.Piece.Symbol != Piece.Symbol) continue;
                if (other.Destination != Destination) continue;
                if (other.Origin.File == Origin.File) IsFileDistinct = false;
                if (other.Origin.Rank == Origin.Rank) IsRankDistinct = false;
            }
            // String representations.
            San = GenerateSan();
            Uci = GenerateUci();
        }


        /// <summary>The piece being moved.</summary>
        public readonly Piece Piece;
        /// <summary>The starting position of the piece being moved.</summary>
        public readonly Position Origin;
        /// <summary>The ending position of the piece being moved.</summary>
        public readonly Position Destination;
        /// <summary>
        /// The piece being captured, or null if the move does not capture a piece.
        /// </summary>
        public readonly Piece? CapturedPiece;
        /// <summary>
        /// The position of the piece being captured,
        /// or null if the move does not capture a piece.
        /// </summary>
        public readonly Position? CapturedPosition;
        /// <summary>
        /// The piece being promoted to, or null if the move is not a promotion.
        /// </summary>
        public readonly Piece? Promotion;

        /// <summary>True if the move captures a piece; otherwise, false.</summary>
        public bool IsCapture => CapturedPiece != null;
        /// <summary>True if the move is a promotion; otherwise, false.</summary>
        public bool IsPromotion => Promotion != null;

        /// <summary>True if the move is a castling move; otherwise, false.</summary>
        public readonly bool IsCastle;
        /// <summary>True if the move is a kingside castle; otherwise, false.</summary>
        public readonly bool IsKingsideCastle;
        /// <summary>True if the move is a queenside castle; otherwise, false.</summary>
        public readonly bool IsQueensideCastle;
        /// <summary>
        /// The rook being castled with, or null if the move is not a castling move.
        /// </summary>
        public readonly Rook? CastlingRook;
        /// <summary>
        /// The starting position of the castling rook,
        /// or null if the move is not a castling move.
        /// </summary>
        public readonly Position? CastlingRookOrigin;
        /// <summary>
        /// The ending position of the castling rook, or null if the move is not a castling move.
        /// </summary>
        public readonly Position? CastlingRookDestination;

        /// <summary>True if the move is an en passant capture; otherwise, false.</summary>
        public readonly bool IsEnPassant;
        /// <summary>True if the move checks the opposing king; otherwise, false.</summary>
        public readonly bool IsCheck;
        /// <summary>True if the move checkmates the opposing king; otherwise, false.</summary>
        public readonly bool IsCheckmate;

        /// <summary>
        /// True if the file of the move's origin is distinct; otherwise, false.
        /// </summary>
        public readonly bool IsFileDistinct;
        /// <summary>
        /// True if the rank of the move's origin is distinct; otherwise, false.
        /// </summary>
        public readonly bool IsRankDistinct;

        /// <summary>
        /// The
        /// <a href="https://en.wikipedia.org/wiki/Algebraic_notation_(chess)#Notation_for_moves">
        /// standard algebraic notation (SAN)
        /// </a>
        /// string that represents the move.
        /// </summary>
        public readonly string San;
        /// <summary>
        /// The
        /// <a href="https://en.wikipedia.org/wiki/Universal_Chess_Interface">
        /// Universal Chess Interface (UCI)
        /// </a>
        /// string that represents the move.
        /// </summary>
        public readonly string Uci;


        /// <summary>
        /// Returns the
        /// <a href="https://en.wikipedia.org/wiki/Algebraic_notation_(chess)#Notation_for_moves">
        /// standard algebraic notation (SAN)
        /// </a>
        /// string that represents the move.
        /// </summary>
        /// <returns>
        /// The
        /// <a href="https://en.wikipedia.org/wiki/Algebraic_notation_(chess)#Notation_for_moves">
        /// standard algebraic notation (SAN)
        /// </a>
        /// string that represents the move.
        /// </returns>
        public override string ToString() => San;


        /// <summary>
        /// Generates the
        /// <a href="https://en.wikipedia.org/wiki/Algebraic_notation_(chess)#Notation_for_moves">
        /// standard algebraic notation (SAN)
        /// </a>
        /// string that represents the move.
        /// </summary>
        /// <returns>
        /// The
        /// <a href="https://en.wikipedia.org/wiki/Algebraic_notation_(chess)#Notation_for_moves">
        /// standard algebraic notation (SAN)
        /// </a>
        /// string that represents the move.
        /// </returns>
        private string GenerateSan()
        {
            // Castling.
            if (IsCastle)
            {
                StringBuilder castlingSan = new StringBuilder("O-O");
                if (IsQueensideCastle) castlingSan.Append("-O");
                if (IsCheckmate) castlingSan.Append('#');
                else if (IsCheck) castlingSan.Append('+');
                return castlingSan.ToString();
            }
            // Non-castling.
            StringBuilder san = new StringBuilder();
            // Piece symbol.
            if (!(Piece is Pawn)) san.Append(char.ToUpper(Piece.Symbol));
            // Disambiguation.
            if (!IsFileDistinct) san.Append(Origin.File);
            if (!IsRankDistinct) san.Append(Origin.Rank);
            // Capture.
            if (IsCapture)
            {
                if (Piece is Pawn) san.Append(Origin.File);
                san.Append('x');
            }
            // Destination.
            san.Append(Destination.ToString());
            // Promotion.
            if (Promotion != null)
            {
                san.Append('=');
                san.Append(char.ToUpper(Promotion.Symbol));
            }
            // Check and checkmate.
            if (IsCheckmate) san.Append('#');
            else if (IsCheck) san.Append('+');
            // SAN.
            return san.ToString();
        }

        /// <summary>
        /// Generates the
        /// <a href="https://en.wikipedia.org/wiki/Universal_Chess_Interface">
        /// Universal Chess Interface (UCI)
        /// </a>
        /// string that represents the move.
        /// </summary>
        /// <returns>
        /// The
        /// <a href="https://en.wikipedia.org/wiki/Universal_Chess_Interface">
        /// Universal Chess Interface (UCI)
        /// </a>
        /// string that represents the move.
        /// </returns>
        private string GenerateUci()
        {
            if (Promotion == null)
            {
                return $"{Origin}{Destination}";
            }
            else
            {
                return $"{Origin}{Destination}{char.ToLower(Promotion.Symbol)}";
            }
        }
    }
}

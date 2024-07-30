#nullable enable


using Chess.Core;
using Chess.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Chess
{
    /// <summary>A game of chess.</summary>
    public class Game
    {
        // Temporary default constructor
        public Game()
        {
            board = new Piece[8, 8];
            // Kings
            WhiteKing = new King     (this, Color.White, new Position(4, 0));
            BlackKing = new King     (this, Color.Black, new Position(4, 7));
            // White pieces (1st rank)
            board[0, 0] = new Rook   (this, Color.White, new Position(0, 0));
            board[1, 0] = new Knight (this, Color.White, new Position(1, 0));
            board[2, 0] = new Bishop (this, Color.White, new Position(2, 0));
            board[3, 0] = new Queen  (this, Color.White, new Position(3, 0));
            board[4, 0] = WhiteKing;
            board[5, 0] = new Bishop (this, Color.White, new Position(5, 0));
            board[6, 0] = new Knight (this, Color.White, new Position(6, 0));
            board[7, 0] = new Rook   (this, Color.White, new Position(7, 0));
            // White pawns (2nd rank)
            board[0, 1] = new Pawn   (this, Color.White, new Position(0, 1));
            board[1, 1] = new Pawn   (this, Color.White, new Position(1, 1));
            board[2, 1] = new Pawn   (this, Color.White, new Position(2, 1));
            board[3, 1] = new Pawn   (this, Color.White, new Position(3, 1));
            board[4, 1] = new Pawn   (this, Color.White, new Position(4, 1));
            board[5, 1] = new Pawn   (this, Color.White, new Position(5, 1));
            board[6, 1] = new Pawn   (this, Color.White, new Position(6, 1));
            board[7, 1] = new Pawn   (this, Color.White, new Position(7, 1));
            // Black pawns (7th rank)
            board[0, 6] = new Pawn   (this, Color.Black, new Position(0, 6));
            board[1, 6] = new Pawn   (this, Color.Black, new Position(1, 6));
            board[2, 6] = new Pawn   (this, Color.Black, new Position(2, 6));
            board[3, 6] = new Pawn   (this, Color.Black, new Position(3, 6));
            board[4, 6] = new Pawn   (this, Color.Black, new Position(4, 6));
            board[5, 6] = new Pawn   (this, Color.Black, new Position(5, 6));
            board[6, 6] = new Pawn   (this, Color.Black, new Position(6, 6));
            board[7, 6] = new Pawn   (this, Color.Black, new Position(7, 6));
            // Black pieces (8th rank)
            board[0, 7] = new Rook   (this, Color.Black, new Position(0, 7));
            board[1, 7] = new Knight (this, Color.Black, new Position(1, 7));
            board[2, 7] = new Bishop (this, Color.Black, new Position(2, 7));
            board[3, 7] = new Queen  (this, Color.Black, new Position(3, 7));
            board[4, 7] = BlackKing;
            board[5, 7] = new Bishop (this, Color.Black, new Position(5, 7));
            board[6, 7] = new Knight (this, Color.Black, new Position(6, 7));
            board[7, 7] = new Rook   (this, Color.Black, new Position(7, 7));
            // White and Black Pieces
            pieces = new List<Piece>();
            foreach (Piece? piece in board)
            {
                if (piece == null) continue;
                pieces.Add(piece);
            }
            whitePieces = pieces.Where(piece => piece.Color == Color.White).ToList();
            blackPieces = pieces.Where(piece => piece.Color == Color.Black).ToList();
            // Legal moves
            ActiveColor = Color.White;
            FullMoveCount = 1;
            HalfMoveClock = 0;
            CanWhiteCastleKingside = true;
            CanWhiteCastleQueenside = true;
            CanBlackCastleKingside = true;
            CanBlackCastleQueenside = true;
            EnPassantTarget = null;
            legalMoves = new List<LegalMove>();
            UpdateLegalMoves();
        }


        /// <summary>The color whose turn it currently is.</summary>
        public Color ActiveColor { get; private set; }
        /// <summary>
        /// The number of full moves. Starts at 1 and is incremented after Black's move.
        /// </summary>
        public int FullMoveCount { get; private set; }
        /// <summary>The number of halfmoves since the last capture or pawn advance.</summary>
        public int HalfMoveClock { get; private set; }

        /// <summary>The white king.</summary>
        public readonly King WhiteKing;
        /// <summary>The black king.</summary>
        public readonly King BlackKing;

        /// <summary>The pieces.</summary>
        public IReadOnlyCollection<Piece> Pieces => pieces.AsReadOnly();
        /// <summary>The white pieces.</summary>
        public IReadOnlyCollection<Piece> WhitePieces => whitePieces.AsReadOnly();
        /// <summary>The black pieces.</summary>
        public IReadOnlyCollection<Piece> BlackPieces => blackPieces.AsReadOnly();

        /// <summary>The current legal moves.</summary>
        public IReadOnlyCollection<LegalMove> LegalMoves => legalMoves.AsReadOnly();

        /// <summary>True if white has the right to castle kingside; otherwise, false.</summary>
        public bool CanWhiteCastleKingside { get; private set; }
        /// <summary>True if white has the right to castle queenside; otherwise, false.</summary>
        public bool CanWhiteCastleQueenside { get; private set; }
        /// <summary>True if black has the right to castle kingside; otherwise, false.</summary>
        public bool CanBlackCastleKingside { get; private set; }
        /// <summary>True if black has the right to castle queenside; otherwise, false.</summary>
        public bool CanBlackCastleQueenside { get; private set; }

        /// <summary>
        /// The current en passant target that would be the destination of an en passant move.
        /// <c>null</c> if the last move was not a pawn moving two squares.
        /// </summary>
        public Position? EnPassantTarget { get; private set; }

        private Piece?[,] board = new Piece?[8, 8];
        private List<Piece> pieces;
        private List<Piece> whitePieces;
        private List<Piece> blackPieces;

        private List<LegalMove> legalMoves;


        /// <summary>Gets the piece at the position on the board.</summary>
        /// <param name="x">The file (column) of the position.</param>
        /// <param name="y">The rank (row) of the position.</param>
        /// <returns>The piece at the position on the board.</returns>
        public Piece? GetPiece(int x, int y)
        {
            return GetPiece(new Position(x, y));
        }

        /// <summary>Gets the piece at the position on the board.</summary>
        /// <param name="position">The position on the board.</param>
        /// <returns>The piece at the position on the board.</returns>
        public Piece? GetPiece(Position position)
        {
            return board[position.X, position.Y];
        }

        /// <summary>Gets the piece at the position on the board.</summary>
        /// <param name="algebraicNotation">
        /// The position on the board, represented by
        /// <a href="https://en.wikipedia.org/wiki/Algebraic_notation_(chess)">
        /// algebraic notation
        /// </a>.
        /// </param>
        /// <returns>The piece at the position on the board.</returns>
        public Piece? GetPiece(string algebraicNotation)
        {
            return GetPiece(new Position(algebraicNotation));
        }


        /// <summary>Retrieves the king of the specified color.</summary>
        /// <param name="color">The color of the king to retrieve.</param>
        /// <returns>The king of the specified color.</returns>
        public King GetKing(Color color)
        {
            return color == Color.White ? WhiteKing : BlackKing;
        }

        /// <summary>Retrieves the pieces of the specified color.</summary>
        /// <param name="color">The color of the pieces to retrieve.</param>
        /// <returns>The pieces of the specified color.</returns>
        public IReadOnlyCollection<Piece> GetPieces(Color color)
        {
            return color == Color.White ? WhitePieces : BlackPieces;
        }

        /// <summary>Determines if the specified color has the right to castle kingside.</summary>
        /// <param name="color">The color.</param>
        /// <returns>
        /// True if the color has the right to castle kingside; otherwise, false.
        /// </returns>
        public bool CanCastleKingside(Color color)
        {
            return color == Color.White ? CanWhiteCastleKingside : CanBlackCastleKingside;
        }

        /// <summary>
        /// Determines if the specified color has the right to castle queenside.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>
        /// True if the color has the right to castle queenside; otherwise, false.
        /// </returns>
        public bool CanCastleQueenside(Color color)
        {
            return color == Color.White ? CanWhiteCastleQueenside : CanBlackCastleQueenside;
        }


        /// <summary>Makes a move.</summary>
        /// <param name="move">The legal move to make.</param>
        /// <returns>True if the move was legal and was made; otherwise, false.</returns>
        public bool Move(LegalMove move)
        {
            // If the move isn't legal, don't make it.
            if (!LegalMoves.Contains(move))
            {
                return false;
            }
            // Remove the captured piece.
            if (move.CapturedPiece != null)
            {
                List<Piece> capturedPieces = GetMutablePieces(move.CapturedPiece.Color);
                capturedPieces.Remove(move.CapturedPiece);
                SetPieceOnBoard(move.CapturedPosition ?? default, null);
            }
            // Move the piece.
            SetPieceOnBoard(move.Origin, null);
            SetPieceOnBoard(move.Destination, move.Piece);
            move.Piece.Position = move.Destination;
            // Move the rook when castling.
            if (move.CastlingRook != null)
            {
                SetPieceOnBoard(move.CastlingRookOrigin ?? default, null);
                SetPieceOnBoard(move.CastlingRookDestination ?? default, move.CastlingRook);
                move.CastlingRook.Position = move.CastlingRookDestination ?? default;
            }
            // Handle promotions.
            if (move.Promotion != null)
            {
                List<Piece> pieces = GetMutablePieces(move.Piece.Color);
                pieces.Remove(move.Piece);
                pieces.Add(move.Promotion);
                SetPieceOnBoard(move.Piece.Position, null);
                SetPieceOnBoard(move.Destination, move.Promotion);
                move.Promotion.Position = move.Destination;
            }
            // Castling rights.
            UpdateCastlingRights(move);
            // En Passant.
            if (move.Piece is Pawn && Math.Abs(move.Origin.Y - move.Destination.Y) == 2)
            {
                int enPassantY = (move.Origin.Y + move.Destination.Y) / 2;
                EnPassantTarget = new Position(move.Destination.X, enPassantY);
            }
            else
            {
                EnPassantTarget = null;
            }
            // Update the full move and half move counters,
            // toggle the active color, and update legal moves.
            if (ActiveColor == Color.Black) FullMoveCount++;
            if (move.IsCapture || move.Piece is Pawn)
            {
                HalfMoveClock = 0;
            }
            else
            {
                HalfMoveClock++;
            }
            ActiveColor = ActiveColor.Opposite();
            UpdateLegalMoves();
            return true;
        }


        /// <summary>
        /// Updates <c>LegalMoves</c> for all pieces.
        /// Updates <c>LegalMoves</c> for this <c>Game</c>.
        /// </summary>
        private void UpdateLegalMoves()
        {
            List<PsuedoLegalMove> psuedoLegalMoves = new List<PsuedoLegalMove>();
            IReadOnlyCollection<Piece> activePieces = GetPieces(ActiveColor);
            IReadOnlyCollection<Piece> inactivePieces = GetPieces(ActiveColor.Opposite());
            // Get the psuedo-legal moves for all active pieces.
            foreach (Piece piece in activePieces)
            {
                IEnumerable<PsuedoLegalMove> moves = piece.GeneratePsuedoLegalMoves();
                psuedoLegalMoves.AddRange(moves);
            }
            legalMoves.Clear();
            // Convert the legal moves to LegalMoves.
            foreach (PsuedoLegalMove move in psuedoLegalMoves)
            {
                if (!IsLegalMove(move)) continue;
                legalMoves.Add(new LegalMove(this, psuedoLegalMoves, move));
            }
            // Set each piece's legal moves.
            foreach (Piece piece in activePieces)
            {
                IEnumerable<LegalMove> moves = legalMoves.Where(move => move.Piece == piece);
                piece.SetLegalMoves(moves);
            }
            foreach (Piece piece in inactivePieces)
            {
                piece.ClearLegalMoves();
            }
        }


        /// <summary>Determines if the psuedo-legal move is legal or not.</summary>
        /// <param name="psuedoLegalMove">A psuedo-legal move.</param>
        /// <returns>True if the psuedo-legal move is legal; otherwise, false.</returns>
        internal bool IsLegalMove(PsuedoLegalMove move)
        {
            // Temporarily make the move.
            Piece? capturedPiece = MakePsuedoLegalMove(move);
            // Get the king and opposing pieces.
            Color color = move.Piece.Color;
            Position king = GetKing(color).Position;
            IReadOnlyCollection<Piece> opposingPieces = GetPieces(color.Opposite());
            if (king == move.Origin) king = move.Destination;
            // Check if the king is in check after the move is made.
            bool isLegalMove = !opposingPieces.Any(piece =>
                piece != capturedPiece && piece.IsAttacking(king));
            // Undo the move.
            UndoPsuedoLegalMove(move, capturedPiece);
            return isLegalMove;
        }

        /// <summary>
        /// Determines if <paramref name="position"/> is being attacked
        /// by <paramref name="color"/>.
        /// </summary>
        /// <param name="position">The position that might be being attacked.</param>
        /// <param name="color">The color that might be attacking position.</param>
        /// <returns>True if the position is attacked; otherwise, false.</returns>
        internal bool IsAttacked(Position position, Color color)
        {
            IReadOnlyCollection<Piece> attackingPieces = GetPieces(color);
            return attackingPieces.Any(piece => piece.IsAttacking(position));
        }

        /// <summary>Determines if the move checks the opposing king.</summary>
        /// <param name="move">The move that might be checking the opposing king.</param>
        /// <returns>True if the move checks the opposing king; otherwise, false.</returns>
        internal bool IsCheck(PsuedoLegalMove move)
        {
            // Temporarily make the move.
            Piece? capturedPiece = MakePsuedoLegalMove(move);
            // Check if the opposing king is in check after the move is made.
            Color color = move.Piece.Color;
            Position opposingKing = GetKing(color.Opposite()).Position;
            bool isCheck = IsAttacked(opposingKing, color);
            // Undo the move.
            UndoPsuedoLegalMove(move, capturedPiece);
            return isCheck;
        }

        /// <summary>Determines if the move checkmates the opposing king.</summary>
        /// <param name="move">The move that might be checkmating the opposing king.</param>
        /// <returns>True if the move checkmates the opposing king; otherwise, false.</returns>
        internal bool IsCheckmate(PsuedoLegalMove move)
        {
            // Temporarily make the move.
            Piece? capturedPiece = MakePsuedoLegalMove(move);
            // Get the defending king and pieces.
            Color color = move.Piece.Color;
            IReadOnlyCollection<Piece> defendingPieces = GetPieces(color.Opposite());
            Position defendingKing = GetKing(color.Opposite()).Position;
            // Check if the opposing king is in check and if there are no legal moves
            // after the move is made.
            bool isCheck = IsAttacked(defendingKing, color);
            bool isCheckmate = true;
            foreach (Piece piece in defendingPieces)
            {
                if (piece == capturedPiece) continue;
                IEnumerable<PsuedoLegalMove> moves = piece.GeneratePsuedoLegalMoves();
                if (moves.Any(move => IsLegalMove(move)))
                {
                    isCheckmate = false;
                    break;
                }
            }
            // Undo the move.
            UndoPsuedoLegalMove(move, capturedPiece);
            return isCheck && isCheckmate;
        }


        /// <summary>Temporarily makes a psuedo-legal move.</summary>
        /// <param name="move">The move to temporarily make.</param>
        /// <returns>The captured piece, or null if no piece was captured.</returns>
        /// <remarks>
        /// Meant to be used in conjunction with <c>UndoPseudoLegalMove</c>.
        /// The captured piece is not removed from the list of pieces.
        /// </remarks>
        private Piece? MakePsuedoLegalMove(PsuedoLegalMove move)
        {
            Piece? capturedPiece = GetPiece(move.Destination);
            SetPieceOnBoard(move.Origin, null);
            SetPieceOnBoard(move.Destination, move.Piece);
            move.Piece.Position = move.Destination;
            return capturedPiece;
        }

        /// <summary>Undoes a temporarily made psuedo-legal move.</summary>
        /// <param name="move">The move to undo.</param>
        /// <param name="capturedPiece">
        /// The piece that was captured, or null if no piece was captured.
        /// </param>
        /// <remarks>
        /// Meant to be used in conjunction with <c>MakePseudoLegalMove</c>.
        /// </remarks>
        private void UndoPsuedoLegalMove(PsuedoLegalMove move, Piece? capturedPiece)
        {
            SetPieceOnBoard(move.Origin, move.Piece);
            SetPieceOnBoard(move.Destination, capturedPiece);
            move.Piece.Position = move.Origin;
        }


        /// <summary>Updates castling rights after a move is made.</summary>
        /// <param name="move">The move being made.</param>
        private void UpdateCastlingRights(LegalMove move)
        {
            // Rook moves.
            if (move.Origin == new Position("a1")) CanWhiteCastleQueenside = false;
            if (move.Origin == new Position("h1")) CanWhiteCastleKingside = false;
            if (move.Origin == new Position("a1")) CanBlackCastleQueenside = false;
            if (move.Origin == new Position("h8")) CanBlackCastleKingside = false;
            // King moves.
            if (move.Origin == new Position("e1"))
            {
                CanWhiteCastleKingside = false;
                CanWhiteCastleQueenside = false;
            }
            if (move.Origin == new Position("e8"))
            { 
                CanBlackCastleKingside = false;
                CanBlackCastleQueenside = false;
            }
        }


        /// <summary>Retrieves a mutable list of pieces of the specified color.</summary>
        /// <param name="color">The color of the pieces to retrieve.</param>
        /// <returns>A mutable list of pieces of the specified color.</returns>
        private List<Piece> GetMutablePieces(Color color)
        {
            return color == Color.White ? whitePieces : blackPieces;
        }

        /// <summary>Sets the position on the board to the piece.</summary>
        /// <param name="position">The position on the board to set.</param>
        /// <param name="piece">The piece to set the position on the board to.</param>
        private void SetPieceOnBoard(Position position, Piece? piece)
        {
            board[position.X, position.Y] = piece;
        }
    }
}

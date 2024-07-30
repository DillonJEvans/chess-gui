#nullable enable


using System.Collections.Generic;
using System.Linq;


namespace Chess.Core
{
    /// <summary>A chess piece.</summary>
    public abstract class Piece
    {
        /// <summary>Creates a piece.</summary>
        /// <param name="game">The game the piece belong to.</param>
        /// <param name="color">The coloor of the piece.</param>
        /// <param name="position">The position of the piece.</param>
        protected internal Piece(Game game, Color color, Position position)
        {
            Game = game;
            Color = color;
            Position = position;
            legalMoves = new List<LegalMove>();
        }


        /// <summary>The color of the piece.</summary>
        public readonly Color Color;
        /// <summary>The position of the piece.</summary>
        public Position Position { get; internal set; }
        /// <summary>The current legal moves.</summary>
        public IReadOnlyCollection<LegalMove> LegalMoves => legalMoves.AsReadOnly();

        /// <summary>The symbol representing the piece.</summary>
        public abstract char Symbol { get; }
        /// <summary>
        /// The symbol representing the piece.
        /// Uppercase if the piece is white, lowercase if the piece is black.
        /// </summary>
        public char ColorSymbol
        {
            get
            {
                if (Color == Color.White) return char.ToUpper(Symbol);
                else return char.ToLower(Symbol);
            }
        }

        protected Game Game { get; }

        private List<LegalMove> legalMoves;


        /// <summary>Sets <c>LegalMoves</c> to the moves.</summary>
        /// <param name="moves">The moves to set <c>LegalMoves</c> to.</param>
        internal void SetLegalMoves(IEnumerable<LegalMove> moves)
        {
            ClearLegalMoves();
            legalMoves.AddRange(moves);
        }

        /// <summary>Clears <c>LegalMoves</c>.</summary>
        internal void ClearLegalMoves()
        {
            legalMoves.Clear();
        }

        /// <summary>Determines if this piece is attacking a position on the board.</summary>
        /// <param name="position">The position to check if this piece is attacking.</param>
        /// <returns>True if this piece is attacking the position; otherwise, false.</returns>
        internal bool IsAttacking(Position position)
        {
            IEnumerable<PsuedoLegalMove> psuedoLegalMoves = GeneratePsuedoLegalMoves();
            return psuedoLegalMoves.Any(move => move.Destination == position);
        }


        /// <summary>
        /// Generates an enumerable of
        /// <a href="https://www.chessprogramming.org/Pseudo-Legal_Move">
        /// psuedo-legal moves
        /// </a>,
        /// or moves that may be legal, but do not consider
        /// whether the king is in check after the move is made.
        /// </summary>
        /// <returns>
        /// An enumerable of
        /// <a href="https://www.chessprogramming.org/Pseudo-Legal_Move">
        /// psuedo-legal moves
        /// </a>.
        /// </returns>
        protected internal abstract IEnumerable<PsuedoLegalMove> GeneratePsuedoLegalMoves();


        /// <summary>
        /// Determines if this piece can capture a piece by moving to the destination.
        /// </summary>
        /// <param name="deltaX">
        /// The difference in X from <c>Position</c> to the destination.
        /// </param>
        /// <param name="deltaY">
        /// The difference in Y from <c>Position</c> to the destination.
        /// </param>
        /// <returns>True if a piece can be captured; otherwise, false.</returns>
        protected virtual bool CanCapture(int deltaX, int deltaY)
        {
            if (!Position.Add(deltaX, deltaY, out Position destination))
            {
                return false;
            }
            Piece? piece = Game.GetPiece(destination);
            return piece != null && piece.Color != Color;
        }

        /// <summary>
        /// Determines if the relative position is not occupied by a piece.
        /// </summary>
        /// <param name="deltaX">
        /// The difference in X from <c>Position</c> to the destination.
        /// </param>
        /// <param name="deltaY">
        /// The difference in Y from <c>Position</c> to the destination.
        /// </param>
        /// <returns>True if the relative position is unoccupied; otherwise, false.</returns>
        protected virtual bool IsRelativePositionUnoccupied(int deltaX, int deltaY)
        {
            if (!Position.Add(deltaX, deltaY, out Position square))
            {
                return false;
            }
            Piece? piece = Game.GetPiece(square);
            return piece == null;
        }

        /// <summary>
        /// Potentially adds a move to <paramref name="psuedoLegalMoves"/>.
        /// The origin of the move would be <c>Position</c>,
        /// and the destination would be <c>Position</c> plus the given delta.
        /// The move is not added if the destination
        /// is off the board or occupied by a friendly piece.
        /// </summary>
        /// <param name="psuedoLegalMoves">The collection of psuedo-legal moves to add to.</param>
        /// <param name="deltaX">
        /// The difference in X from <c>Position</c> to the destination.
        /// </param>
        /// <param name="deltaY">
        /// The difference in Y from <c>Position</c> to the destination.
        /// </param>
        /// <returns>True if the move was added; otherwise, false.</returns>
        protected virtual bool AddMove(ICollection<PsuedoLegalMove> psuedoLegalMoves,
                                       int deltaX,
                                       int deltaY)
        {
            // Don't add the move if the destination is not on the board.
            if (!Position.Add(deltaX, deltaY, out Position destination))
            {
                return false;
            }
            Piece? capturedPiece = Game.GetPiece(destination);
            // If the destination is occupied by a friendly piece, do not add the move.
            if (capturedPiece?.Color == Color)
            {
                return false;
            }
            psuedoLegalMoves.Add(new PsuedoLegalMove(this, destination));
            return true;
        }

        /// <summary>
        /// Starts at <c>Position</c>, going in the given direction one square
        /// at a time, adding moves to <paramref name="psuedoLegalMoves"/>
        /// that go from <c>Position</c> to each square.
        /// Stops once the end of the board or another piece is found.
        /// </summary>
        /// <param name="psuedoLegalMoves">
        /// The collection of psuedo-legal moves to add to.
        /// </param>
        /// <param name="directionX">The X of the ray's direction.</param>
        /// <param name="directionY">The Y of the ray's direction.</param>
        protected virtual void AddMovesAlongRay(ICollection<PsuedoLegalMove> psuedoLegalMoves,
                                                int directionX,
                                                int directionY)
        {
            Position destination = Position;
            Piece? capturedPiece = null;
            // Keep adding moves until a piece is found,
            // or until the end of the board is reached.
            while (capturedPiece == null &&
                   destination.Add(directionX, directionY, out destination))
            {
                capturedPiece = Game.GetPiece(destination);
                // Only add the move if the square is unoccupied,
                // or occupied by an opposing piece.
                if (capturedPiece?.Color != Color)
                {
                    psuedoLegalMoves.Add(new PsuedoLegalMove(this, destination));
                }
            }
        }

        /// <summary>
        /// Adds a move to <paramref name="psuedoLegalMoves"/>.
        /// Only adds the move if it does not capture a piece.
        /// </summary>
        /// <param name="psuedoLegalMoves">The collection of psuedo-legal moves to add to.</param>
        /// <param name="deltaX">
        /// The difference in X from <c>Position</c> to the destination.
        /// </param>
        /// <param name="deltaY">
        /// The difference in Y from <c>Position</c> to the destination.
        /// </param>
        /// <returns>True if the move was added; otherwise, false.</returns>
        protected virtual bool AddNonCapturingMove(ICollection<PsuedoLegalMove> psuedoLegalMoves,
                                                   int deltaX,
                                                   int deltaY)
        {
            if (!IsRelativePositionUnoccupied(deltaX, deltaY))
            {
                return false;
            }
            return AddMove(psuedoLegalMoves, deltaX, deltaY);
        }

        /// <summary>
        /// Adds a move to <paramref name="psuedoLegalMoves"/>.
        /// Only adds the move if it captures a piece.
        /// </summary>
        /// <param name="psuedoLegalMoves">The collection of psuedo-legal moves to add to.</param>
        /// <param name="deltaX">
        /// The difference in X from <c>Position</c> to the destination.
        /// </param>
        /// <param name="deltaY">
        /// The difference in Y from <c>Position</c> to the destination.
        /// </param>
        /// <returns>True if the move was added; otherwise, false.</returns>
        protected virtual bool AddCapturingMove(ICollection<PsuedoLegalMove> psuedoLegalMoves,
                                                int deltaX,
                                                int deltaY)
        {
            if (!CanCapture(deltaX, deltaY))
            {
                return false;
            }
            return AddMove(psuedoLegalMoves, deltaX, deltaY);
        }


        public override string ToString()
        {
            return $"{ColorSymbol}{Position}";
        }
    }
}

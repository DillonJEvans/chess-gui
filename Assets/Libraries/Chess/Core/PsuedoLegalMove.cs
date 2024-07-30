#nullable enable


namespace Chess.Core
{
    /// <summary>
    /// A
    /// <a href="https://www.chessprogramming.org/Pseudo-Legal_Move">
    /// psuedo-legal move
    /// </a>
    /// in chess.
    /// </summary>
    public class PsuedoLegalMove
    {
        /// <summary>
        /// Creates a psuedo-legal move.
        /// </summary>
        /// <param name="piece">The piece being moved.</param>
        /// <param name="destination">The ending position of the piece being moved.</param>
        /// <param name="promotion">
        /// The piece being promoted to, or null if the move is not a promotion.
        /// </param>
        public PsuedoLegalMove(Piece piece, Position destination, Piece? promotion = null)
        {
            Piece = piece;
            Origin = piece.Position;
            Destination = destination;
            Promotion = promotion;
            Uci = GenerateUci();
        }


        /// <summary>The piece being moved.</summary>
        public readonly Piece Piece;
        /// <summary>The starting position of the piece being moved.</summary>
        public readonly Position Origin;
        /// <summary>The ending position of the piece being moved.</summary>
        public readonly Position Destination;
        /// <summary>
        /// The piece being promoted to, or null if the move is not a promotion.
        /// </summary>
        public readonly Piece? Promotion;

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
        public override string ToString() => Uci;


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

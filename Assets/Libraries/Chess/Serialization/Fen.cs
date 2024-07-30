#nullable enable


using Chess.Core;
using System.Collections.Generic;
using System.Text;


namespace Chess.Serialization
{
    /// <summary>
    /// Contains methods for serializing and deserializing
    /// <a href="https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation">
    /// Forsyth-Edwards Notation (FEN)
    /// </a>
    /// strings.
    /// </summary>
    public static class Fen
    {
        /// <summary>
        /// Serializes a game to a
        /// <a href="https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation">
        /// Forsyth-Edwards Notation (FEN)
        /// </a>
        /// string.
        /// </summary>
        /// <param name="game">The game to serialize.</param>
        /// <returns>
        /// The
        /// <a href="https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation">
        /// Forsyth-Edwards Notation (FEN)
        /// </a>
        /// string representing the game.
        /// </returns>
        public static string Serialize(Game game)
        {
            string[] fields =
            {
                SerializePieces(game),
                SerializeActiveColor(game),
                SerializeCastlingRights(game),
                SerializeEnPassantTarget(game),
                game.HalfMoveClock.ToString(),
                game.FullMoveCount.ToString()
            };
            return string.Join(' ', fields);
        }


        private static string SerializePieces(Game game)
        {
            ICollection<string> ranks = new List<string>();
            for (int y = 7; y >= 0; y--)
            {
                StringBuilder rank = new StringBuilder();
                int emptySquares = 0;
                for (int x = 0; x <= 7; x++)
                {
                    Piece? piece = game.GetPiece(x, y);
                    if (piece == null)
                    {
                        emptySquares++;
                    }
                    else
                    {
                        if (emptySquares > 0) rank.Append(emptySquares);
                        emptySquares = 0;
                        rank.Append(piece.ColorSymbol);
                    }
                }
                if (emptySquares > 0) rank.Append(emptySquares);
                ranks.Add(rank.ToString());
            }
            return string.Join("/", ranks);
        }

        private static string SerializeActiveColor(Game game)
        {
            return game.ActiveColor switch
            {
                Color.White => "w",
                Color.Black => "b",
                _ => string.Empty
            };
        }

        private static string SerializeCastlingRights(Game game)
        {
            StringBuilder castling = new StringBuilder();
            if (game.CanWhiteCastleKingside) castling.Append('K');
            if (game.CanWhiteCastleQueenside) castling.Append('Q');
            if (game.CanBlackCastleKingside) castling.Append('k');
            if (game.CanBlackCastleQueenside) castling.Append('q');
            if (castling.Length == 0) castling.Append('-');
            return castling.ToString();
        }

        private static string SerializeEnPassantTarget(Game game)
        {
            if (game.EnPassantTarget == null) return "-";
            return game.EnPassantTarget.ToString();
        }
    }
}

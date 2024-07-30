#nullable enable


using System.Diagnostics.Contracts;


namespace Chess.Core
{
    /// <summary>
    /// A position on the chess board.
    /// </summary>
    /// <remarks>
    /// This struct is immutable.
    /// X and Y will always be between 0 and 7,
    /// where (0, 0) is a1 and (7, 7) is h8.
    /// </remarks>
    public readonly struct Position
    {
        /// <summary>
        /// Constructs a new position with the given X and Y.
        /// </summary>
        /// <param name="x">
        /// The X of the position. Must be between 0 and 7.
        /// </param>
        /// <param name="y">
        /// The Y of the position. Must be between 0 and 7.
        /// </param>
        public Position(int x, int y)
        {
            Contract.Requires(x < 0 || x > 7, "X must be between 0 and 7.");
            Contract.Requires(y < 0 || y > 7, "Y must be between 0 and 7.");
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs a new position from
        /// <a href="https://en.wikipedia.org/wiki/Algebraic_notation_(chess)">
        /// algebraic notation
        /// </a>.
        /// </summary>
        /// <param name="algebraicNotation">
        /// The position in
        /// <a href="https://en.wikipedia.org/wiki/Algebraic_notation_(chess)">
        /// algebraic notation
        /// </a>.
        /// </param>
        public Position(string algebraicNotation)
        {
            algebraicNotation = algebraicNotation.Trim();
            Contract.Requires(algebraicNotation.Length != 2,
                "Algebraic notation must have a length of exactly 2.");
            int x = char.ToLower(algebraicNotation[0]) - 'a';
            int y = algebraicNotation[1] - '1';
            Contract.Requires(x < 0 || x > 7, "File must be between a and h.");
            Contract.Requires(y < 0 || y > 7, "Rank must be between 1 and 8.");
            X = x;
            Y = y;
        }


        /// <summary>
        /// The X component of the position.
        /// </summary>
        /// <remarks>
        /// Between 0 and 7 inclusive,
        /// where 0 is the 'a' file and 7 is the 'h' file.
        /// </remarks>
        public readonly int X;
        /// <summary>
        /// The Y component of the position.
        /// </summary>
        /// <remarks>
        /// Between 0 and 7 inclusive,
        /// where 0 is the 1st rank and 7 is the 8th rank.
        /// </remarks>
        public readonly int Y;

        /// <summary>
        /// The
        /// <a href="https://en.wikipedia.org/wiki/Chessboard">
        /// file
        /// </a>
        /// of the position.
        /// </summary>
        /// <remarks>
        /// Between 'a' and 'h', inclusive.
        /// </remarks>
        public readonly char File => (char) ('a' + X);
        /// <summary>
        /// The
        /// <a href="https://en.wikipedia.org/wiki/Chessboard">
        /// rank
        /// </a>
        /// of the position.
        /// </summary>
        /// <remarks>
        /// Between 1 and 8, inclusive.
        /// </remarks>
        public readonly int Rank => Y + 1;


        /// <summary>
        /// Adds (x, y) to this position,
        /// storing the result in <paramref name="result"/>.
        /// </summary>
        /// <remarks>
        /// Does not alter this position since positions are immutable.
        /// </remarks>
        /// <param name="x">The X to add to this position.</param>
        /// <param name="y">The Y to add to this position.</param>
        /// <param name="result">
        /// The result of the addition. Ignore this value if false is returned.
        /// </param>
        /// <returns>
        /// True if adding (x, y) to this position was a success;
        /// false if the addition would have produced an invalid position.
        /// </returns>
        internal bool Add(int x, int y, out Position result)
        {
            int sumX = X + x;
            int sumY = Y + y;
            if (sumX < 0 || sumX > 7 || sumY < 0 || sumY > 7)
            {
                result = default;
                return false;
            }
            result = new Position(sumX, sumY);
            return true;
        }


        public override int GetHashCode()
        {
            // X and Y can only be between 0 and 7 (inclusive).
            // The following produces unique hash codes for each position,
            // based on how squares of the chess board are commonly numbered.
            // For example: http://hgm.nubati.net/book_format.html
            // A description of the Polyglot opening book format uses:
            //   a1 == 0, h1 == 7, a2 == 8, ..., h8 == 63
            // which is reproduced by this hashing function.
            return Y * 8 + X;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Position position)
            {
                return Equals(position);
            }
            return false;
        }

        public bool Equals(Position position)
        {
            return X == position.X && Y == position.Y;
        }

        public static bool operator ==(Position lhs, Position rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Position lhs, Position rhs)
        {
            return !(lhs == rhs);
        }


        public override string ToString() => $"{File}{Rank}";
    }
}

#nullable enable


namespace Chess.Core
{
    /// <summary>The two colors (white and black) in chess.</summary>
    public enum Color
    {
        White,
        Black
    }


    /// <summary>Extension methods for the <c>Color</c> enum.</summary>
    public static class ColorExtension
    {
        /// <summary>Gets the opposite of <paramref name="color"/>.</summary>
        /// <param name="color">The original color.</param>
        /// <returns>The opposite of <paramref name="color"/>.</returns>
        public static Color Opposite(this Color color)
        {
            return color switch
            {
                Color.White => Color.Black,
                Color.Black => Color.White,
                _ => default
            };
        }
    }
}

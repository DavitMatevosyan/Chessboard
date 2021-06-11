using System.Diagnostics;

namespace Chessboard.Models.Positions
{
    /// <summary>
    /// Indicates the Position of a Piece
    /// </summary>
    [DebuggerDisplay("col:{Column} row:{Row}")]
    public class Position
    {
        /// <summary>
        /// Indicates the Column of a Piece
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Indicates the Row of a Piece
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        public Position(int col, int row)
        {
            Column = col;
            Row = row;
        }

        public bool IsInBounds()
            => Column >= 1 && Row >= 1 && Column <= 8 && Row <= 8;

        public override string ToString()
        {
            return $"{(char)(Column + 48)}{Row}";
        }

        #region Operator Overloads

        /// <summary>
        /// Static overload for == operator
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool operator ==(Position first, Position second)
        {
            return (first.Column == second.Column) && (first.Row == second.Row);
        }

        /// <summary>
        /// Static overload for != operator
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool operator !=(Position first, Position second)
        {
            return (first.Column != second.Column) || (first.Row != second.Row);
        }

        #endregion
    }
}

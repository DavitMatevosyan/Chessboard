using Chessboard.Logic.Exceptions;
using Chessboard.Models.Positions;

namespace Chessboard.Logic
{
    public static class Helpers
    {
        #region Helpers Related to Position

        /// <summary>
        /// Helper method for converting from string to Type position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal static Position ConvertToPosition(string position)
        {
            if (!int.TryParse(((char)(position[0] - 48)).ToString(), out int col))
                throw new PositionFormatException($"{position} has wrong format");
            if (!int.TryParse(position[1].ToString(), out int row))
                throw new PositionFormatException($"{position} has wrong format");

            return new Position(col, row);
        }

        #endregion
    }
}

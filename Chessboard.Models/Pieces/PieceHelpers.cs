using Chessboard.Models.Positions;
using System.Collections.Generic;
using System.Linq;

namespace Chessboard.Models.Pieces
{
    /// <summary>
    /// Helpers for Pieces
    /// </summary>
    public static class PieceHelpers
    {
        /// <summary>
        /// Determines whether the given piece is in the list or not
        /// </summary>
        /// <param name="pieces"></param>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static bool ContainsPiece(this List<IPiece> pieces, IPiece piece)
        {
            return pieces.Any(x => x.Name == piece.Name);
        }

        public static IPiece PieceAtPosition(this List<IPiece> pieces, Position position)
        {
            return pieces.FirstOrDefault(x => x.Position.Column == position.Column
                                                && x.Position.Row == position.Row);
        }
    }
}

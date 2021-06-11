using Chessboard.Models.Positions;
using System.Collections.Generic;

namespace Chessboard.Models.Pieces
{
    public interface IPiece
    {
        /// <summary>
        /// Indicates the Color of the Piece
        /// </summary>
        public Color Color { get; init; }

        /// <summary>
        /// Indicates the Position of the Piece
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Indicates the name of the Piece i.e. White Queen is "WQ1", Black Pawn is "BP4"
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Shows whether the current Piece is defended or not
        /// </summary>
        public bool IsDefended { get; set; }

        /// <summary>
        /// Shows Each Piece's Available Moves
        /// </summary>
        public List<Move> AvailableMoves { get; set; }

        /// <summary>
        /// Shows which same colored pieces are currently Defended
        /// </summary>
        public List<IPiece> DefendingPieces { get; set; }

        public int MoveCount { get; set; }

        /// <summary>
        /// Calculates the given piece's available moves relative to <paramref name="AllPieces"/>
        /// </summary>
        /// <param name="AllPieces"></param>
        public void CalculateAvailableMoves(List<IPiece> AllPieces);
    }
}

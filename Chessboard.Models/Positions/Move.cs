using System.Diagnostics;

namespace Chessboard.Models.Positions
{
    [DebuggerDisplay("{Name} {Position}")]
    public class Move
    {
        /// <summary>
        /// Indicates which Piece can move to the given position
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates Where can the given Piece move to
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        public Move(string name, Position pos)
        {
            Name = name;
            Position = pos;
        }
    }
}

using Chessboard.Models.Positions;
using System.Collections.Generic;
using System.Diagnostics;

namespace Chessboard.Models.Pieces
{
    [DebuggerDisplay("{Name} {Position}")]
    public class Queen : IPiece, IAttackingPiece
    {
        /// <summary>
        /// Used for generating unique names for the white pieces
        /// </summary>
        private static string _whiteIndexer = "WQ1";

        /// <summary>
        /// Used for generating unique names for the black pieces
        /// </summary>
        private static string _blackIndexer = "BQ1";

        public Color Color { get; init; }

        public Position Position { get; set; }

        public string Name { get; init; }

        public bool IsDefended { get; set; }

        public int MoveCount { get; set; }

        public List<Move> AvailableMoves { get; set; }

        public List<IPiece> DefendingPieces { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="pos"></param>
        public Queen(Color Col, Position pos)
        {
            Color = Col;
            Position = pos;
            MoveCount = 0;
            AvailableMoves = new List<Move>();
            DefendingPieces = new List<IPiece>();
            if (Color == Color.White)
            {
                Name = _whiteIndexer;
                _whiteIndexer = $"{_whiteIndexer.Substring(0, 2)}{int.Parse(_whiteIndexer[2].ToString()) + 1}";
            }
            else
            {
                Name = _blackIndexer;
                _blackIndexer = $"{_blackIndexer.Substring(0, 2)}{int.Parse(_blackIndexer[2].ToString()) + 1}";
            }
        }

        public void CalculateAvailableMoves(List<IPiece> AllPieces)
        {
            DefendingPieces = new List<IPiece>();
            AvailableMoves = new List<Move>();
            this.CalculateMovesHorizontally(AllPieces);
            this.CalculateMovesVertically(AllPieces);
            this.CalculateMovesForwardDiagonally(AllPieces);
            this.CalculateMovesBackwardDiagonally(AllPieces);
        }

        internal static void ResetIndexing()
        {
            _whiteIndexer = "WQ1";
            _blackIndexer = "BQ1";
        }
    }
}

using Chessboard.Models.Positions;
using System.Collections.Generic;
using System.Diagnostics;

namespace Chessboard.Models.Pieces
{
    [DebuggerDisplay("{Name} {Position}")]
    public class King : IPiece
    {
        public Color Color { get; init; }

        public Position Position { get; set; }

        public string Name { get; init; }

        public bool IsDefended { get; set; }

        public List<Move> AvailableMoves { get; set; }

        public List<IPiece> DefendingPieces { get; set; }

        public int MoveCount { get; set; }

        /// <summary>
        /// Shows the neighbourhood of the King
        /// </summary>
        public List<Move> Neighbourhood { get; set; }


        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="pos"></param>
        public King(Color Col, Position pos)
        {
            Color = Col;
            Position = pos;
            MoveCount = 0;
            AvailableMoves = new List<Move>();
            Neighbourhood = new List<Move>();
            DefendingPieces = new List<IPiece>();

            Name = $"{((char)Color).ToString()}K0";
        }

        public void CalculateAvailableMoves(List<IPiece> AllPieces)
        {
            return;
        }
        
        /// <summary>
        /// Calculates the available moves for the king, Relative to <paramref name="UnavailableMoves"/>
        /// </summary>
        /// <param name="AllPieces"></param>
        /// <param name="UnavailableMoves"></param>
        public void CalculateAvailableMoves(List<IPiece> AllPieces, List<Move> UnavailableMoves)
        {
            AvailableMoves = new List<Move>();
            DefendingPieces = new List<IPiece>();
            Neighbourhood = new List<Move>();
            this.CalculateKingMoves(AllPieces, UnavailableMoves);
        }
    }
}

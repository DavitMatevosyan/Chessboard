using Chessboard.Models.Pieces;
using Chessboard.Models.Positions;
using System.Collections.Generic;
using System.Linq;

namespace Chessboard.Models
{
    public static class StandardChessExtensions
    {
        public static string UnderCheck(List<IPiece> pieces, List<Move> whiteUnavailable, List<Move> blackUnavailable)
        {
            string alerts = "";
            var coloredKing = pieces.First(x => x is King && x.Color == Color.White);
            var move = whiteUnavailable.FirstOrDefault(x => x.Position == coloredKing.Position);
            if (move is not null)
                alerts += $"{move.Name}_C";
            coloredKing = pieces.First(x => x is King && x.Color == Color.Black);
            move = blackUnavailable.FirstOrDefault(x => x.Position == coloredKing.Position);
            if (move is not null)
                alerts += $"{move.Name}_C";
            return alerts;
        }

        public static string Mate(List<Move> WhitePieceMoves, List<Move> BlackPieceMoves, Color Turn)
        {
            if (Turn == Color.Black && WhitePieceMoves.Count == 0)
                return "WM";
            if (Turn == Color.White && BlackPieceMoves.Count == 0)
                return "BM";
            return "";
        }


        public static void ResetIndexings()
        {
            Bishop.ResetIndexing();
            Knight.ResetIndexing();
            Pawn.ResetIndexing();
            Queen.ResetIndexing();
            Rook.ResetIndexing();
        }
    }
}

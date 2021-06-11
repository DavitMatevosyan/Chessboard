using System.Collections.Generic;

namespace Chessboard.Models.Positions
{
    public static class PositionHelpers
    {
        public static bool ContainsPosition(this List<Position> positions, Position comparer)
        {
            foreach (var item in positions)
            {
                if (comparer == item)
                    return true;
            }
            return false;
        }

        public static bool ContainsPosition(this List<Move> moves, Position comparer)
        {
            foreach (var item in moves)
            {
                if (comparer == item.Position)
                    return true;
            }
            return false;
        }

        public static bool ContainsColumn(this List<Move> moves, int column)
        {
            foreach (var item in moves)
            {
                if (column == item.Position.Column)
                    return true;
            }
            return false;
        }

        public static bool ContainsRow(this List<Move> moves, int row)
        {
            foreach (var item in moves)
            {
                if (row == item.Position.Row)
                    return true;
            }
            return false;
        }

        public static Position GetPositionGivenColumn(this List<Move> moves, int column)
        {
            foreach (var item in moves)
            {
                if (column == item.Position.Row)
                    return item.Position;
            }
            return null;
        }
    }
}

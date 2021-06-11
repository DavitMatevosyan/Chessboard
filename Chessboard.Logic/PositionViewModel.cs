using Chessboard.Models.Positions;

namespace Chessboard.Logic
{
    public class PositionViewModel
    {
        private Position _position { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }

        public PositionViewModel(int col, int row)
        {
            _position = new Position(col, row);
        }
    }
}

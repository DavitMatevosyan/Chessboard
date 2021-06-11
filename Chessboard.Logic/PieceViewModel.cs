using Chessboard.Models.Pieces;
using Chessboard.Models.Positions;
using Chessboard.Logic.Exceptions;
using System.Diagnostics;

namespace Chessboard.Logic
{
    [DebuggerDisplay("{_piece.Name} {Position}")]
    public class PieceViewModel
    {
        internal IPiece _piece { get; set; }
        public string Type { get; set; }
        public string Position { get; set; }
        public string Color { get; set; }

        public PieceViewModel(string color, string position, string type)
        {
            Color = color;
            Position = position;
            Type = type;
            InitializePiece();
        }

        internal PieceViewModel(IPiece piece)
        {
            _piece = piece;
            Type = GetType(piece);
            Color = "White";
            if (_piece.Color == Models.Pieces.Color.Black)
                Color = "Black";
            Position = ConvertPositionToString(piece.Position);
        }

        public string Name => _piece.Name;

        #region Initializing

        private void InitializePiece()
        {
            Color color = Models.Pieces.Color.White;
            Position pos = ConvertToPosition(Position);

            if (Color == "Black")
                color = Models.Pieces.Color.Black;

            _piece = ConvertToPiece(Type, color, pos);
        }


        #endregion

        #region Converters

        /// <summary>
        /// Converts from given position to Type Position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Position ConvertToPosition(string position)
        {
            return Helpers.ConvertToPosition(position);
        }

        /// <summary>
        /// Converts from given name to a respective Piece
        /// </summary>
        /// <param name="type"></param>
        /// <param name="color"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private IPiece ConvertToPiece(string type, Color color, Position pos)
        {
            return type switch
            {
                "Rook" => new Rook(color, pos),
                "Knight" => new Knight(color, pos),
                "Bishop" => new Bishop(color, pos),
                "Queen" => new Queen(color, pos),
                "King" => new King(color, pos),
                "Pawn" => new Pawn(color, pos),
                _ => throw new TypeFormatException($"{type} is not a valid piece"),
            };
        }

        private string GetType(IPiece piece)
        {
            if (piece is Rook)
                return "Rook";
            if (piece is Knight)
                return "Knight";
            if (piece is Bishop)
                return "Bishop";
            if (piece is Queen)
                return "Queen";
            if (piece is King)
                return "King";
            if (piece is Pawn)
                return "Pawn";
            else
                throw new TypeFormatException($"{piece.Name} is not valid");
        }

        private string ConvertPositionToString(Position position)
        {
            string pos = $"{(char)(position.Column + 48)}{position.Row}";
            return pos;
        }

        #endregion

        internal void AdjustPosition()
        {
            Position = $"{(char)(_piece.Position.Column + 97)}{_piece.Position.Row}";
        }

    }
}

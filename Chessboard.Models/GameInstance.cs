using Chessboard.Data;
using Chessboard.Models.Pieces;
using Chessboard.Models.Positions;
using System.Collections.Generic;
using System.Linq;

namespace Chessboard.Models
{
    public class GameInstance
    {
        public List<IPiece> WhitePieces { get; set; }
        public List<IPiece> BlackPieces { get; set; }
        public List<IPiece> AllPieces { get; set; }
        public List<Move> BlackPieceMoves { get; set; }
        public List<Move> WhitePieceMoves { get; set; }
        public Color Turn { get; set; }
        public string Alerts { get; set; }


        public GameInstance()
        {
            ChessContext context = new ChessContext();
            WhitePieces = new List<IPiece>();
            BlackPieces = new List<IPiece>();
            AllPieces = new List<IPiece>();
            BlackPieceMoves = new List<Move>();
            WhitePieceMoves = new List<Move>();

            Turn = Color.White;
        }

        #region Adding

        /// <summary>
        /// Adds a given <paramref name="piece"/>
        /// </summary>
        /// <param name="piece"></param>
        public string AddPiece(IPiece piece)
        {
            var added = TryAddPiece(piece);
            if (added.Length > 0)
                return added;

            if (piece.Color == Color.White)
            {
                WhitePieces.Add(piece);
                piece.CalculateAvailableMoves(AllPieces);
                if (piece is King king)
                {
                    king.CalculateAvailableMoves(AllPieces, BlackPieceMoves);
                    WhitePieceMoves.AddRange(king.Neighbourhood);
                }
                else
                    WhitePieceMoves.AddRange(piece.AvailableMoves);
            }
            else
            {
                BlackPieces.Add(piece);
                piece.CalculateAvailableMoves(AllPieces);
                if (piece is King king)
                {
                    king.CalculateAvailableMoves(AllPieces, WhitePieceMoves);
                    BlackPieceMoves.AddRange(king.Neighbourhood);
                }
                else
                    BlackPieceMoves.AddRange(piece.AvailableMoves);
            }
            AllPieces.Add(piece);
            return added;
        }


        /// <summary>
        /// Tries to add a piece if failed returns a string with the respective alert
        /// </summary>
        /// <param name="piece"></param>
        /// <returns>If string is empty then adding has been successful</returns>
        private string TryAddPiece(IPiece piece)
        {
            if (piece is King king)
            {
                if (king.Color == Color.White)
                {
                    if (BlackPieceMoves.Any(x => x.Position == piece.Position))
                        return "King is under Check";
                }
                else
                {
                    if (WhitePieceMoves.Any(x => x.Position == piece.Position))
                        return "King is under Check";
                }

            }
            if (AllPieces.Any(x => x.Position == piece.Position))
                return "Colliding Position";
            return "";
        }

        #endregion

        #region Validators 

        /// <summary>
        /// Returns whether the King can make a certain move.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool CanMakeKingMove(Position position)
        {
            var king = AllPieces.First(x => x.Color == Turn && x is King);
            if (king.AvailableMoves.FirstOrDefault(x => x.Position == position) is null)
                return false;
            if (Turn == Color.White)
            {
                return !(BlackPieceMoves.Any(x => x.Position == position));
            }
            return !(WhitePieceMoves.Any(x => x.Position == position));
        }

        /// <summary>
        /// Returns if a piece can make a certain move
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="newPos"></param>
        /// <returns></returns>
        public bool CanMakeAMove(IPiece piece, Position newPos)
        {
            var myPiece = AllPieces.FirstOrDefault(x => x.Color == Turn && x.Name == piece.Name);
            if (myPiece is null)
                return false;
            return myPiece.AvailableMoves.Any(x => x.Position == newPos);
        }

        #endregion

        #region Moving Pieces

        /// <summary>
        /// Makes a move
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="newPosition"></param>
        public IPiece? MakeAMove(IPiece piece, Position newPosition)
        {
            if (!piece.AvailableMoves.ContainsPosition(newPosition))
                return null;
            if (piece is King)
            {
                var move = piece.AvailableMoves.First(x => x.Position == newPosition);
                if(move.Name.Length > 3)
                {
                    var rook = AllPieces.FirstOrDefault(x => x.Name == move.Name[3..]);
                    if (rook is null) return null;
                    rook.Position.Column = rook.Name[2] == '2' ? 6 : 4;
                    piece.Position = newPosition;
                    piece.MoveCount++;
                    Update();
                    ToggleTurn();
                    return rook;
                }
            }
            Position oldPosition = new Position(piece.Position.Column, piece.Position.Row);
            piece.Position = newPosition;
            var pieceToBeEaten = AllPieces.FirstOrDefault(x => x.Position == newPosition && x.Name != piece.Name);
            piece.MoveCount++;
            if (piece is Pawn pawn)
            {
                if (oldPosition.Row == newPosition.Row)
                    newPosition.Row += piece.Color == Color.White ? 1 : -1;
            }
            if (pieceToBeEaten is not null)
                AllPieces.Remove(pieceToBeEaten);
            Update();
            ToggleTurn();
            return pieceToBeEaten;
        }

        #endregion

        #region Methods used at every move

        /// <summary>
        /// Updates all available moves and unavailable moves for both kings
        /// </summary>
        public void Update()
        {
            BlackPieceMoves = new List<Move>();
            WhitePieceMoves = new List<Move>();
            foreach (var item in AllPieces)
            {
                item.AvailableMoves.Clear();
                item.CalculateAvailableMoves(AllPieces);
                if (item.Color == Color.White)
                {
                    if (item is Pawn pawn)
                        WhitePieceMoves.AddRange(pawn.HittingMoves);
                    else
                        WhitePieceMoves.AddRange(item.AvailableMoves);
                }
                else
                {
                    if (item is Pawn pawn)
                        BlackPieceMoves.AddRange(pawn.HittingMoves);
                    else
                        BlackPieceMoves.AddRange(item.AvailableMoves);
                }
            }
            List<IPiece> kings = AllPieces.Where(x => x is King).ToList();
            foreach (var item in kings)
            {
                if(item is King king)
                {
                    if (king.Color == Color.White)
                        king.CalculateAvailableMoves(AllPieces, BlackPieceMoves);
                    else
                        king.CalculateAvailableMoves(AllPieces, WhitePieceMoves);
                }
            }

            MovesCalculatorHelper.ResetDefendingPieces(AllPieces);
            CheckForAlerts();
            MovesCalculatorHelper.FilterKingsAreUnderChain(AllPieces);
        }

        /// <summary>
        /// Checks for any kind of alerts (i.e. Check, Mate, Stalemate)
        /// </summary>
        private void CheckForAlerts()
        {
            Alerts = StandardChessExtensions.UnderCheck(AllPieces, BlackPieceMoves, WhitePieceMoves);
            if (!string.IsNullOrEmpty(Alerts))
            {
           //     Alerts = StandardChessExtensions.Mate(WhitePieceMoves, BlackPieceMoves, Turn);
           //     if (Alerts.Length == 2)
           //         return;
                var piece = AllPieces.FirstOrDefault(x => x.Name == Alerts.Substring(0, 3));
                if (piece is null)
                    return;
                King king = AllPieces.FirstOrDefault(x => (char)x.Color != Alerts[0] && x is King) as King;
                if (king is null)
                    return;
                if (Alerts[0] == 'W')
                    BlackPieceMoves = MovesCalculatorHelper.FilterUnderCheckMoves(piece, king, BlackPieces, BlackPieceMoves);
                else
                    WhitePieceMoves = MovesCalculatorHelper.FilterUnderCheckMoves(piece, king, WhitePieces, WhitePieceMoves);
            }
        }

        /// <summary>
        /// Toggles the turn for the game
        /// </summary>
        /// <param name="turn"></param>
        /// <returns></returns>
        public Color ToggleTurn()
        {
            if (Turn == Color.White)
                Turn = Color.Black;
            else
                Turn = Color.White;
            return Turn;
        }

        #endregion
    }
}

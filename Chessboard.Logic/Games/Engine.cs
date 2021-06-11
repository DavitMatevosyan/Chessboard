using Chessboard.Models;
using Chessboard.Models.Pieces;
using Chessboard.Models.Positions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chessboard.Logic.Games
{
    internal class Engine
    {
        private GameInstance _instance { get; set; }

        /// <summary>
        /// List of all pieces
        /// </summary>
        private List<IPiece> _allPieces { get; set; }

        /// <summary>
        /// Black king
        /// </summary>
        private King _blackKing { get; set; }

        /// <summary>
        /// The character of the last played piece
        /// </summary>
        private string lastPlayedPieceName = "";

        /// <summary>
        /// last played piece
        /// </summary>
        private IPiece _playedPiece;

        /// <summary>
        /// The direction the methods are going to use to attack, 1 means the attacking direction is either upwards or to right
        /// </summary>
        private int direction = 1;

        /// <summary>
        /// if True means the attacking direction is vertical, else the direction is horizontal
        /// </summary>
        private bool isVertical = true;

        private int directionTries = 0;

        public Engine(GameInstance instance)
        {
            _instance = instance;
            _allPieces = instance.AllPieces;
            _blackKing = instance.AllPieces.FirstOrDefault(x => x.Color == Color.Black && x is King) as King;

            DecideDirection();
        }

        /// <summary>
        /// Main method that runs the logic
        /// </summary>
        /// <returns></returns>
        public IPiece Play()
        {
            var underHit = UnderKingHit();
            if (underHit is not null)
                return underHit;
            Position kingPos = new Position(_blackKing.Position.Column, _blackKing.Position.Row);
            if (isVertical)
            {
                if (HavePieceAtCol(kingPos.Column - direction))
                {
                    var attackingPiece = _allPieces.First(x => x is not King && x.AvailableMoves.ContainsColumn(kingPos.Column));
                    _instance.CanMakeAMove(attackingPiece, attackingPiece.AvailableMoves.GetPositionGivenColumn(_blackKing.Position.Column));
                }
            }

            return null;
        }

        public IPiece PlayQueenRook()
        {
            if (isVertical)
            {
                if (!LadderMoveVertical())
                {
                    if (LadderMoveHorizontal())
                    {
                        if (++directionTries == 2)
                        {
                            directionTries = 0;
                            isVertical = !isVertical;
                        }
                    }
                    else
                        MoveWhiteKing();
                    return _playedPiece;
                }
                return _playedPiece;
            }
            else
            {
                if (!LadderMoveHorizontal())
                {
                    if (LadderMoveVertical())
                    {
                        if (++directionTries == 2)
                        {
                            directionTries = 0;
                            isVertical = !isVertical;
                        }
                    }
                    else
                        MoveWhiteKing();
                    return _playedPiece;
                }
                return _playedPiece;

            }
        }

        private void MoveWhiteKing()
        {
            King whiteKing = _allPieces.FirstOrDefault(x => x.Color == Color.White && x is King) as King;
            Position pos;
            if (whiteKing.Position.Row > 3 && whiteKing.Position.Row < 8 || whiteKing.Position.Row == 1)
                pos = whiteKing.AvailableMoves.First(x => x.Position.Row >= whiteKing.Position.Row).Position;
            else
                pos = whiteKing.AvailableMoves.Find(x => x.Position.Row <= whiteKing.Position.Row).Position;
            _instance.MakeAMove(whiteKing, pos);
            _playedPiece = whiteKing;
        }

        /// <summary>
        /// Decides which is the optimal direction to attack
        /// </summary>
        private void DecideDirection()
        {
            foreach (var item in _instance.WhitePieces)
            {
                if (_instance.WhitePieces.Any(x => x.Name != item.Name
                                                            && x.Position.Column == item.Position.Column))
                    isVertical = false;
            }
            if (isVertical)
            {
                if (_blackKing.Position.Row < 5)
                    direction = -1;
            }
            else
            {
                if (_blackKing.Position.Column < 5)
                    direction = -1;
            }
        }

        /// <summary>
        /// Checks whether each piece is under the king's attack or not
        /// </summary>
        /// <returns></returns>
        private IPiece UnderKingHit()
        {
            foreach (var item in _instance.WhitePieces)
            {
                if (_blackKing.Neighbourhood.Any(x => x.Position == item.Position))
                {
                    EscapeKingHit(item);
                    return item;
                }
            }
            return null;
        }

        #region Ladder Move Vertical/Horizontal

        /// <summary>
        /// Attempts vertical ladder move
        /// </summary>
        /// <returns>true if succeeded</returns>
        private bool LadderMoveVertical()
        {
            int row = _blackKing.Position.Row - direction;
            bool isRowBusy = HavePieceAtRow(row);
            foreach (var item in _allPieces)
            {
                if (lastPlayedPieceName != item.Name && item.Position.Row != row)
                    if (CheckAndMoveVertical(item, row, isRowBusy))
                        return true;
            }
            if (CheckRow())
                return true;
            return false;

        }

        /// <summary>
        /// Attemps horizontal ladder move
        /// </summary>
        /// <returns>True if succeeded</returns>
        private bool LadderMoveHorizontal() 
        {
            int col = _blackKing.Position.Column - direction;
            bool colIsBusy = HavePieceAtCol(col);
            foreach (var item in _allPieces)
            {
                if (item.Name != lastPlayedPieceName && item.Position.Column != col)
                    if (CheckAndMoveHorizontal(item, col, colIsBusy))
                        return true;
            }
            if (CheckCol())
                return true;
            return false;

        }

        #endregion

        #region Check Row/Col

        /// <summary>
        /// Checks the black king vertically 
        /// </summary>
        /// <returns></returns>
        private bool CheckRow()
        {
            foreach (var item in _allPieces)
            {
                Position pos = item.AvailableMoves.FirstOrDefault(x => x.Position.Row == _blackKing.Position.Row).Position;
                if (pos is not null)
                {
                    if (!_blackKing.Neighbourhood.ContainsPosition(pos) && lastPlayedPieceName != item.Name)
                    {
                        _instance.MakeAMove(item, pos);
                        lastPlayedPieceName = item.Name;
                        _playedPiece = item;
                        return true;
                    }
                }
            }
            return false;

        }

        /// <summary>
        /// Checks the black king horizontally 
        /// </summary>
        /// <returns></returns>
        private bool CheckCol()
        {
            foreach (var item in _allPieces)
            {
                Position pos = item.AvailableMoves.FirstOrDefault(x => x.Position.Column == _blackKing.Position.Column).Position;
                if (pos is not null)
                {
                    if (!_blackKing.Neighbourhood.ContainsPosition(pos) && lastPlayedPieceName != item.Name)
                    {
                        _instance.MakeAMove(item, pos);
                        lastPlayedPieceName = item.Name;
                        _playedPiece = item;
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Check and Move Horizontal/Vertical

        /// <summary>
        /// Gets one step closer to edge of board
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="row"></param>
        /// <param name="isRowBusy"></param>
        /// <returns></returns>
        private bool CheckAndMoveVertical(IPiece piece, int row, bool isRowBusy)
        {
            if (isRowBusy)
                row += direction;
            var move = piece.AvailableMoves.FirstOrDefault(x => x.Position.Row == row);
            if (move is not null)
            {
                var pos = move.Position;
                if (!_blackKing.Neighbourhood.ContainsPosition(pos) && !HavePieceAtRow(row))
                {
                    _instance.MakeAMove(piece, pos);
                    lastPlayedPieceName = piece.Name;
                    _playedPiece = piece;
                    return true;
                }
                if (_blackKing.Neighbourhood.ContainsPosition(pos))
                {
                    EscapeKingHitHorizontally(piece);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets one step closer to edge of board
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="row"></param>
        /// <param name="isRowBusy"></param>
        /// <returns></returns>
        private bool CheckAndMoveHorizontal(IPiece piece, int col, bool isColBusy)
        {
            if (isColBusy)
                col += direction;

            var move = piece.AvailableMoves.FirstOrDefault(x => x.Position.Column == col);
            if (move is not null)
            {
                var pos = move.Position;
                if (!_blackKing.Neighbourhood.ContainsPosition(pos) && !HavePieceAtCol(col))
                {
                    _instance.MakeAMove(piece, pos);
                    lastPlayedPieceName = piece.Name;
                    _playedPiece = piece;
                    return true;
                }
                if (_blackKing.Neighbourhood.ContainsPosition(pos))
                {
                    EscapeKingHitVertically(piece);
                    return true;
                }
            }
            return false;

        }

        #endregion

        #region Escape King Hit Vertically/Horizontally

        /// <summary>
        /// Escapes King hit Horizontally
        /// </summary>
        /// <param name="piece"></param>
        private void EscapeKingHitHorizontally(IPiece piece)
        {
            if (!piece.AvailableMoves.Where(x => !HavePieceAtCol(x.Position.Column)).Any())
            {
                EscapeKingHitVertically(piece);
                return;
            }
            int col = piece.Position.Column < 4 ? piece.AvailableMoves.Where(x => !HavePieceAtCol(x.Position.Column)).Max(x => x.Position.Column)
                                                          : piece.AvailableMoves.Where(x => !HavePieceAtCol(x.Position.Column)).Min(x => x.Position.Column);
            Position pos = new Position(col, piece.Position.Row);
            _instance.MakeAMove(piece, pos);
            _playedPiece = piece;

        }

        /// <summary>
        /// Escapes King hit Vertically
        /// </summary>
        /// <param name="piece"></param>
        private void EscapeKingHitVertically(IPiece piece)
        {
            if (!piece.AvailableMoves.Where(x => !HavePieceAtRow(x.Position.Row)).Any())
            {
                EscapeKingHitHorizontally(piece);
                return;
            }
            int row = piece.Position.Row < 4 ? piece.AvailableMoves.Where(x => !HavePieceAtRow(x.Position.Row)).Max(x => x.Position.Row)
                                : piece.AvailableMoves.Where(x => !HavePieceAtRow(x.Position.Row)).Max(x => x.Position.Row);
            Position pos = new Position(piece.Position.Column, row);
            _instance.MakeAMove(piece, pos);
            _playedPiece = piece;
        }

        /// <summary>
        /// Experimental Method
        /// </summary>
        /// <param name="piece"></param>
        [Obsolete("Experimental Do not use")]
        private void EscapeKingHit(IPiece piece)
        {
            if (isVertical)
            {
                var possibleMoves = piece.AvailableMoves.Where(x => !HavePieceAtCol(x.Position.Column)).ToList();
                if (possibleMoves.Count > 0)
                {
                    int col = piece.Position.Column < 4 ? possibleMoves.Max(x => x.Position.Column)
                                              : possibleMoves.Min(x => x.Position.Column);
                    Position pos = new Position(col, piece.Position.Row);
                    if (_blackKing.Neighbourhood.ContainsPosition(pos))
                        return;
                    _instance.MakeAMove(piece, pos);
                    _playedPiece = piece;
                }
            }
        }

        #endregion

        #region Have Piece at Col/Row

        /// <summary>
        /// Returns whether you have a piece at that Col
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool HavePieceAtCol(int col)
            => _instance.AllPieces.FirstOrDefault(x => x.Name[1] != 'K' && x.Position.Column == col) is not null;

        /// <summary>
        /// Returns whether you have a piece at that Row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool HavePieceAtRow(int row)
            => _instance.AllPieces.FirstOrDefault(x => x.Name[1] != 'K' && x.Position.Row == row) is not null;

        #endregion



    }
}

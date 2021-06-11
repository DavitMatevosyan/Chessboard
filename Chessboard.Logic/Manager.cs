using Chessboard.Logic.Games;
using Chessboard.Models;
using Chessboard.Models.Pieces;
using Chessboard.Models.Positions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Chessboard.Logic
{
    public class Manager
    {
        private GameInstance _instance { get; set; }
        private PvcMate _pvcMate { get; set; }
        private MoveLog _currentMove { get; set; }

        public MoveLog LastMove { get; set; }
        public List<PieceViewModel> AllPieceViewModels { get; private set; }
        public List<MoveLog> GameHistory { get; set; }
        public int PieceCount { get => _instance.AllPieces.Count; }
        public string Turn { get => _instance.Turn == Color.White ? "White" : "Black"; }
        public string Alerts { get => _instance.Alerts; }


        public Manager()
        {
            _instance = new GameInstance();
            GameHistory = new List<MoveLog>();
            _currentMove = new MoveLog();

            AllPieceViewModels = new List<PieceViewModel>();
            StandardChessExtensions.ResetIndexings();
        }


        #region Adding

        /// <summary>
        /// Using the GameInstance class adds a given <paramref name="piece"/> 
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public string AddPiece(PieceViewModel piece)
        {
            string response = _instance.AddPiece(piece._piece);
            if (response.Length > 0)
                return response;
            AllPieceViewModels.Add(piece);
            return "";
        }

        public string CanAddPiece(PieceViewModel piece)
        {
            string response = _instance.AddPiece(piece._piece);
            if (response.Length > 0)
                return response;
            return "";
        }

        #endregion

        #region Validators 

        /// <summary>
        /// Validates whether the given position is valid or not
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsPositionInBounds(string position)
        {
            return position[1] <= 56 && position[1] >= 49
                && position[0] >= 97 && position[0] <= 104;
        }


        /// <summary>
        /// Checks if the given <paramref name="position"/> is valid
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool CanConvertToPosition(string position)
        {
            if (!int.TryParse(((char)(position[0] - 48)).ToString(), out int col))
                return false;
            if (!int.TryParse(position[1].ToString(), out int row))
                return false;
            return true;
        }

        /// <summary>
        /// Return whether the user can make a certain move
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool CanMakeThisMove(string position)
        {
            Position pos = Helpers.ConvertToPosition(position);
            return _instance.CanMakeKingMove(pos);
        }

        public bool CanMakeAMove(PieceViewModel pieceVM, string newPosition)
        {
            Position newPos = Helpers.ConvertToPosition(newPosition);
            return _instance.CanMakeAMove(pieceVM._piece, newPos);
        }

        #endregion

        #region PVP

        public IEnumerable<PieceViewModel> InitPVPGame()
        {
            for (int i = 1; i <= 8; i++)
                _instance.AddPiece(new Pawn(Color.White, new Position(i, 2)));
            _instance.AddPiece(new Rook(Color.White, new Position(1, 1)));
            _instance.AddPiece(new Knight(Color.White, new Position(2, 1)));
            _instance.AddPiece(new Bishop(Color.White, new Position(3, 1)));
            _instance.AddPiece(new Queen(Color.White, new Position(4, 1)));
            _instance.AddPiece(new King(Color.White, new Position(5, 1)));
            _instance.AddPiece(new Bishop(Color.White, new Position(6, 1)));
            _instance.AddPiece(new Knight(Color.White, new Position(7, 1)));
            _instance.AddPiece(new Rook(Color.White, new Position(8, 1)));

            for (int i = 1; i <= 8; i++)
                _instance.AddPiece(new Pawn(Color.Black, new Position(i, 7)));
            _instance.AddPiece(new Rook(Color.Black, new Position(1, 8)));
            _instance.AddPiece(new Knight(Color.Black, new Position(2, 8)));
            _instance.AddPiece(new Bishop(Color.Black, new Position(3, 8)));
            _instance.AddPiece(new Queen(Color.Black, new Position(4, 8)));
            _instance.AddPiece(new King(Color.Black, new Position(5, 8)));
            _instance.AddPiece(new Bishop(Color.Black, new Position(6, 8)));
            _instance.AddPiece(new Knight(Color.Black, new Position(7, 8)));
            _instance.AddPiece(new Rook(Color.Black, new Position(8, 8)));

            foreach (var item in _instance.AllPieces)
            {
                PieceViewModel pieceVM = new PieceViewModel(item);
                AllPieceViewModels.Add(pieceVM);
                yield return pieceVM;
            }
        }

        #endregion

        #region PVC

        public void StartPvc()
        {
            _pvcMate = new PvcMate(_instance);
            _pvcMate.Start();
        }

        public PieceViewModel PlayWhites()
        {
            var piece = _pvcMate.Play();
            var piecevm = AllPieceViewModels.FirstOrDefault(x => x.Name == piece.Name);
            piecevm._piece.Position = piece.Position;
            piecevm.AdjustPosition();
            if (piecevm is not null)
                return piecevm;
            return null;
        }

        #endregion

        #region Move

        /// <summary>
        /// Making a move
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public PieceViewModel? MakeAMove(PieceViewModel piece, ref string position)
        {
            Position pos = Helpers.ConvertToPosition(position);
            IPiece interactedWith = _instance.MakeAMove(piece._piece, pos);
            LogAMove(piece, position, interactedWith);
            if (interactedWith is null)
                return null;
            position = $"{position[0]}{piece._piece.Position.Row }";
            var interactedVM = AllPieceViewModels.FirstOrDefault(x => x.Name == interactedWith.Name);
            string col = interactedVM._piece.Name[2] == '2' ? "f" : "d";
            interactedVM.Position = $"{col}{interactedWith.Position.Row}";

            return interactedVM;
        }

        private void LogAMove(PieceViewModel piece, string newPosition, IPiece interactedPiece)
        {
            string move = newPosition;
            PieceViewModel interacted;
            if (interactedPiece is not null)
            {
                interacted = new PieceViewModel(interactedPiece);
                if (interacted.Color != piece.Color)
                {
                    interacted = new PieceViewModel(interactedPiece);
                    move += "+";
                }
                else if (interacted.Color == piece.Color && interactedPiece is Rook)
                {
                    if (interactedPiece.Position.Column == 8)
                        move = "O-O";
                    else
                        move = "O-O-O";
                }
            }

            if (string.IsNullOrEmpty(_currentMove.WhiteMove))
                _currentMove.WhiteMove = move;
            else
            {
                _currentMove.BlackMove = move;
                GameHistory.Add(_currentMove);
                _currentMove = new MoveLog();
            }

        }

        #endregion


        #region Getting Moves

        /// <summary>
        /// Gets the possible moves
        /// </summary>
        /// <param name="name"></param>
        /// <returns>JSON text</returns>
        public string GetMoves(string name)
        {
            var piece = _instance.AllPieces.FirstOrDefault(x => x.Name == name);
            if (piece is null)
                return "";
            var listOfMoves = GetMoves(piece);
            string JsonMoves = JsonConvert.SerializeObject(listOfMoves);
            return JsonMoves;
        }

        /// <summary>
        /// Gets all possible moves for the given <paramref name="name"/> Piece
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private IEnumerable<(int col, int row)> GetMoves(IPiece piece)
        {
            var moves = piece.AvailableMoves;
            foreach (var item in moves)
            {
                yield return (item.Position.Column, item.Position.Row);
            }
        }

        #endregion

    }
}

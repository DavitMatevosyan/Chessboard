using Chessboard.Logic;
using System.Collections.Generic;
using System.Linq;

namespace Chessboard.Console
{
    public class Games
    {
        private List<PieceViewModel> _pieces { get; set; }
        private Manager _manager { get; set; }

        private string[,] board { get; set; }

        public Games(List<PieceViewModel> pieces)
        {
            _pieces = pieces;
            _manager = new Manager();
            board = new string[8, 8];
            InitializeBoard();
        }

        #region Games

        public void Play()
        {
            string game = "";
            while (game != "q")
            {
                System.Console.WriteLine("Enter the game you want to play, " +
                        "'3mate' for the 1 king vs 3 pieces mate, " +
                        "'2mate' for the 1 king vs 2 pieces mate, " +
                        "'knight' for the knight movement and 'q' to quit " +
                        "'Pvp'");
                game = System.Console.ReadLine().ToLower();
                switch (game)
                {
                    case "2mate":
                        AddPiece("White", "King");
                        AddPiece("White", "Queen");
                        AddPiece("White", "Rook");


                        DrawBoard();
                        AddPiece("Black", "King");
                        DrawBoard();
                        PlayPvc();
                        break;
                    case "3mate":
                        AddPiece("White", "King");
                        AddPiece("White", "Queen");
                        AddPiece("White", "Rook");
                        AddPiece("White", "Rook");

                        AddPiece("Black", "King");

                        DrawBoard();
                        DrawBoard();
                        PlayPvc();
                        break;
                    case "knight":
                        AddPiece("White", "Knight");
                        DrawBoard();
                        //_manager.PlayKnightMoves();
                        break;
                    case "q":
                        return;
                    default:
                        System.Console.WriteLine("Enter either 'mate', 'twopiecemate', 'knight', 'hw' or 'q' to quit'");
                        break;
                }
            }
        }

        /// <summary>
        /// Plays the Player vs Computer game 
        /// </summary>
        private void PlayPvc()
        {
            _manager.StartPvc();
            string position = GetNewPosition();
            var king =_pieces.First(x => x.Type == "King" && x.Color == "Black");
            _manager.MakeAMove(king, ref position);
            king.Position = position;



            InitializeBoard();
            UpdateBoard();
            DrawBoard();
        }

        #endregion

        #region Playing

        /// <summary>
        /// Gets new position of the black King
        /// </summary>
        /// <returns></returns>
        private string GetNewPosition()
        {
            bool canMove = false;
            string position = "";
            while (!canMove)
            {
                System.Console.Write($"Enter new Position:  ");
                position = System.Console.ReadLine();
                if (!_manager.IsPositionInBounds(position))
                    System.Console.WriteLine("The Given Position is not in bounds");
                else if (!_manager.CanConvertToPosition(position))
                    System.Console.WriteLine("Invalid position format (i.e. 'f7', 'g2', 'c4', etc...)");
                else if (!_manager.CanMakeThisMove(position))
                    System.Console.WriteLine("You can't make such move");
                else
                    canMove = true;
            }
            return position;
        }

        #endregion

        #region Adding Pieces

        /// <summary>
        /// Adds a piece
        /// </summary>
        /// <param name="color"></param>
        /// <param name="type"></param>
        private void AddPiece(string color, string type)
        {
            bool hasAdded = false;
            string position = "";
            PieceViewModel piece;

            while (!hasAdded)
            {
                System.Console.Write($"Enter Position of the {color} {type}:  ");
                position = System.Console.ReadLine();
                if (!_manager.IsPositionInBounds(position))
                    System.Console.WriteLine("The Given Position is not in bounds");
                piece = new PieceViewModel(color, position, type);
                string response = _manager.AddPiece(piece);
                if (response.Length > 0)
                    System.Console.WriteLine("The piece could not be added");
                if (response.Length == 0)
                    hasAdded = true;
            }

            int col = int.Parse(((char)(position[0] - 48)).ToString());
            int row = int.Parse(position[1].ToString());
            board[row - 1, col - 1] = $"{color.ToLower()[0]}{type[0]}";
            _pieces.Add(new PieceViewModel(color, position, type));
        }

        #endregion

        #region Board Related

        /// <summary>
        /// Initializes the board that is going to be printed to the console. 
        /// In other words this is the ui
        /// </summary>
        private void InitializeBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = "  ";
                }
            }
        }

        /// <summary>
        /// Updates the board
        /// </summary>
        private void UpdateBoard()
        {
            foreach (var item in _pieces)
            {
                int row = int.Parse(item.Position[1].ToString());
                int col = int.Parse(((char)(item.Position[0] - 48)).ToString());
                board[row - 1, col - 1] = $"{item.Color[0].ToString().ToLower()}{item.Type[0]}";
            }
        }

        /// <summary>
        /// Draws the board to the console, this is the UI
        /// </summary>
        private void DrawBoard()
        {
            System.Console.Clear();
            System.Console.WriteLine($"  +---+---+---+---+---+---+---+---+ ");
            for (int row = 7; row >= 0; row--)
            {
                for (int col = -1; col <= 7; col++)
                {
                    if (col == -1)
                    {
                        System.Console.Write(row + 1 + " |");
                        continue;
                    }
                    else if (board[row, col] != "  ")
                    {
                        System.Console.Write($"{board[row, col]} |");
                    }
                    else
                        System.Console.Write($"   |");
                }
                System.Console.Write($"\n  +---+---+---+---+---+---+---+---+ ");
                System.Console.WriteLine();
                if (row == 0)
                    System.Console.WriteLine("   a   b   c   d   e   f   g   h");
            }
        }

        #endregion
    }
}

using Chessboard.Logic;
using Chessboard.Logic.Data;
using Chessboard.UI.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chessboard.UI
{
    public partial class MainWindow : Window
    {
        private UserData _playerOneData;
        private UserData _playerTwoData;
        private DataManager _dataManager;

        private List<PieceViewModel> _pieces;
        private Manager _manager;
        private PieceViewModel _pressedPiece;
        private Image _pressedPieceImage;
        private List<PieceVMImage> _pieceImages;
        private string gameType;
        private bool gameStarted = true;
        private int _whiteEatenIndex = 0;
        private int _blackEatenIndex = 0;
        private int _pvcPieceInitIndex = 0;
        private int _numberOfRooks = 0;

        public MainWindow()
        {
            InitializeComponent();
            gameType = "PVP";
            _manager = new Manager();
            _pieceImages = new List<PieceVMImage>();
            InitAllPieces();
        }

        public MainWindow(DataManager manager) : this()
        {
            _dataManager = manager;
            _playerOneData = _dataManager.GetMainUser();
            _playerTwoData = _dataManager.GetSecondaryUser();
            if(!string.IsNullOrEmpty(_playerOneData.IconUri))
                POneIcon.Source = new BitmapImage(new Uri(_playerOneData.IconUri, UriKind.Absolute));
            POneUsername.Content = _playerOneData.Username;
            POneScore.Content = _playerOneData.Score;

        }

        private void btn_LogInPlayerTwoClick(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow(_dataManager);
            login.Show();

        }

        #region Game Mode Changing

        /// <summary>
        /// Triggered when pressed "Player vs Player" button in the game type menu
        /// resets the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PvpGameModeSelected(object sender, RoutedEventArgs e)
        {
            gameType = "PVP";
            _whiteEatenIndex = 0;
            _blackEatenIndex = 0;
            _pieces = new List<PieceViewModel>();
            _manager = new Manager();
            _pressedPiece = null;
            _pressedPieceImage = null;
            _pieceImages = new List<PieceVMImage>();
            boardGrid.Children.Clear();
            backBoardgrid.Children.Clear();
            InitAllPieces();
            boardImage.MouseDown -= boardImage_Click;
            PVCborder.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Triggered when pressed "Queen Rook Mate" button in the game type menu
        /// resets the board and adds those pieces
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PvcGameModeSelected(object sender, RoutedEventArgs e)
        {
            boardImage.MouseDown -= boardImage_Click;
            gameType = "PVC";
            gameStarted = false;
            _pieces = new List<PieceViewModel>();
            _manager = new Manager();
            _pressedPiece = null;
            _pressedPieceImage = null;
            _pieceImages = new List<PieceVMImage>();
            _pvcPieceInitIndex = 0;
            PVCborder.Visibility = Visibility.Visible;
            boardGrid.Children.Clear();
            backBoardgrid.Children.Clear();
            boardImage.MouseDown += boardImage_Click;
            if (sender is Button btn)
            {
                if (btn.Name[^1] == '1')
                    _numberOfRooks = 1;
                else
                    _numberOfRooks = 2;
            }
            InitPvcPieces();
        }

        #endregion

        #region Initial Run

        /// <summary>
        /// Adds All pieces to the board
        /// </summary>
        private void InitAllPieces()
        {
            _pieces = _manager.InitPVPGame().ToList();
            foreach (var item in _pieces)
            {
                _pieceImages.Add(new PieceVMImage(GetImage(item), item));
            }
        }

        private void InitPvcPieces()
        {
            _pieces = new List<PieceViewModel>();
            _pvcPieceInitIndex = 0;
            int index = 0;
            AddImage(@"\Images\Standard\whiteKing.png", index++);
            AddImage(@"\Images\Standard\whiteQueen.png", index++);
            AddImage(@"\Images\Standard\whiteRook.png", index++);
            if (_numberOfRooks == 2)
                AddImage(@"\Images\Standard\whiteRook.png", index++);
            AddImage(@"\Images\Standard\blackKing.png", index++);
        }

        private void boardImage_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img)
            {
                Point mousePoint = e.GetPosition(img);
                int col = (int)(mousePoint.X / 45) - 1;
                int row = (int)(mousePoint.Y / 45) - 1;
                GetPVCImage(col, row, _pvcPieceInitIndex++);
            }
        }


        #endregion

        #region Making a Move

        /// <summary>
        /// First Click on the piece, Chooses the piece to move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PieceImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RemoveCanMoveToLabels();

            if (gameType == "PVC")
            {
                if (!gameStarted)
                    return;
                if (sender is Image pieceImage)
                    PressedPiece(pieceImage);
            }
            if (gameType == "PVP")
            {
                if (sender is Image pieceImage)
                {
                    PressedPiece(pieceImage);
                }
            }
        }

        private void PressedPiece(Image pieceImage)
        {
            _pressedPieceImage = pieceImage;
            var pieceVM = _pieces.First(x => x.Name == pieceImage.Name);
            if (pieceVM.Color != _manager.Turn)
            {
                MessageBox.Show("Not Your Turn!");
                return;
            }
            _pressedPiece = pieceVM;
            var moves = _manager.GetMoves(pieceVM).ToList();
            foreach (var item in moves)
            {
                Label canMoveTo = new Label
                {
                    Background = new SolidColorBrush(Color.FromArgb(60, 221, 221, 50))
                };
                canMoveTo.MouseLeftButtonDown += PieceMoveToPosition_LabelClick;
                boardGrid.Children.Add(canMoveTo);
                Grid.SetColumn(canMoveTo, item.col - 1);
                Grid.SetRow(canMoveTo, 8 - item.row);
            }
        }

        private void PieceMoveToPosition_LabelClick(object sender, MouseButtonEventArgs e)
        {
            int row = 0;
            int col = 0;
            if (sender is Label moveTo)
            {
                row = 8 - Grid.GetRow(moveTo);
                col = Grid.GetColumn(moveTo);
            }
            string pos = $"{(char)(col + 97)}{row}";
            if (!_manager.CanMakeAMove(_pressedPiece, pos))
            {
                MessageBox.Show("Unavailable Position");
                RemoveCanMoveToLabels();
                return;
            }
            var interactedWith = _manager.MakeAMove(_pressedPiece, ref pos);

            if (interactedWith is not null)
            {
                if (interactedWith.Color != _pressedPiece.Color)
                    RemovePieceImage(interactedWith);
                else
                {
                    Image pieceImage = null;
                    foreach (var item in boardGrid.Children)
                    {
                        if (item is Image img)
                        {
                            if (img.Name == interactedWith.Name)
                                pieceImage = img;
                        }
                    }
                    UpdateBoard(interactedWith.Position, pieceImage);
                }
            }
            if (gameType == "PVC")
            {
                var playedPiece = _manager.PlayWhites();
                foreach (var item in boardGrid.Children)
                {
                    if (item is Image pieceImg)
                    {
                        if (pieceImg.Name == playedPiece.Name)
                        {
                            Grid.SetColumn(pieceImg, (int)(playedPiece.Position[0] - 97));
                            Grid.SetRow(pieceImg, 8 - int.Parse(playedPiece.Position[1].ToString()));
                        }
                    }
                }
            }
            UpdateBoard(pos);
            //   ListBox_GameHistory.Items.Add(_manager.LastMove);
            if (!string.IsNullOrEmpty(_manager.Alerts))
                MessageBox.Show(_manager.Alerts);

        }

        #endregion

        #region Board Related

        /// <summary>
        /// Removes all labels from the board grid, meaning removes all yellow points
        /// </summary>
        private void RemoveCanMoveToLabels()
        {
            List<Label> listOfLabels = new List<Label>();
            foreach (var item in boardGrid.Children)
            {
                if (item is Label label)
                {
                    listOfLabels.Add(label);
                }
            }
            foreach (var item in listOfLabels)
            {
                boardGrid.Children.Remove(item);
            }
        }

        private void UpdateBoard(string newPos)
        {
            int row = 8 - int.Parse(newPos[1].ToString());
            int col = int.Parse(((char)(newPos[0] - 48)).ToString()) - 1;
            Grid.SetRow(_pressedPieceImage, row);
            Grid.SetColumn(_pressedPieceImage, col);
            RemoveCanMoveToLabels();
        }

        private void UpdateBoard(string newPos, Image pieceImage)
        {
            int row = 8 - int.Parse(newPos[1].ToString());
            int col = int.Parse(((char)(newPos[0] - 48)).ToString()) - 1;
            Grid.SetRow(pieceImage, row);
            Grid.SetColumn(pieceImage, col);
            RemoveCanMoveToLabels();
        }

        /// <summary>
        /// Adds a piece to the local storage
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="color"></param>
        /// <param name="type"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private PieceViewModel AddImage(string uri, string color, string type, int col, int row)
        {
            char newCol = Convert.ToChar(col + 97);
            PieceViewModel pieceVM = new PieceViewModel(color, $"{newCol}{8 - row}", type);

            _manager.AddPiece(pieceVM);

            var pieceImage = new Image()
            {
                Source = new BitmapImage(new Uri(uri, UriKind.Relative)),
                Width = 40,
                Name = pieceVM.Name
            };
            pieceImage.MouseDown += PieceImage_MouseDown;
            boardGrid.Children.Add(pieceImage);
            Grid.SetColumn(pieceImage, col);
            Grid.SetRow(pieceImage, row);
            _pieceImages.Add(new PieceVMImage(pieceImage, pieceVM));
            return pieceVM;
        }

        private void AddImage(string uri, int pieceIndex)
        {
            var pieceImage = new Image()
            {
                Source = new BitmapImage(new Uri(uri, UriKind.Relative)),
                Width = 40,
                Height = 40,
                Margin = new Thickness(10 + (20 * pieceIndex), 400, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            pieceImage.MouseDown += PieceImage_MouseDown;
            backBoardgrid.Children.Add(pieceImage);
        }

        private void RemovePieceImage(PieceViewModel piece)
        {
            var removed = _pieceImages.FirstOrDefault(x => x.PieceVM.Name == piece.Name);
            if (removed is null)
                return;
            boardGrid.Children.Remove(removed.Image);
            if (removed.PieceVM.Color == "White")
                removed.Image.Margin = new Thickness(10 + (20 * _blackEatenIndex++), 0, 0, 0);
            else
                removed.Image.Margin = new Thickness(10 + (20 * _whiteEatenIndex++), 400, 0, 0);
            removed.Image.HorizontalAlignment = HorizontalAlignment.Left;
            removed.Image.VerticalAlignment = VerticalAlignment.Top;
            backBoardgrid.Children.Add(removed.Image);
        }

        private void GetPVCImage(int col, int row, int index)
        {
            PieceViewModel addedPiece = null;
            switch (index)
            {
                case 0:
                    addedPiece = AddImage(@"\Images\Standard\whiteKing.png", "White", "King", col, row);
                    _pieces.Add(addedPiece);
                    break;
                case 1:
                    addedPiece = AddImage(@"\Images\Standard\whiteQueen.png", "White", "Queen", col, row);
                    _pieces.Add(addedPiece);
                    break;
                case 2:
                    addedPiece = AddImage(@"\Images\Standard\whiteRook.png", "White", "Rook", col, row);
                    _pieces.Add(addedPiece);
                    break;
                case 3:
                    if (_numberOfRooks == 2)
                    {
                        addedPiece = AddImage(@"\Images\Standard\whiteRook.png", "White", "Rook", col, row);
                        _pieces.Add(addedPiece);
                    }
                    else
                    {
                        addedPiece = AddImage(@"\Images\Standard\blackKing.png", "Black", "King", col, row);
                        _pieces.Add(addedPiece);
                        boardImage.MouseDown -= boardImage_Click;
                        gameStarted = true;
                        _manager.StartPvc();
                    }
                    break;
                case 4:
                    if (_numberOfRooks == 2)
                    {
                        addedPiece = AddImage(@"\Images\Standard\blackKing.png", "Black", "King", col, row);
                        _pieces.Add(addedPiece);
                        boardImage.MouseDown -= boardImage_Click;
                        gameStarted = true;
                        _manager.StartPvc();
                    }
                    break;
                default:
                    break;
            }
            if (backBoardgrid.Children.Count > 0)
                backBoardgrid.Children.RemoveAt(0);

        }

        /// <summary>
        /// Returns the correct image for the given <paramref name="piecevm"/> piece view model
        /// </summary>
        /// <param name="piecevm"></param>
        /// <returns></returns>
        private Image GetImage(PieceViewModel piecevm)
        {
            char newCol = Convert.ToChar(piecevm.Position[0]);

            var pieceImage = new Image()
            {
                Width = 40,
                Name = piecevm.Name
            };
            pieceImage.MouseDown += PieceImage_MouseDown;
            if (piecevm.Color == "White")
            {
                switch (piecevm.Name[1])
                {
                    case 'P':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\whitePawn.png", UriKind.Relative));
                        break;
                    case 'R':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\whiteRook.png", UriKind.Relative));
                        break;
                    case 'N':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\whiteKnight.png", UriKind.Relative));
                        break;
                    case 'B':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\whiteBishop.png", UriKind.Relative));
                        break;
                    case 'Q':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\whiteQueen.png", UriKind.Relative));
                        break;
                    case 'K':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\whiteKing.png", UriKind.Relative));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (piecevm.Name[1])
                {
                    case 'P':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\blackPawn.png", UriKind.Relative));
                        break;
                    case 'R':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\blackRook.png", UriKind.Relative));
                        break;
                    case 'N':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\blackKnight.png", UriKind.Relative));
                        break;
                    case 'B':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\blackBishop.png", UriKind.Relative));
                        break;
                    case 'Q':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\blackQueen.png", UriKind.Relative));
                        break;
                    case 'K':
                        pieceImage.Source = new BitmapImage(new Uri(@"\Images\Standard\blackKing.png", UriKind.Relative));
                        break;
                    default:
                        break;
                }
            }

            boardGrid.Children.Add(pieceImage);
            Grid.SetColumn(pieceImage, int.Parse(piecevm.Position[0].ToString()) - 1);
            Grid.SetRow(pieceImage, 8 - int.Parse(piecevm.Position[1].ToString()));

            return pieceImage;

        }

        #endregion
    }
}

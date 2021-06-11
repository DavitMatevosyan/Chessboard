using Chessboard.Logic;
using System.Windows.Controls;

namespace Chessboard.UI
{
    internal class PieceVMImage
    {
        public Image Image { get; set; }
        public PieceViewModel PieceVM { get; set; }

        public PieceVMImage(Image image, PieceViewModel piecevm)
        {
            Image = image;
            PieceVM = piecevm;
        }
    }
}

using Chessboard.Logic;
using System.Collections.Generic;

namespace Chessboard.Console
{
    class View
    {
        /// <summary>
        /// List of all pieces
        /// </summary>
        private List<PieceViewModel> _pieces { get; set; }

        /// <summary>
        /// Types of games
        /// </summary>
        public Games Games { get; set; }

        public View()
        {
            _pieces = new List<PieceViewModel>();
            Games = new Games(_pieces);
        }
    }
}

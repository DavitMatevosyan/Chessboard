using System.Linq;
using Chessboard.Models;
using Chessboard.Models.Pieces;
using Chessboard.Models.Positions;

namespace Chessboard.Logic.Games
{
    public class PvcMate : IGame
    {
        private GameInstance _instance { get; set; }
        private Engine engine { get; set; }

        public PvcMate(GameInstance instance)
        {
            engine = new Engine(instance);
            _instance = instance;
        }

        /// <summary>
        /// initial method for starting the game
        /// </summary>
        public void Start()
        {
            _instance.Update();
            _instance.ToggleTurn();
        }

        /// <summary>
        /// Make a move using black king
        /// </summary>
        /// <param name="newPosition"></param>
        public void MakeAMove(Position newPosition)
        {
            var king = _instance.BlackPieces.First(x => x is King);
            _instance.MakeAMove(king, newPosition);
        }

        public IPiece Play()
        {
            return engine.PlayQueenRook();
        }

    }
}

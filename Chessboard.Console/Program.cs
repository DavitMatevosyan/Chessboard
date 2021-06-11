using Chessboard.Logic;

namespace Chessboard.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            View view = new View();
            view.Games.Play();
        }
    }
}

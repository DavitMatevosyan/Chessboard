namespace Chessboard.Data
{
    public interface IGameContext
    {
        int AddGame();
        int GetGame(int id);
        int GetAllGames();
        int GetAllGames(int userId);
        int UpdateGame(int id);
        int DeleteGame(int id);
    }
}

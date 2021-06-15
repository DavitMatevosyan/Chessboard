namespace Chessboard.Data
{
    public interface IChessContext
    {
        #region CRUD User

        int AddUser(User user);
        int GetUser(int id);
        int GetAllUsers();
        int UpdateUser(int id);
        int DeleteUser(int id);

        #endregion

        #region CRUD Game

        int AddGame(Game game);
        int GetGame(int id);
        int GetAllGames();
        int GetAllGames(int userId);
        int UpdateGame(int id);
        int DeleteGame(int id);

        #endregion
    }
}

namespace Chessboard.Data
{
    public interface IUserContext
    {
        int AddUser();
        int GetUser(int id);
        int GetAllUsers();
        int UpdateUser(int id);
        int DeleteUser(int id);
    }
}

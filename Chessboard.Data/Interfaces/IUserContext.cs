using System.Collections.Generic;

namespace Chessboard.Data
{
    public interface IUserContext
    {
        /// <summary>
        /// Adds a new user to the Production Database
        /// </summary>
        /// <param name="user">The user that will be added</param>
        /// <returns>1 if succeeded, -1 if not</returns>
        int AddUser(User newUser);
        User GetUser(int id);
        User GetUser(string username, string passwordHash);
        List<User> GetAllUsers();
        int UpdateUser(int id);
        int DeleteUser(int id);
    }
}

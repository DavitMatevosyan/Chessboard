using Chessboard.Data;

namespace Chessboard.Logic.Data
{
    public class DataManager
    {
        IChessContext _context;

        private User _currentUser;

        public DataManager(IChessContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new user to the _context database
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <param name="passwordHash">password hash</param>
        /// <param name="email">email of the user can be empty</param>
        /// <param name="iconuri">icon uri</param>
        /// <returns>1 if succeeded, -1 if not</returns>
        public int AddUser(string username, string passwordHash, string email = "", string iconuri = "")
        {
            var user = new User()
            {
                Username = username,
                PasswordHash = passwordHash,
                Email = email,
                IconUri = iconuri
            };
            return _context.AddUser(user);
        }

        public int LogIn(string username, string passwordHash)
        {
            _currentUser = _context.GetUser(username, passwordHash);

            return 0;
        }

        #region Get User Data

        /// <summary>
        /// Gets current user's username
        /// </summary>
        /// <returns></returns>
        public string GetUsername() => _currentUser.Username;

        /// <summary>
        /// Gets current user's email
        /// </summary>
        /// <returns></returns>
        public string GetEmail() => _currentUser.Email;

        /// <summary>
        /// Gets current user's Icon uri
        /// </summary>
        /// <returns></returns>
        public string GetIcon() => _currentUser.IconUri;

        #endregion
    }
}

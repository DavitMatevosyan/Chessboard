using Chessboard.Data;

namespace Chessboard.Logic.Data
{
    public class DataManager
    {
        IChessContext _context;

        private User _mainUser;
        private User _secondaryUser;

        public DataManager(IChessContext context)
        {
            _context = context;
            _mainUser = new User();
            _secondaryUser = new User();
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

        /// <summary>
        /// Tries to log in with the given Data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwordHash"></param>
        /// <returns>1 is succeeded, -1 if not</returns>
        public int LogIn(string username, string passwordHash)
        {
            _mainUser = _context.GetUser(username, passwordHash);
            if (_mainUser.Username is null)
                return -1;
            return 1;
        }

        /// <summary>
        /// Tries to log in as the second user with the given data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwordHash"></param>
        /// <returns>1 is succeeded, -1 if not</returns>
        public int LogInPlayerTwo(string username, string passwordHash)
        {
            _secondaryUser = _context.GetUser(username, passwordHash);
            if (_mainUser.Username is null)
                return -1;
            return 1;

        }

        #region Get User Data

        public UserData GetMainUser()
        {
            return new UserData()
            {
                Id = _mainUser.Id,
                Username = _mainUser.Username,
                Email = _mainUser.Email,
                IconUri = _mainUser.IconUri,
                Score = _mainUser.Score
            };
        }

        public UserData GetSecondaryUser()
        {
            return new UserData()
            {
                Id = _secondaryUser.Id,
                Username = _secondaryUser.Username,
                Email = _secondaryUser.Email,
                Score = _secondaryUser.Score,
                IconUri = _secondaryUser.IconUri
            };
        }

        /// <summary>
        /// Gets current user's username
        /// </summary>
        /// <returns></returns>
        public string GetUsername() => _mainUser.Username;

        /// <summary>
        /// Gets current user's email
        /// </summary>
        /// <returns></returns>
        public string GetEmail() => _mainUser.Email;

        /// <summary>
        /// Gets current user's Icon uri
        /// </summary>
        /// <returns></returns>
        public string GetIcon() => _mainUser.IconUri;

        /// <summary>
        /// Gets current user's Score
        /// </summary>
        /// <returns></returns>
        public int GetScore() => _mainUser.Score;

        #endregion
    }
}

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Chessboard.Data
{
    /// <summary>
    /// The Database context for the production environment, which uses real database objects
    /// </summary>
    public class ChessContext : IChessContext
    {
        /// <summary>
        /// Sql client provider
        /// </summary>
        DbProviderFactory factory;
        /// <summary>
        /// My connection string
        /// </summary>
        string _connectionString;

        /// <summary>
        /// Default constructor to initialize the DbProviderFactory and connectionString
        /// </summary>
        public ChessContext()
        {
            factory = SqlClientFactory.Instance;
            ConnectionJson connection = new ConnectionJson();
            _connectionString = connection.GetConnectionString();
        }

        #region Add Game/User

        public int AddGame(Game game)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new user to the Production Database
        /// </summary>
        /// <param name="user">The user that will be added</param>
        /// <returns>1 if succeeded, -1 if not</returns>
        public int AddUser(User user)
        {
            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                DbCommand command = new SqlCommand();
                command.CommandType = CommandType.StoredProcedure; // default is Text
                command.CommandText = GetAddUserCommandText(user);
                command.Connection = conn;

                try
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the sql command for adding a new user based on the given <paramref name="user"/>
        /// </summary>
        /// <param name="user">the user that will be added</param>
        /// <returns>the sql command</returns>
        private string GetAddUserCommandText(User user)
        {
            string command = $"Exec Users.AddUser @username = {user.Username}, @passwordHash = {user.PasswordHash}";
            if (!string.IsNullOrEmpty(user.Email))
                command += $", @Email = {user.Email}";
            if (!string.IsNullOrEmpty(user.IconUri))
                command += $", @IconUri = {user.IconUri}";

            return command;
        }

        #endregion

        #region Delete

        public int DeleteGame(int id)
        {
            throw new NotImplementedException();
        }

        public int DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Get 

        public int GetAllGames()
        {
            throw new NotImplementedException();
        }

        public int GetAllGames(int userId)
        {
            throw new NotImplementedException();
        }

        public int GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public int GetGame(int id)
        {
            throw new NotImplementedException();
        }

        public int GetUser(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Update

        public int UpdateGame(int id)
        {
            throw new NotImplementedException();
        }

        public int UpdateUser(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        [Obsolete("Just for an example", error: true)]
        public void Read()
        {
            using (DbConnection conn = factory.CreateConnection())
            {
                var a = conn.GetType().Name;
                conn.ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Chess;Trusted_Connection=True;";

                conn.Open();
                DbCommand dbCommand = factory.CreateCommand();
                dbCommand.Connection = conn;
                dbCommand.CommandText = "SELECT * FROM USERS";
                using (DbDataReader reader = dbCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ab = $"-> Car #{reader["Id"]} is a {reader["Username"]}.";
                    }
                }
            }
        }

    }

}

using System;
using System.Collections.Generic;
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

        SqlConnection _connection;

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

        public int AddUser(User user)
        {
            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                DbCommand command = new SqlCommand();
                ConfigureCommand(command, conn, user);

                try
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                    return 1;
                }
                catch (Exception ex)
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
        private void ConfigureCommand(DbCommand command, DbConnection connection, User user)
        {
            command.CommandType = CommandType.StoredProcedure; // default is Text
            command.CommandText = "Users.AddUser";
            command.Connection = connection;

            SqlParameter usernameParam = new SqlParameter("@Username", user.Username);
            SqlParameter emailParam = new SqlParameter("@Email", user.Email);
            SqlParameter passParam = new SqlParameter("@PasswordHash", user.PasswordHash);
            SqlParameter iconUriParam = new SqlParameter("@IconUri", user.IconUri);

            command.Parameters.Add(usernameParam);
            command.Parameters.Add(emailParam);
            command.Parameters.Add(passParam);
            command.Parameters.Add(iconUriParam);
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

        public User GetUser(string username, string passwordHash)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.ConnectionString = _connectionString;
                string query = $"SELECT * FROM Users.LogInUser('{username}', '{passwordHash}')";
                SqlCommand command = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    var reader = command.ExecuteReader();

                    User user = new User();
                    if (reader.Read())
                    {
                        user.Id = int.Parse(reader["Id"].ToString());
                        user.Username = reader["Username"].ToString();
                        user.Email = reader["Email"].ToString();
                        user.IconUri = reader["IconUri"].ToString();
                    }

                    conn.Close();
                    return user;
                }
                catch (Exception)
                {
                    return null;
                }


            }
        }

        public int GetAllGames()
        {
            throw new NotImplementedException();
        }

        public int GetAllGames(int userId)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public int GetGame(int id)
        {
            throw new NotImplementedException();
        }

        public User GetUser(int id)
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

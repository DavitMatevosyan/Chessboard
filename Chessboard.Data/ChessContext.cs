using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Chessboard.Data
{
    public class ChessContext : IChessContext
    {
        DbProviderFactory factory;
        string _connectionString;

        public ChessContext()
        {
            factory = SqlClientFactory.Instance;
            ConnectionJson connection = new ConnectionJson();
            _connectionString = connection.GetConnectionString();
        }

        #region Add Game/User

        public int AddGame()
        {
            throw new NotImplementedException();
        }

        public int AddUser()
        {
            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                DbCommand command = new SqlCommand();
                command.CommandType = CommandType.StoredProcedure; // default is Text
                command.CommandText = "";
                command.Connection = conn;

                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
            return 1;
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


        private void Setup()
        {
            IDbConnection dbConnection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=Chess;Trusted_Connection=True;");            
        }
    }

}

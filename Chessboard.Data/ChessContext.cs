using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Chessboard.Data
{
    public class ChessContext
    {
        DbProviderFactory factory;

        public ChessContext()
        {
            factory = SqlClientFactory.Instance;
            
            
        }

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

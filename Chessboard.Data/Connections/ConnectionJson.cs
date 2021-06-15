using System.IO;
using System.Text.Json;

namespace Chessboard.Data
{
    internal class ConnectionJson
    {
        class JsonType
        {
            public string ChessConnection { get; set; }
        }

        private string _connectionString;

        public ConnectionJson()
        {
            var filename = "Connections/ConnectionSettings.json";
            var jsonString = File.ReadAllText(filename);
            var connStr = JsonSerializer.Deserialize<JsonType>(jsonString);
            _connectionString = connStr.ChessConnection;
        }

        public string GetConnectionString() => _connectionString;
    }
}

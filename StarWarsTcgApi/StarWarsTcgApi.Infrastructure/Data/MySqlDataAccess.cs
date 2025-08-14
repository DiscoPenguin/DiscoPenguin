using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace StarWarsTcgApi.Infrastructure.Data
{
    public class MySqlDataAccess
    {
        private readonly string _connectionString;
        public MySqlDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration), "Default connection string is not configured");
        }
        public IDbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
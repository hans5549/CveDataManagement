using System.Data;
using WebConsole.Interfaces;
using MySql.Data.MySqlClient;

namespace WebConsole.Services.Database;

public class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public string GetLastInsertIdCommand()
    {
        return "SELECT LAST_INSERT_ID();";
    }
}
using System.Data;
using WebConsole.Interfaces;
using Microsoft.Data.SqlClient;

namespace WebConsole.Services.Database;

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public string GetLastInsertIdCommand()
    {
        return "SELECT SCOPE_IDENTITY();";
    }
}
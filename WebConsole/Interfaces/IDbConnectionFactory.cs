using System.Data;

namespace WebConsole.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    string GetLastInsertIdCommand();
}
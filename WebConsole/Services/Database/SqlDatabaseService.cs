using WebConsole.Interfaces;
using WebConsole.Models;

namespace WebConsole.Services.Database;

public class SqlDatabaseService : IDatabaseService
{
    private readonly IDbConnectionFactory _connectionFactory; // 新增自己的 connectionFactory
    private readonly CveDataInserter _cveDataInserter;
    private readonly CnaDataInserter _cnaDataInserter;
    private readonly AdpDataInserter _adpDataInserter;

    public SqlDatabaseService(string connectionString, string dbType)
    {
        _connectionFactory = dbType.ToLower() switch
        {
            "sqlserver" => new SqlServerConnectionFactory(connectionString),
            "mysql" => new MySqlConnectionFactory(connectionString),
            _ => throw new ArgumentException("Unsupported database type")
        };

        _cveDataInserter = new CveDataInserter(_connectionFactory);
        _cnaDataInserter = new CnaDataInserter(_connectionFactory);
        _adpDataInserter = new AdpDataInserter(_connectionFactory);
    }

    public void InsertCveData(Cve.RootCve cveData)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var cveMetadataId = _cveDataInserter.InsertCveMetadata(cveData.CveMetadata, connection, transaction);
            var rootCveId = _cveDataInserter.InsertRootCve(cveData, cveMetadataId, connection, transaction);

            if (cveData.Containers == null) return;
            var containersId = _cveDataInserter.InsertContainers(rootCveId, connection, transaction);

            if (cveData.Containers.Cna != null)
            {
                var cnaId = _cnaDataInserter.InsertCnaContainer(cveData.Containers.Cna, connection, transaction);
                _cveDataInserter.UpdateContainersCnaId(containersId, cnaId, connection, transaction);
                _cnaDataInserter.InsertCnaRelatedData(cveData.Containers.Cna, cnaId, connection, transaction);
            }

            if (cveData.Containers.Adp is { Count: > 0 })
                foreach (var adp in cveData.Containers.Adp)
                    _adpDataInserter.InsertAdpContainer(adp, containersId, connection, transaction);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new Exception("Failed to insert CVE data", ex);
        }
    }
}
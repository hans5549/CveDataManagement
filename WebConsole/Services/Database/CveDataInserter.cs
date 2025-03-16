using System.Data;
using WebConsole.Interfaces;
using WebConsole.Models;

namespace WebConsole.Services.Database;

public class CveDataInserter
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CveDataInserter(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public int InsertCveMetadata(Cve.CveMetadata metadata, IDbConnection connection, IDbTransaction transaction)
    {
        if (metadata == null) return -1;

        var sql = $@"
            INSERT INTO CveMetadata (CveId, AssignerOrgId, AssignerShortName, State, DateReserved, DatePublished, DateUpdated)
            VALUES (@CveId, @AssignerOrgId, @AssignerShortName, @State, @DateReserved, @DatePublished, @DateUpdated);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@CveId", (object)metadata.CveId ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@AssignerOrgId",
            (object)metadata.AssignerOrgId ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@AssignerShortName",
            (object)metadata.AssignerShortName ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@State", (object)metadata.State ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@DateReserved",
            (object)metadata.DateReserved ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@DatePublished",
            (object)metadata.DatePublished ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@DateUpdated", (object)metadata.DateUpdated ?? DBNull.Value));

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int InsertRootCve(Cve.RootCve cveData, int cveMetadataId, IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = $@"
            INSERT INTO RootCve (DataType, DataVersion, CveMetadataId)
            VALUES (@DataType, @DataVersion, @CveMetadataId);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@DataType", (object)cveData.DataType ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@DataVersion", (object)cveData.DataVersion ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@CveMetadataId", cveMetadataId));

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int InsertContainers(int rootCveId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = $@"
            INSERT INTO Containers (RootCveId)
            VALUES (@RootCveId);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@RootCveId", rootCveId));
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void UpdateContainersCnaId(int containersId, int cnaId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = "UPDATE Containers SET CnaId = @CnaId WHERE ContainersId = @ContainersId;";
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@CnaId", cnaId));
        command.Parameters.Add(CreateParameter(command, "@ContainersId", containersId));
        command.ExecuteNonQuery();
    }

    private IDbDataParameter CreateParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        return parameter;
    }
}
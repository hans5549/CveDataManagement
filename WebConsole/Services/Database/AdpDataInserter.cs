using WebConsole.Interfaces;
using WebConsole.Models;
using System.Data;

namespace WebConsole.Services.Database;

public class AdpDataInserter
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AdpDataInserter(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void InsertAdpContainer(Cve.AdpContainer adp, int containersId, IDbConnection connection,
        IDbTransaction transaction)
    {
        var providerMetadataId = InsertProviderMetadata(adp.ProviderMetadata, connection, transaction);

        var sql = $@"
            INSERT INTO AdpContainer (ContainersId, Title, ProviderMetadataId)
            VALUES (@ContainersId, @Title, @ProviderMetadataId);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@ContainersId", containersId));
        command.Parameters.Add(CreateParameter(command, "@Title", (object)adp.Title ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@ProviderMetadataId", providerMetadataId));
        var adpId = Convert.ToInt32(command.ExecuteScalar());

        if (adp.Metrics != null)
            foreach (var metric in adp.Metrics)
                InsertAdpMetric(metric, adpId, connection, transaction);
    }

    private int InsertProviderMetadata(Cve.ProviderMetadata metadata, IDbConnection connection,
        IDbTransaction transaction)
    {
        if (metadata == null) return -1;

        var sql = $@"
            INSERT INTO ProviderMetadata (OrgId, ShortName, DateUpdated)
            VALUES (@OrgId, @ShortName, @DateUpdated);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@OrgId", (object)metadata.OrgId ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@ShortName", (object)metadata.ShortName ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@DateUpdated", (object)metadata.DateUpdated ?? DBNull.Value));
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private void InsertAdpMetric(Cve.AdpMetric metric, int adpId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = $@"
            INSERT INTO AdpMetric (AdpId)
            VALUES (@AdpId);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@AdpId", adpId));
        var adpMetricId = Convert.ToInt32(command.ExecuteScalar());

        if (metric.Other != null) InsertSsvc(metric.Other, adpMetricId, connection, transaction);
    }

    private void InsertSsvc(Cve.Ssvc ssvc, int adpMetricId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = $@"
            INSERT INTO Ssvc (AdpMetricId, Type)
            VALUES (@AdpMetricId, @Type);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@AdpMetricId", adpMetricId));
        command.Parameters.Add(CreateParameter(command, "@Type", (object)ssvc.Type ?? DBNull.Value));
        var ssvcId = Convert.ToInt32(command.ExecuteScalar());

        if (ssvc.Content != null) InsertSsvcContent(ssvc.Content, ssvcId, connection, transaction);
    }

    private void InsertSsvcContent(Cve.SsvcContent content, int ssvcId, IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = $@"
            INSERT INTO SsvcContent (SsvcId, Id, Timestamp, Role, Version)
            VALUES (@SsvcId, @Id, @Timestamp, @Role, @Version);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@SsvcId", ssvcId));
        command.Parameters.Add(CreateParameter(command, "@Id", (object)content.Id ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Timestamp", content.Timestamp));
        command.Parameters.Add(CreateParameter(command, "@Role", (object)content.Role ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Version", (object)content.Version ?? DBNull.Value));
        var ssvcContentId = Convert.ToInt32(command.ExecuteScalar());

        if (content.Options != null)
            foreach (var option in content.Options)
                InsertSsvcOption(option, ssvcContentId, connection, transaction);
    }

    private void InsertSsvcOption(Cve.SsvcOption option, int ssvcContentId, IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO SsvcOption (SsvcContentId, Exploitation, Automatable, TechnicalImpact)
            VALUES (@SsvcContentId, @Exploitation, @Automatable, @TechnicalImpact);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@SsvcContentId", ssvcContentId));
        command.Parameters.Add(CreateParameter(command, "@Exploitation", (object)option.Exploitation ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Automatable", (object)option.Automatable ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@TechnicalImpact",
            (object)option.TechnicalImpact ?? DBNull.Value));
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
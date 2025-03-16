using WebConsole.Interfaces;
using WebConsole.Models;
using System.Data;

namespace WebConsole.Services.Database;

public class CnaDataInserter
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CnaDataInserter(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public int InsertCnaContainer(Cve.CnaContainer cna, IDbConnection connection, IDbTransaction transaction)
    {
        var providerMetadataId = InsertProviderMetadata(cna.ProviderMetadata, connection, transaction);

        var sql = $@"
            INSERT INTO CnaContainer (ProviderMetadataId, Title)
            VALUES (@ProviderMetadataId, @Title);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@ProviderMetadataId", providerMetadataId));
        command.Parameters.Add(CreateParameter(command, "@Title", (object)cna.Title ?? DBNull.Value));
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void InsertCnaRelatedData(Cve.CnaContainer cna, int cnaId, IDbConnection connection,
        IDbTransaction transaction)
    {
        if (cna.Affected != null)
            foreach (var affected in cna.Affected)
                InsertAffected(affected, cnaId, connection, transaction);

        if (cna.Descriptions != null)
            foreach (var desc in cna.Descriptions)
                InsertDescription(desc, cnaId, connection, transaction);

        if (cna.Metrics != null)
            foreach (var metric in cna.Metrics)
                InsertMetric(metric, cnaId, connection, transaction);

        if (cna.Timeline != null)
            foreach (var timeline in cna.Timeline)
                InsertTimelineEntry(timeline, cnaId, connection, transaction);

        if (cna.Credits != null)
            foreach (var credit in cna.Credits)
                InsertCredit(credit, cnaId, connection, transaction);

        if (cna.References != null)
            foreach (var reference in cna.References)
                InsertReference(reference, cnaId, connection, transaction);
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

    private void InsertAffected(Cve.Affected affected, int cnaId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = $@"
            INSERT INTO Affected (CnaId, Vendor, Product)
            VALUES (@CnaId, @Vendor, @Product);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@CnaId", cnaId));
        command.Parameters.Add(CreateParameter(command, "@Vendor", (object)affected.Vendor ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Product", (object)affected.Product ?? DBNull.Value));
        var affectedId = Convert.ToInt32(command.ExecuteScalar());

        if (affected.Versions != null)
            foreach (var version in affected.Versions)
                InsertVersion(version, affectedId, connection, transaction);

        if (affected.Modules != null)
            foreach (var module in affected.Modules)
                InsertModule(module, affectedId, connection, transaction);
    }

    private void InsertVersion(Cve.Version version, int affectedId, IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO Versions (AffectedId, VersionValue, Status, LessThanOrEqual, VersionType)
            VALUES (@AffectedId, @VersionValue, @Status, @LessThanOrEqual, @VersionType);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@AffectedId", affectedId));
        command.Parameters.Add(CreateParameter(command, "@VersionValue", (object)version.VersionValue ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Status", (object)version.Status ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@LessThanOrEqual",
            (object)version.LessThanOrEqual ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@VersionType", (object)version.VersionType ?? DBNull.Value));
        command.ExecuteNonQuery();
    }

    private void InsertModule(string moduleName, int affectedId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO Modules (AffectedId, ModuleName)
            VALUES (@AffectedId, @ModuleName);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@AffectedId", affectedId));
        command.Parameters.Add(CreateParameter(command, "@ModuleName", (object)moduleName ?? DBNull.Value));
        command.ExecuteNonQuery();
    }

    private void InsertDescription(Cve.Description desc, int cnaId, IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO Description (CveId, Language, DescriptionText)
            VALUES (@CveId, @Language, @DescriptionText);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@CveId", (object)desc.CveId ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Language", (object)desc.Language ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@DescriptionText",
            (object)desc.DescriptionText ?? DBNull.Value));
        command.ExecuteNonQuery();
    }

    private void InsertMetric(Cve.Metric metric, int cnaId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = $@"
            INSERT INTO Metric (CnaId)
            VALUES (@CnaId);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@CnaId", cnaId));
        var metricId = Convert.ToInt32(command.ExecuteScalar());

        if (metric.CvssV4_0 != null) InsertCvssV4_0(metric.CvssV4_0, metricId, connection, transaction);
        if (metric.CvssV3_1 != null) InsertCvssV3_1(metric.CvssV3_1, metricId, connection, transaction);
        if (metric.CvssV3_0 != null) InsertCvssV3_0(metric.CvssV3_0, metricId, connection, transaction);
        if (metric.CvssV2_0 != null) InsertCvssV2_0(metric.CvssV2_0, metricId, connection, transaction);
    }

    private void InsertCvssV4_0(Cve.CvssV4_0 cvss, int metricId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO CvssV4_0 (MetricId, Version, BaseScore, VectorString, BaseSeverity)
            VALUES (@MetricId, @Version, @BaseScore, @VectorString, @BaseSeverity);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@MetricId", metricId));
        command.Parameters.Add(CreateParameter(command, "@Version", (object)cvss.Version ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@BaseScore", cvss.BaseScore));
        command.Parameters.Add(CreateParameter(command, "@VectorString", (object)cvss.VectorString ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@BaseSeverity", (object)cvss.BaseSeverity ?? DBNull.Value));
        command.ExecuteNonQuery();
    }

    private void InsertCvssV3_1(Cve.CvssV3_1 cvss, int metricId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO CvssV3_1 (MetricId, Version, BaseScore, VectorString, BaseSeverity)
            VALUES (@MetricId, @Version, @BaseScore, @VectorString, @BaseSeverity);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@MetricId", metricId));
        command.Parameters.Add(CreateParameter(command, "@Version", (object)cvss.Version ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@BaseScore", cvss.BaseScore));
        command.Parameters.Add(CreateParameter(command, "@VectorString", (object)cvss.VectorString ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@BaseSeverity", (object)cvss.BaseSeverity ?? DBNull.Value));
        command.ExecuteNonQuery();
    }

    private void InsertCvssV3_0(Cve.CvssV3_0 cvss, int metricId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO CvssV3_0 (MetricId, Version, BaseScore, VectorString, BaseSeverity)
            VALUES (@MetricId, @Version, @BaseScore, @VectorString, @BaseSeverity);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@MetricId", metricId));
        command.Parameters.Add(CreateParameter(command, "@Version", (object)cvss.Version ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@BaseScore", cvss.BaseScore));
        command.Parameters.Add(CreateParameter(command, "@VectorString", (object)cvss.VectorString ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@BaseSeverity", (object)cvss.BaseSeverity ?? DBNull.Value));
        command.ExecuteNonQuery();
    }

    private void InsertCvssV2_0(Cve.CvssV2_0 cvss, int metricId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO CvssV2_0 (MetricId, Version, BaseScore, VectorString)
            VALUES (@MetricId, @Version, @BaseScore, @VectorString);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@MetricId", metricId));
        command.Parameters.Add(CreateParameter(command, "@Version", (object)cvss.Version ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@BaseScore", cvss.BaseScore));
        command.Parameters.Add(CreateParameter(command, "@VectorString", (object)cvss.VectorString ?? DBNull.Value));
        command.ExecuteNonQuery();
    }

    private void InsertTimelineEntry(Cve.TimelineEntry timeline, int cnaId, IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO TimelineEntry (CnaId, CveId, Time, Language, Value)
            VALUES (@CnaId, @CveId, @Time, @Language, @Value);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@CnaId", cnaId));
        command.Parameters.Add(CreateParameter(command, "@CveId", (object)timeline.CveId ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Time", timeline.Time));
        command.Parameters.Add(CreateParameter(command, "@Language", (object)timeline.Language ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Value", (object)timeline.Value ?? DBNull.Value));
        command.ExecuteNonQuery();
    }

    private void InsertCredit(Cve.Credit credit, int cnaId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO Credit (CnaId, CveId, Language, Type, Value)
            VALUES (@CnaId, @CveId, @Language, @Type, @Value);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@CnaId", cnaId));
        command.Parameters.Add(CreateParameter(command, "@CveId", (object)credit.CveId ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Language", (object)credit.Language ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Type", (object)credit.Type ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Value", (object)credit.Value ?? DBNull.Value));
        command.ExecuteNonQuery();
    }

    private void InsertReference(Cve.Reference reference, int cnaId, IDbConnection connection,
        IDbTransaction transaction)
    {
        var sql = $@"
            INSERT INTO Reference (CnaId, CveId, Url, Name)
            VALUES (@CnaId, @CveId, @Url, @Name);
            {_connectionFactory.GetLastInsertIdCommand()}";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@CnaId", cnaId));
        command.Parameters.Add(CreateParameter(command, "@CveId", (object)reference.CveId ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Url", (object)reference.Url ?? DBNull.Value));
        command.Parameters.Add(CreateParameter(command, "@Name", (object)reference.Name ?? DBNull.Value));
        var referenceId = Convert.ToInt32(command.ExecuteScalar());

        if (reference.Tags != null)
            foreach (var tag in reference.Tags)
                InsertReferenceTag(tag, referenceId, connection, transaction);
    }

    private void InsertReferenceTag(string tag, int referenceId, IDbConnection connection, IDbTransaction transaction)
    {
        var sql = @"
            INSERT INTO ReferenceTags (ReferenceId, Tag)
            VALUES (@ReferenceId, @Tag);";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.Parameters.Add(CreateParameter(command, "@ReferenceId", referenceId));
        command.Parameters.Add(CreateParameter(command, "@Tag", (object)tag ?? DBNull.Value));
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
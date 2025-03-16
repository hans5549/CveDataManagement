using WebConsole.Interfaces;
using WebConsole.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebConsole.Services;

public class CveMapperService : ICveMapper
{
    public MappingResult MapCveToModel(string filePath, string failureDirectory)
    {
        try
        {
            if (!Directory.Exists(failureDirectory)) Directory.CreateDirectory(failureDirectory);

            using var streamReader = new StreamReader(filePath);
            using var jsonReader = new JsonTextReader(streamReader);
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new AdpMetricConverter());
            var rootCve = serializer.Deserialize<Cve.RootCve>(jsonReader);

            if (rootCve?.CveMetadata == null)
                throw new Exception("Failed to parse JSON data or missing critical CVE metadata.");

            // 處理 metrics，過濾並驗證
            ProcessMetrics(rootCve);

            return new MappingResult
            {
                IsSuccess = true,
                OriginalFilePath = filePath,
                RootCve = rootCve
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error mapping {filePath}: {ex.Message}");
            MoveFileToFailureDirectory(filePath, failureDirectory);
            return new MappingResult
            {
                IsSuccess = false,
                OriginalFilePath = filePath,
                RootCve = null
            };
        }
    }

    private void ProcessMetrics(Cve.RootCve rootCve)
    {
        if (rootCve?.Containers?.Adp == null) return;

        foreach (var adp in rootCve.Containers.Adp)
        {
            if (adp.Metrics == null) continue;

            foreach (var metric in adp.Metrics.Where(m => m.Other != null))
                if (metric.Other.Type == "ssvc")
                    metric.Other.Content.Timestamp = ValidateSqlDateTime(metric.Other.Content.Timestamp);
        }
    }

    private DateTime ValidateSqlDateTime(DateTime date)
    {
        var minSqlDateTime = new DateTime(1753, 1, 1);
        var maxSqlDateTime = new DateTime(9999, 12, 31, 23, 59, 59);

        if (date < minSqlDateTime)
        {
            Console.WriteLine($"Date '{date}' before 1753, adjusting to min: {minSqlDateTime}");
            return minSqlDateTime;
        }

        if (date > maxSqlDateTime)
        {
            Console.WriteLine($"Date '{date}' after 9999, adjusting to max: {maxSqlDateTime}");
            return maxSqlDateTime;
        }

        return date;
    }

    private static void MoveFileToFailureDirectory(string sourcePath, string failureDirectory)
    {
        try
        {
            if (!Directory.Exists(failureDirectory)) Directory.CreateDirectory(failureDirectory);
            var fileName = Path.GetFileName(sourcePath);
            var destPath = Path.Combine(failureDirectory, fileName);
            File.Move(sourcePath, destPath);
        }
        catch
        {
            // Silent failure
        }
    }
}

// 自訂轉換器，避免遞迴問題
public class AdpMetricConverter : JsonConverter<Cve.AdpMetric>
{
    public override Cve.AdpMetric ReadJson(JsonReader reader, Type objectType, Cve.AdpMetric existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var other = jsonObject["other"];
        if (other == null) return new Cve.AdpMetric { Other = null };

        var type = other["type"]?.ToString();
        if (type != "ssvc")
            // 如果不是 ssvc（例如 kev），返回空物件
            return new Cve.AdpMetric { Other = null };

        // 手動構建 Ssvc 物件
        var ssvc = new Cve.Ssvc
        {
            Type = type,
            Content = new Cve.SsvcContent
            {
                Id = other["content"]?["id"]?.ToString(),
                Timestamp = other["content"]?["timestamp"] != null
                    ? DateTime.Parse(other["content"]["timestamp"].ToString())
                    : DateTime.MinValue,
                Role = other["content"]?["role"]?.ToString(),
                Version = other["content"]?["version"]?.ToString(),
                Options = other["content"]?["options"]?.Select(opt => new Cve.SsvcOption
                {
                    Exploitation = opt["Exploitation"]?.ToString(),
                    Automatable = opt["Automatable"]?.ToString(),
                    TechnicalImpact = opt["Technical Impact"]?.ToString()
                }).ToList()
            }
        };

        return new Cve.AdpMetric { Other = ssvc };
    }

    public override void WriteJson(JsonWriter writer, Cve.AdpMetric value, JsonSerializer serializer)
    {
        throw new NotImplementedException("Serialization not needed for this use case.");
    }
}
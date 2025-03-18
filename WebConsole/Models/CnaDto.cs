namespace WebConsole.Models;

/// <summary>
/// 用於映射CNA查詢結果的DTO
/// </summary>
public class CnaDto
{
    public int CnaId { get; set; }
    public string? Title { get; set; }
    public int? ProviderMetadataId { get; set; }
    public string? OrgId { get; set; }
    public string? ShortName { get; set; }
    public DateTime? DateUpdated { get; set; }
} 
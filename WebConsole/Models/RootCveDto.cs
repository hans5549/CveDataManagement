namespace WebConsole.Models;

/// <summary>
/// 用於映射 RootCve 查詢結果的 DTO
/// </summary>
public class RootCveDto
{
    public int RootCveId { get; set; }
    public string? DataType { get; set; }
    public string? DataVersion { get; set; }
} 
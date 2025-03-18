namespace WebConsole.Models;

/// <summary>
/// 包含ID的受影響產品DTO
/// </summary>
public class AffectedWithIdDto
{
    public int AffectedId { get; set; }
    public string? Vendor { get; set; }
    public string? Product { get; set; }
} 
namespace WebConsole.Models;

public class CveMetadataDto
{
    public int CveMetadataId { get; set; }
    public string? CveId { get; set; }
    public string? AssignerOrgId { get; set; }
    public string? AssignerShortName { get; set; }
    public string? State { get; set; }
    public DateTime? DateReserved { get; set; }
    public DateTime? DatePublished { get; set; }
    public DateTime? DateUpdated { get; set; }
} 
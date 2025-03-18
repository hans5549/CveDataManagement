namespace WebConsole.Models;

public class MetricDto
{
    public string? CvssV4_0_Version { get; set; } = string.Empty;
    public float? CvssV4_0_BaseScore { get; set; }
    public string? CvssV4_0_VectorString { get; set; } =  string.Empty;
    public string? CvssV4_0_BaseSeverity { get; set; }  = string.Empty;
    public string? CvssV3_1_Version { get; set; }  = string.Empty;
    public float? CvssV3_1_BaseScore { get; set; }
    public string? CvssV3_1_VectorString { get; set; } = string.Empty;
    public string? CvssV3_1_BaseSeverity { get; set; } = string.Empty;
}
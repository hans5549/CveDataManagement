using System;

namespace WebConsole.Models
{
    // 用於查詢CVSS評分的DTO
    public class MetricsDto
    {
        public int CveCvssScoreId { get; set; }
        public string CveId { get; set; }
        public string Format { get; set; }
        public string Version { get; set; }
        public double BaseScore { get; set; }
        public string BaseSeverity { get; set; }
        public string VectorString { get; set; }
    }
} 
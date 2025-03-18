using System;
using System.Collections.Generic;

namespace WebConsole.Models
{
    // 用於查詢問題類型的DTO
    public class ProblemTypeDto
    {
        public int ProblemTypeId { get; set; }
        public int CnaId { get; set; }
    }

    // 用於查詢時間線的DTO
    public class TimelineDto
    {
        public DateTime Time { get; set; }
        public string Language { get; set; }
        public string Value { get; set; }
    }
} 
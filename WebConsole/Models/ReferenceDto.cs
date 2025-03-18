using System.Collections.Generic;

namespace WebConsole.Models;

// 用於查詢參考資料的DTO
public class ReferenceDto
{
    public int ReferenceId { get; set; }
    public string CveId { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
}
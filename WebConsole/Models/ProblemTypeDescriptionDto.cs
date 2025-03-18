namespace WebConsole.Models;

// 用於查詢問題類型描述的DTO
public class ProblemTypeDescriptionDto
{
    public string CweId { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }
    public string Type { get; set; }
}
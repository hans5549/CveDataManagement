using WebConsole.Models;

namespace WebConsole.Interfaces;

public interface ICveMapper
{
    MappingResult MapCveToModel(string filePath, string failureDirectory);
}
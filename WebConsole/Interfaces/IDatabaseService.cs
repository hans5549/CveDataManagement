using WebConsole.Models;

namespace WebConsole.Interfaces;

public interface IDatabaseService
{
    void InsertCveData(Cve.RootCve cveData);
}
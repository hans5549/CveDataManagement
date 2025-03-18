using WebConsole.Models;

namespace WebConsole.Interfaces;

public interface IDatabaseService
{
    void InsertCveData(Cve.RootCve cveData);
    
    /// <summary>
    /// 根據 CVE ID 取得完整的 CVE 資料
    /// </summary>
    /// <param name="cveId">CVE 識別碼，例如 CVE-2023-1234</param>
    /// <returns>完整的 CVE 資料模型</returns>
    Cve.RootCve GetCveDataByCveId(string cveId);
}
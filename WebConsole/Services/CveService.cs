using WebConsole.Models;
using WebConsole.Repository;

namespace WebConsole.Services;

public class CveService
{
    private readonly CveRepository _cveRepository;

    public CveService(CveRepository cveRepository)
    {
        _cveRepository = cveRepository;
    }

    #region Vendor and Product Methods
    public List<string> GetAllVendorList()
    {
        return _cveRepository.SelectAllVendors();
    }

    public List<string> GetVendorByLetter(string letter)
    {
        return _cveRepository.SelectVendorByLetter(letter);
    }

    public List<string> GetProductByVendor(string vendor)
    {
        return _cveRepository.SelectProductByVendor(vendor);
    }

    public List<string> GetCveIdByVendorAndProduct(string vendor, string product)
    {
        return _cveRepository.SelectCveIdByVendorAndProduct(vendor, product);
    }

    public List<CveProduct> GetAllProductWithCveId()
    {
        return _cveRepository.SelectAllProductWithCveId();
    }
    #endregion
    
    /// <summary>
    /// 根據CVE ID獲取完整的CVE資訊
    /// </summary>
    /// <param name="cveId">CVE識別碼，例如CVE-2023-1234</param>
    /// <returns>完整的CVE資料模型</returns>
    public Cve.RootCve GetCveDataById(string cveId)
    {
        try
        {
            if (string.IsNullOrEmpty(cveId))
            {
                return null;
            }
            
            // 標準化CVE ID格式
            cveId = NormalizeCveId(cveId);
            
            return _cveRepository.SelectCveDataByCveId(cveId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    
    /// <summary>
    /// 標準化CVE ID格式
    /// </summary>
    private string NormalizeCveId(string cveId)
    {
        // 將輸入的CVE ID格式化為標準格式
        if (!cveId.StartsWith("CVE-", StringComparison.OrdinalIgnoreCase))
        {
            if (cveId.StartsWith("CVE", StringComparison.OrdinalIgnoreCase))
            {
                cveId = "CVE-" + cveId.Substring(3);
            }
            else
            {
                cveId = "CVE-" + cveId;
            }
        }
        
        return cveId.ToUpperInvariant();
    }
}
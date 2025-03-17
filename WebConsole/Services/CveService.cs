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
}
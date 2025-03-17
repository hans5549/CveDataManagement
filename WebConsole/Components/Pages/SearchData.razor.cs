using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using WebConsole.Models;
using WebConsole.Services;

namespace WebConsole.Components.Pages;

public partial class SearchData : ComponentBase
{
    [Inject] private CveService CveService { get; set; } = null!;

    private string _product = string.Empty;
    private string _vendor = string.Empty;
    private string _vendorLetter = string.Empty;
    private char[] _alphabetArray = new char[26];
    private List<string> _cveId = [];
    private List<string> _products = [];
    private List<string> _vendors = [];
    private Dictionary<string, List<string>> _letterProductMap = [];
    private List<CveProduct> _productWithCveIds = [];

    protected override void OnInitialized()
    {
        for (var i = 0; i < 26; i++)
        {
            var letter = (char)('A' + i);
            _alphabetArray[i] = letter;
            _letterProductMap.TryAdd(letter.ToString(), []);
        }

        base.OnInitialized();
    }

    private void InitializeTree()
    {
        // _productWithCveIds = CveService.GetAllProductWithCveId();
        // var groupedProducts = _productWithCveIds
        //     .Select(x => x.ProductName!)
        //     .Distinct()
        //     .Where(p => !string.IsNullOrEmpty(p) && char.IsLetter(p[0]))
        //     .GroupBy(p => p[0].ToString().ToUpper());
        // foreach (var group in groupedProducts)
        // {
        //     if (_letterProductMap.TryGetValue(group.Key, out var value))
        //     {
        //         value.AddRange(group);
        //     }
        // }
    }

    private void OnVendorLetterChanged(string? letter)
    {
        _vendorLetter = letter ?? string.Empty;
        if (string.IsNullOrEmpty(_vendorLetter))
        {
            _vendor =  string.Empty;
            _vendors = [];
            _product = string.Empty;
            _products = [];
            _cveId = [];
        }
        else
        {
            _vendor = string.Empty;
            _vendors = CveService.GetVendorByLetter(_vendorLetter);
            _product = string.Empty;
            _products = [];
            _cveId = [];
        }
    }

    private void OnVendorsChanged(string? vendor)
    {
        _vendor = vendor ?? string.Empty;
        if (string.IsNullOrEmpty(_vendor))
        {
            _product = string.Empty;
            _products = [];
            _cveId = [];
        }
        else
        {
            _product = string.Empty;
            _products = CveService.GetProductByVendor(_vendor);
            _cveId = [];
        }
    }

    private void OnProductChanged(string? product)
    {
        _product = product ?? string.Empty;
        if (string.IsNullOrEmpty(product))
        {
            _cveId = [];
        }
        else
        {
            _cveId = CveService.GetCveIdByVendorAndProduct(_vendor, _product);
        }
    }

    private void HandledTreeItemClick(string cveId)
    {

    }
}
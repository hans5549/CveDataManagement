using Microsoft.AspNetCore.Components;
using WebConsole.Models;
using WebConsole.Services;

namespace WebConsole.Components.Pages;

public partial class SearchData : ComponentBase
{
    [Inject] private CveService CveService { get; set; } = null!;

    private string _selectedCveId = string.Empty;
    private string _product = string.Empty;
    private string _vendor = string.Empty;
    private string _vendorLetter = string.Empty;
    private readonly char[] _alphabetArray = Enumerable.Range(0, 26).Select(i => (char)('A' + i)).ToArray();
    private List<string> _cveId = [];
    private List<string> _products = [];
    private List<string> _vendors = [];
    private Dictionary<string, List<string>> _letterProductMap = [];
    private List<CveProduct> _productWithCveIds = [];
    private Cve.RootCve? _selectedCveData; // 新增變數來儲存選定的 CVE 資訊

    protected override void OnInitialized()
    {
        InitializeLetterProductMap();
        base.OnInitialized();
    }

    private void InitializeLetterProductMap()
    {
        foreach (var letter in _alphabetArray)
        {
            _letterProductMap.TryAdd(letter.ToString(), []);
        }
    }

    private void OnVendorLetterChanged(string? letter)
    {
        _vendorLetter = letter ?? string.Empty;

        if (string.IsNullOrEmpty(_vendorLetter))
        {
            ResetVendorSelection();
        }
        else
        {
            _vendor = string.Empty;
            _vendors = CveService.GetVendorByLetter(_vendorLetter);
            ResetProductSelection();
        }
    }

    private void OnVendorsChanged(string? vendor)
    {
        _vendor = vendor ?? string.Empty;

        if (string.IsNullOrEmpty(_vendor))
        {
            ResetProductSelection();
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

        _cveId = string.IsNullOrEmpty(_product) ? [] : CveService.GetCveIdByVendorAndProduct(_vendor, _product);
    }

    private void OnSelectedCveIdChanged(string? cveId)
    {
        _selectedCveId = cveId ?? string.Empty;
        _selectedCveData = CveService.GetCveDataById(_selectedCveId);
        StateHasChanged(); // 通知 Blazor 更新 UI
    }

    private void ResetVendorSelection()
    {
        _vendor = string.Empty;
        _vendors = [];
        ResetProductSelection();
    }

    private void ResetProductSelection()
    {
        _product = string.Empty;
        _products = [];
        _cveId = [];
    }
}
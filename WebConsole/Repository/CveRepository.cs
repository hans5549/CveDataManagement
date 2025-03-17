using Microsoft.EntityFrameworkCore;
using WebConsole.Data;
using WebConsole.Models;
using MySqlConnector;

namespace WebConsole.Repository;

public class CveRepository
{
    private readonly CveDbContext _context;

    public CveRepository(CveDbContext context)
    {
        _context = context;
    }

    public List<string> SelectAllVendors()
    {
        try
        {
            var sql = $@"SELECT DISTINCT Vendor FROM Affected ORDER BY Vendor";
            var result = _context.Database.SqlQueryRaw<string>(sql).ToList();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    public List<string> SelectVendorByLetter(string letter)
    {
        try
        {
            var sql = $@"SELECT DISTINCT Vendor FROM Affected WHERE Vendor LIKE CONCAT(@letter, '%') ORDER BY Vendor;";
            object[] parameters = [ new MySqlParameter("@letter", letter) ];
            var result = _context.Database.SqlQueryRaw<string>(sql, parameters).ToList();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    public List<string> SelectProductByVendor(string vendor)
    {
        try
        {
            var sql = $@"SELECT DISTINCT Product FROM Affected WHERE Vendor = @vendor ORDER BY Product;";
            object[] parameters = [ new MySqlParameter("@vendor", vendor) ];
            var result = _context.Database.SqlQueryRaw<string>(sql, parameters).ToList();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    public List<string> SelectCveIdByVendorAndProduct(string vendor, string product)
    {
        try
        {
            var sql = $@"
                        SELECT DISTINCT cm.CveId
                        FROM Affected a
                  INNER JOIN CnaContainer cc ON a.CnaId = cc.CnaId
                  INNER JOIN Containers c ON cc.CnaId = c.CnaId
                  INNER JOIN RootCve rc ON c.RootCveId = rc.RootCveId
                  INNER JOIN CveMetadata cm ON rc.CveMetadataId = cm.CveMetadataId
                       WHERE a.Vendor = @vendor
                         AND a.Product = @product
                    ORDER BY cm.CveId;";
            object[] parameters =
                [
                    new MySqlParameter("@vendor", vendor),
                    new MySqlParameter("@product", product)
                ];
            var result = _context.Database.SqlQueryRaw<string>(sql, parameters).ToList();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    public List<CveProduct> SelectAllProductWithCveId()
    {
        try
        {
            var sql = $@"
                SELECT a.Product AS ProductName,
                       cm.CveId AS CveId
                  FROM Affected a
                  JOIN CnaContainer cc ON a.CnaId = cc.CnaId
                  JOIN Containers c ON cc.CnaId = c.CnaId
                  JOIN RootCve rc ON c.RootCveId = rc.RootCveId
                  JOIN CveMetadata cm ON rc.CveMetadataId = cm.CveMetadataId
              ORDER BY cm.CveId;";
            var result = _context.Database.SqlQueryRaw<CveProduct>(sql).ToList();

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }
}
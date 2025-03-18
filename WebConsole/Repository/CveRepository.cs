using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using WebConsole.Data;
using WebConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebConsole.Repository;

public class CveRepository
{
    private readonly CveDbContext _context; // 假設您的 DbContext 名稱為 YourDbContext

    public CveRepository(CveDbContext context)
    {
        _context = context;
    }

    public List<string> SelectAllVendors()
    {
        try
        {
            var sql = "SELECT DISTINCT Vendor FROM Affected WHERE Vendor IS NOT NULL";
            return _context.Database.SqlQueryRaw<string>(sql).ToList();
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
            var sql = "SELECT DISTINCT Vendor FROM Affected WHERE Vendor LIKE @Letter AND Vendor IS NOT NULL";
            object[] parameters =
            [
                new MySqlParameter("@Letter", letter + "%")
            ];
            return _context.Database.SqlQueryRaw<string>(sql, parameters).ToList();
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
            var sql = "SELECT DISTINCT Product FROM Affected WHERE Vendor = @Vendor AND Product IS NOT NULL";
            object[] parameters =
            [
                new MySqlParameter("@Vendor", vendor)
            ];
            return _context.Database.SqlQueryRaw<string>(sql, parameters).ToList();
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
            var sql = @"
                SELECT DISTINCT cm.CveId
                FROM CveMetadata cm
                JOIN RootCve r ON cm.CveMetadataId = r.CveMetadataId
                JOIN Containers c ON r.RootCveId = c.RootCveId
                JOIN CnaContainer cc ON c.CnaId = cc.CnaId
                JOIN Affected a ON cc.CnaId = a.CnaId
                WHERE a.Vendor = @Vendor AND a.Product = @Product";
            object[] parameters =
            [
                new MySqlParameter("@Vendor", vendor),
                new MySqlParameter("@Product", product)
            ];
            return _context.Database.SqlQueryRaw<string>(sql, parameters).ToList();
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
            var sql = @"
                SELECT a.Vendor, a.Product, cm.CveId
                FROM Affected a
                JOIN CnaContainer cc ON a.CnaId = cc.CnaId
                JOIN Containers c ON cc.CnaId = c.CnaId
                JOIN RootCve r ON c.RootCveId = r.RootCveId
                JOIN CveMetadata cm ON r.CveMetadataId = cm.CveMetadataId
                WHERE a.Vendor IS NOT NULL AND a.Product IS NOT NULL";
            var results = _context.Database.SqlQueryRaw<CveProductResult>(sql).ToList();
            return results.GroupBy(r => (r.Vendor, r.Product))
                          .Select(g => new CveProduct
                          {
                              Vendor = g.Key.Vendor,
                              Product = g.Key.Product,
                              CveIds = g.Select(x => x.CveId).ToList()
                          }).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    public Cve.RootCve SelectCveDataByCveId(string cveId)
    {
        try
        {
            // 1. 查詢CveMetadata
            var metadataSql = @"
                SELECT cm.CveMetadataId, cm.CveId, cm.AssignerOrgId, cm.AssignerShortName, 
                       cm.State, cm.DateReserved, cm.DatePublished, cm.DateUpdated
                FROM CveMetadata cm
                WHERE cm.CveId = @CveId";

            object[] metadataParams =
            [
                new MySqlParameter("@CveId", cveId)
            ];

            var cveMetadata = _context.Database.SqlQueryRaw<CveMetadataDto>(metadataSql, metadataParams).FirstOrDefault();
            if (cveMetadata == null)
            {
                return null;
            }

            // 2. 查詢RootCve基本資訊
            var rootCveSql = @"
                SELECT r.RootCveId, r.DataType, r.DataVersion
                FROM CveMetadata cm
                JOIN RootCve r ON cm.CveMetadataId = r.CveMetadataId
                WHERE cm.CveId = @CveId";

            var rootCveInfo = _context.Database.SqlQueryRaw<RootCveDto>(rootCveSql, metadataParams).FirstOrDefault();
            if (rootCveInfo == null)
            {
                return null;
            }

            // 3. 單獨查詢Containers資訊
            var containersSql = @"
                SELECT ContainersId, CnaId
                FROM Containers
                WHERE RootCveId = @RootCveId";
            
            object[] containersParams =
            [
                new MySqlParameter("@RootCveId", rootCveInfo.RootCveId)
            ];
            
            var containersInfo = _context.Database.SqlQueryRaw<ContainersDto>(containersSql, containersParams).FirstOrDefault();
            
            // 4. 創建RootCve物件
            var rootCve = new Cve.RootCve
            {
                DataType = rootCveInfo.DataType,
                DataVersion = rootCveInfo.DataVersion,
                CveMetadata = new Cve.CveMetadata
                {
                    CveId = cveMetadata.CveId,
                    AssignerOrgId = cveMetadata.AssignerOrgId,
                    AssignerShortName = cveMetadata.AssignerShortName,
                    State = cveMetadata.State,
                    DateReserved = cveMetadata.DateReserved,
                    DatePublished = cveMetadata.DatePublished,
                    DateUpdated = cveMetadata.DateUpdated
                },
                Containers = new Cve.Containers()
            };

            // 5. 如果有CNA資訊，查詢CNA相關資料
            if (containersInfo?.CnaId != null)
            {
                var cnaSql = @"
                    SELECT cc.CnaId, cc.Title, cc.ProviderMetadataId,
                           pm.OrgId, pm.ShortName, pm.DateUpdated
                    FROM CnaContainer cc
                    LEFT JOIN ProviderMetadata pm ON cc.ProviderMetadataId = pm.ProviderMetadataId
                    WHERE cc.CnaId = @CnaId";

                object[] cnaParams =
                [
                    new MySqlParameter("@CnaId", containersInfo.CnaId)
                ];

                var cnaInfo = _context.Database.SqlQueryRaw<CnaDto>(cnaSql, cnaParams).FirstOrDefault();

                if (cnaInfo != null)
                {
                    rootCve.Containers.Cna = new Cve.CnaContainer
                    {
                        Title = cnaInfo.Title,
                        ProviderMetadata = new Cve.ProviderMetadata
                        {
                            OrgId = cnaInfo.OrgId,
                            ShortName = cnaInfo.ShortName,
                            DateUpdated = cnaInfo.DateUpdated
                        }
                    };

                    // 6. 查詢受影響產品
                    var affectedSql = @"
                        SELECT a.AffectedId, a.Vendor, a.Product
                        FROM Affected a
                        WHERE a.CnaId = @CnaId";

                    var affectedProducts = _context.Database.SqlQueryRaw<AffectedWithIdDto>(affectedSql, cnaParams).ToList();

                    if (affectedProducts.Any())
                    {
                        rootCve.Containers.Cna.Affected = [];

                        foreach (var product in affectedProducts)
                        {
                            var affected = new Cve.Affected
                            {
                                Vendor = product.Vendor,
                                Product = product.Product
                            };

                            // 查詢版本資訊
                            var versionsSql = @"
                                SELECT VersionId, AffectedId, VersionValue, Status, LessThanOrEqual, VersionType
                                FROM Versions
                                WHERE AffectedId = @AffectedId";

                            object[] versionsParams =
                            [
                                new MySqlParameter("@AffectedId", product.AffectedId)
                            ];

                            var versions = _context.Database.SqlQueryRaw<VersionDto>(versionsSql, versionsParams).ToList();

                            if (versions.Any())
                            {
                                affected.Versions = [];

                                foreach (var version in versions)
                                {
                                    affected.Versions.Add(new Cve.Version
                                    {
                                        VersionValue = version.VersionValue,
                                        Status = version.Status,
                                        LessThanOrEqual = version.LessThanOrEqual,
                                        VersionType = version.VersionType
                                    });
                                }
                            }

                            rootCve.Containers.Cna.Affected.Add(affected);
                        }
                    }

                    // 7. 查詢描述資訊
                    var descriptionSql = @"
                        SELECT Language, DescriptionText
                        FROM Description
                        WHERE CveId = @CveId";

                    var descriptions = _context.Database.SqlQueryRaw<DescriptionDto>(descriptionSql, metadataParams).ToList();

                    if (descriptions.Any())
                    {
                        rootCve.Containers.Cna.Descriptions = [];

                        foreach (var description in descriptions)
                        {
                            rootCve.Containers.Cna.Descriptions.Add(new Cve.Description
                            {
                                Language = description.Language,
                                DescriptionText = description.DescriptionText
                            });
                        }
                    }
                }
            }

            // 8. 如果有ContainersId，查詢ADP資訊
            if (containersInfo != null)
            {
                var adpSql = @"
                    SELECT ac.AdpId, ac.Title, ac.ProviderMetadataId,
                           pm.OrgId, pm.ShortName, pm.DateUpdated
                    FROM AdpContainer ac
                    LEFT JOIN ProviderMetadata pm ON ac.ProviderMetadataId = pm.ProviderMetadataId
                    WHERE ac.ContainersId = @ContainersId";

                object[] adpParams =
                [
                    new MySqlParameter("@ContainersId", containersInfo.ContainersId)
                ];

                var adpResults = _context.Database.SqlQueryRaw<AdpContainerDto>(adpSql, adpParams).ToList();

                if (adpResults.Any())
                {
                    rootCve.Containers.Adp = [];

                    foreach (var adp in adpResults)
                    {
                        var adpContainer = new Cve.AdpContainer
                        {
                            Title = adp.Title,
                            ProviderMetadata = new Cve.ProviderMetadata
                            {
                                OrgId = adp.OrgId,
                                ShortName = adp.ShortName,
                                DateUpdated = adp.DateUpdated
                            }
                        };

                        rootCve.Containers.Adp.Add(adpContainer);
                    }
                }
            }

            return rootCve;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}
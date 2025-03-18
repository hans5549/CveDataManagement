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
                    
                    // 8. 查詢CVSS評分資訊
                    try
                    {
                        var metricsSql = @"
                            SELECT CveCvssScoreId, CveId, Format, Version, BaseScore, BaseSeverity, VectorString
                            FROM CveCvssScore
                            WHERE CveId = @CveId";
                        
                        var metrics = _context.Database.SqlQueryRaw<MetricsDto>(metricsSql, metadataParams).ToList();
                        
                        if (metrics.Any())
                        {
                            rootCve.Containers.Cna.Metrics = [];
                            
                            foreach (var metric in metrics)
                            {
                                var metricItem = new Cve.Metric();
                                
                                // 根據Format和Version確定CVSS版本
                                if (metric.Format == "CVSS" && metric.Version != null)
                                {
                                    // CVSS v4.0
                                    if (metric.Version.StartsWith("4."))
                                    {
                                        metricItem.CvssV4_0 = new Cve.CvssV4_0
                                        {
                                            Version = metric.Version,
                                            BaseScore = metric.BaseScore,
                                            BaseSeverity = metric.BaseSeverity,
                                            VectorString = metric.VectorString
                                        };
                                    }
                                    // CVSS v3.1
                                    else if (metric.Version.StartsWith("3.1"))
                                    {
                                        metricItem.CvssV3_1 = new Cve.CvssV3_1
                                        {
                                            Version = metric.Version,
                                            BaseScore = metric.BaseScore,
                                            BaseSeverity = metric.BaseSeverity,
                                            VectorString = metric.VectorString
                                        };
                                    }
                                    // CVSS v3.0
                                    else if (metric.Version.StartsWith("3.0"))
                                    {
                                        metricItem.CvssV3_0 = new Cve.CvssV3_0
                                        {
                                            Version = metric.Version,
                                            BaseScore = metric.BaseScore,
                                            BaseSeverity = metric.BaseSeverity,
                                            VectorString = metric.VectorString
                                        };
                                    }
                                    // CVSS v2.0
                                    else if (metric.Version.StartsWith("2."))
                                    {
                                        metricItem.CvssV2_0 = new Cve.CvssV2_0
                                        {
                                            Version = metric.Version,
                                            BaseScore = metric.BaseScore,
                                            VectorString = metric.VectorString
                                        };
                                    }
                                }
                                
                                rootCve.Containers.Cna.Metrics.Add(metricItem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"查詢CveCvssScore表錯誤: {ex.Message}");
                        // 初始化空集合，確保不會因為表不存在而導致空引用異常
                        rootCve.Containers.Cna.Metrics = [];
                    }
                    
                    // 9. 查詢問題類型（ProblemTypes）
                    try
                    {
                        var problemTypesSql = @"
                            SELECT ProblemTypeId, CnaId
                            FROM ProblemType
                            WHERE CnaId = @CnaId";
                        
                        var problemTypes = _context.Database.SqlQueryRaw<ProblemTypeDto>(problemTypesSql, cnaParams).ToList();
                        
                        if (problemTypes.Any())
                        {
                            rootCve.Containers.Cna.ProblemTypes = [];
                            
                            foreach (var problemType in problemTypes)
                            {
                                var problemTypeDesc = new Cve.ProblemType
                                {
                                    Descriptions = []
                                };
                                
                                // 查詢問題類型描述
                                try 
                                {
                                    var problemTypeDescSql = @"
                                        SELECT CweId, Description, Language, Type
                                        FROM ProblemTypeDescription
                                        WHERE ProblemTypeId = @ProblemTypeId";
                                    
                                    object[] problemTypeParams =
                                    [
                                        new MySqlParameter("@ProblemTypeId", problemType.ProblemTypeId)
                                    ];
                                    
                                    var problemTypeDescriptions = _context.Database.SqlQueryRaw<ProblemTypeDescriptionDto>(problemTypeDescSql, problemTypeParams).ToList();
                                    
                                    if (problemTypeDescriptions.Any())
                                    {
                                        foreach (var desc in problemTypeDescriptions)
                                        {
                                            problemTypeDesc.Descriptions.Add(new Cve.ProblemTypeDescription
                                            {
                                                CweId = desc.CweId,
                                                Description = desc.Description,
                                                Language = desc.Language,
                                                Type = desc.Type
                                            });
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"查詢ProblemTypeDescription表錯誤: {ex.Message}");
                                }
                                
                                rootCve.Containers.Cna.ProblemTypes.Add(problemTypeDesc);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"查詢ProblemType表錯誤: {ex.Message}");
                        // 初始化空集合，確保不會因為表不存在而導致空引用異常
                        rootCve.Containers.Cna.ProblemTypes = [];
                    }
                    
                    // 10. 查詢參考資料（References）
                    try
                    {
                        var referencesSql = @"
                            SELECT ReferenceId, CveId, Url, Name
                            FROM Reference 
                            WHERE CveId = @CveId";
                        
                        var references = _context.Database.SqlQueryRaw<ReferenceDto>(referencesSql, metadataParams).ToList();
                        
                        if (references.Any())
                        {
                            rootCve.Containers.Cna.References = [];
                            
                            foreach (var reference in references)
                            {
                                var referenceItem = new Cve.Reference
                                {
                                    Url = reference.Url,
                                    Name = reference.Name,
                                    Tags = []
                                };
                                
                                // 查詢參考標籤
                                try
                                {
                                    var tagsSql = @"
                                        SELECT Tag
                                        FROM ReferenceTags
                                        WHERE ReferenceId = @ReferenceId";
                                    
                                    object[] tagsParams =
                                    [
                                        new MySqlParameter("@ReferenceId", reference.ReferenceId)
                                    ];
                                    
                                    var tags = _context.Database.SqlQueryRaw<string>(tagsSql, tagsParams).ToList();
                                    
                                    if (tags.Any())
                                    {
                                        referenceItem.Tags.AddRange(tags);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"查詢ReferenceTags表錯誤: {ex.Message}");
                                }
                                
                                rootCve.Containers.Cna.References.Add(referenceItem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"查詢Reference表錯誤: {ex.Message}");
                        // 初始化空集合，確保不會因為表不存在而導致空引用異常
                        rootCve.Containers.Cna.References = [];
                    }
                    
                    // 11. 查詢貢獻者（Credits）
                    try
                    {
                        var creditsSql = @"
                            SELECT Language, Type, Value
                            FROM Credit
                            WHERE CveId = @CveId";
                        
                        var credits = _context.Database.SqlQueryRaw<CreditDto>(creditsSql, metadataParams).ToList();
                        
                        if (credits.Any())
                        {
                            rootCve.Containers.Cna.Credits = [];
                            
                            foreach (var credit in credits)
                            {
                                rootCve.Containers.Cna.Credits.Add(new Cve.Credit
                                {
                                    Language = credit.Language,
                                    Type = credit.Type,
                                    Value = credit.Value
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"查詢Credit表錯誤: {ex.Message}");
                        // 初始化空集合，確保不會因為表不存在而導致空引用異常
                        rootCve.Containers.Cna.Credits = [];
                    }
                    
                    // 12. 查詢時間線（Timeline）
                    try
                    {
                        var timelineSql = @"
                            SELECT Time, Language, Value
                            FROM TimelineEntry
                            WHERE CveId = @CveId
                            ORDER BY Time";
                        
                        var timeline = _context.Database.SqlQueryRaw<TimelineDto>(timelineSql, metadataParams).ToList();
                        
                        if (timeline.Any())
                        {
                            rootCve.Containers.Cna.Timeline = [];
                            
                            foreach (var entry in timeline)
                            {
                                rootCve.Containers.Cna.Timeline.Add(new Cve.TimelineEntry
                                {
                                    Time = entry.Time,
                                    Language = entry.Language,
                                    Value = entry.Value
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"查詢TimelineEntry表錯誤: {ex.Message}");
                        // 初始化空集合，確保不會因為表不存在而導致空引用異常
                        rootCve.Containers.Cna.Timeline = [];
                    }
                }
            }

            // 13. 如果有ContainersId，查詢ADP資訊
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
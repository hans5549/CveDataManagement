-- 1. RootCve 表 - 儲存 CVE 資料的根結構
CREATE TABLE RootCve
(
    RootCveId     INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    DataType      VARCHAR(50),                   -- 資料類型
    DataVersion   VARCHAR(10),                   -- 資料版本
    CveMetadataId INT                            -- CVE 元資料 ID
);
CREATE INDEX IX_RootCve_CveMetadataId ON RootCve (CveMetadataId);
-- 為 CveMetadataId 建立索引

-- 2. CveMetadata 表 - 儲存 CVE 的元資料
CREATE TABLE CveMetadata
(
    CveMetadataId     INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveId             VARCHAR(20) UNIQUE,            -- CVE 識別碼，如 CVE-2023-1234
    AssignerOrgId     VARCHAR(50),                   -- 分配機構的組織 ID
    AssignerShortName VARCHAR(50),                   -- 分配機構的簡稱
    State             VARCHAR(20),                   -- CVE 記錄的狀態
    DateReserved      DATETIME,                      -- CVE ID 保留日期
    DatePublished     DATETIME,                      -- CVE 首次公開日期
    DateUpdated       DATETIME                       -- CVE 最後更新日期
);
CREATE INDEX IX_CveMetadata_CveId ON CveMetadata (CveId);
-- 為 CveId 建立索引

-- 3. Containers 表 - 儲存 CNA 和 ADP 的容器結構
CREATE TABLE Containers
(
    ContainersId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    RootCveId    INT,                           -- 根 CVE ID
    CnaId        INT                            -- CNA 容器 ID
);
CREATE INDEX IX_Containers_RootCveId ON Containers (RootCveId); -- 為 RootCveId 建立索引
CREATE INDEX IX_Containers_CnaId ON Containers (CnaId);
-- 為 CnaId 建立索引

-- 4. CnaContainer 表 - 儲存 CNA 的主要資訊
CREATE TABLE CnaContainer
(
    CnaId              INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    ProviderMetadataId INT,                           -- 提供者元資料 ID
    Title              VARCHAR(1000)                  -- CVE 標題
);
CREATE INDEX IX_CnaContainer_ProviderMetadataId ON CnaContainer (ProviderMetadataId);
-- 為 ProviderMetadataId 建立索引

-- 5. ProviderMetadata 表 - 儲存提供者元資料
CREATE TABLE ProviderMetadata
(
    ProviderMetadataId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    OrgId              VARCHAR(50),                   -- 組織 ID
    ShortName          VARCHAR(50),                   -- 組織簡稱
    DateUpdated        DATETIME                       -- 最後更新日期
);
CREATE INDEX IX_ProviderMetadata_OrgId ON ProviderMetadata (OrgId);
-- 為 OrgId 建立索引

-- 6. Affected 表 - 儲存受影響的產品資訊
CREATE TABLE Affected
(
    AffectedId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CnaId      INT,                           -- CNA 容器 ID
    Vendor     VARCHAR(500),                  -- 廠商名稱
    Product    NVARCHAR(MAX)                  -- 產品名稱
);
CREATE INDEX IX_Affected_CnaId ON Affected (CnaId);
-- 為 CnaId 建立索引

-- 7. Versions 表 - 儲存受影響產品的版本資訊
CREATE TABLE Versions
(
    VersionId       INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    AffectedId      INT,                           -- 受影響產品 ID
    VersionValue    NVARCHAR(MAX),                 -- 版本值
    Status          VARCHAR(20),                   -- 版本狀態，如 "affected" 或 "unaffected"
    LessThanOrEqual NVARCHAR(MAX),                 -- 小於等於的版本範圍
    VersionType     VARCHAR(50)                    -- 版本類型
);
CREATE INDEX IX_Versions_AffectedId ON Versions (AffectedId);
-- 為 AffectedId 建立索引

-- 8. Modules 表 - 儲存受影響的模組資訊
CREATE TABLE Modules
(
    ModuleId   INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    AffectedId INT,                           -- 受影響產品 ID
    ModuleName VARCHAR(1000)                  -- 模組名稱
);
CREATE INDEX IX_Modules_AffectedId ON Modules (AffectedId);
-- 為 AffectedId 建立索引

-- 9. AdpContainer 表 - 儲存 ADP 的資訊
CREATE TABLE AdpContainer
(
    AdpId              INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    ContainersId       INT,                           -- 容器 ID
    Title              VARCHAR(255),                  -- ADP 提供的標題
    ProviderMetadataId INT                            -- ADP 提供者元資料 ID
);
CREATE INDEX IX_AdpContainer_ContainersId ON AdpContainer (ContainersId); -- 為 ContainersId 建立索引
CREATE INDEX IX_AdpContainer_ProviderMetadataId ON AdpContainer (ProviderMetadataId);
-- 為 ProviderMetadataId 建立索引

-- 10. CveRecord 表 - 儲存核心 CVE 記錄資訊
CREATE TABLE CveRecord
(
    CveRecordId       INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveId             VARCHAR(20),                   -- CVE 識別碼
    Title             VARCHAR(255),                  -- CVE 標題
    DatePublished     DATETIME,                      -- 公開日期
    DateReserved      DATETIME,                      -- 保留日期
    DateUpdated       DATETIME,                      -- 更新日期
    DatePublic        DATETIME,                      -- 公開披露日期
    DateRejected      DATETIME,                      -- 拒絕日期
    AssignerOrgId     VARCHAR(50),                   -- 分配機構的組織 ID
    AssignerShortName VARCHAR(50),                   -- 分配機構的簡稱
    State             VARCHAR(20),                   -- CVE 記錄狀態
    Discovery         VARCHAR(50)                    -- 漏洞發現方式
);
CREATE INDEX IX_CveRecord_CveId ON CveRecord (CveId);
-- 為 CveId 建立索引

-- 11. CveAffectedProduct 表 - 儲存受影響的產品詳細資訊
CREATE TABLE CveAffectedProduct
(
    CveAffectedProductId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveId                VARCHAR(20),                   -- CVE 識別碼
    Vendor               VARCHAR(100),                  -- 廠商名稱
    Product              VARCHAR(100),                  -- 產品名稱
    DefaultStatus        VARCHAR(20),                   -- 預設狀態
    Repo                 VARCHAR(255),                  -- 程式碼儲存庫
    CollectionUrl        VARCHAR(255),                  -- 集合 URL
    PackageName          VARCHAR(100)                   -- 套件名稱
);
CREATE INDEX IX_CveAffectedProduct_CveId ON CveAffectedProduct (CveId); -- 為 CveId 建立索引
CREATE INDEX IX_CveAffectedProduct_Vendor_Product ON CveAffectedProduct (Vendor, Product);
-- 為 Vendor 和 Product 建立複合索引

-- 12. CveAffectedProductCpes 表 - 儲存 CPE 列表（多對多關係）
CREATE TABLE CveAffectedProductCpes
(
    CveAffectedProductCpeId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveAffectedProductId    INT,                           -- 受影響產品 ID
    Cpe                     VARCHAR(255)                   -- CPE 值
);
CREATE INDEX IX_CveAffectedProductCpes_CveAffectedProductId ON CveAffectedProductCpes (CveAffectedProductId);
-- 為 CveAffectedProductId 建立索引

-- 13. CveAffectedProductModules 表 - 儲存模組列表（多對多關係）
CREATE TABLE CveAffectedProductModules
(
    CveAffectedProductModuleId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveAffectedProductId       INT,                           -- 受影響產品 ID
    ModuleName                 VARCHAR(100)                   -- 模組名稱
);
CREATE INDEX IX_CveAffectedProductModules_CveAffectedProductId ON CveAffectedProductModules (CveAffectedProductId);
-- 為 CveAffectedProductId 建立索引

-- 14. CveVersion 表 - 儲存版本詳細資訊
CREATE TABLE CveVersion
(
    CveVersionId    INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveId           VARCHAR(20),                   -- CVE 識別碼
    Product         VARCHAR(100),                  -- 產品名稱
    Version         VARCHAR(50),                   -- 版本號
    Status          VARCHAR(20),                   -- 版本狀態
    LessThan        VARCHAR(50),                   -- 小於的版本範圍
    LessThanOrEqual VARCHAR(50),                   -- 小於等於的版本範圍
    VersionType     VARCHAR(50),                   -- 版本類型
    ChangeAt        VARCHAR(50),                   -- 變更時間
    ChangeStatus    VARCHAR(20)                    -- 變更狀態
);
CREATE INDEX IX_CveVersion_CveId ON CveVersion (CveId); -- 為 CveId 建立索引
CREATE INDEX IX_CveVersion_Product_Version ON CveVersion (Product, Version);
-- 為 Product 和 Version 建立複合索引

-- 15. CveModule 表 - 儲存 CVE 相關模組
CREATE TABLE CveModule
(
    CveModuleId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveId       VARCHAR(20),                   -- CVE 識別碼
    ModuleName  VARCHAR(100)                   -- 模組名稱
);
CREATE INDEX IX_CveModule_CveId ON CveModule (CveId); -- 為 CveId 建立索引
CREATE INDEX IX_CveModule_ModuleName ON CveModule (ModuleName);
-- 為 ModuleName 建立索引

-- 16. Description 表 - 儲存 CVE 描述
CREATE TABLE Description
(
    DescriptionId   INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveId           VARCHAR(20),                   -- CVE 識別碼
    Language        VARCHAR(10),                   -- 描述語言
    DescriptionText NVARCHAR(MAX)                  -- 描述文字內容
);
CREATE INDEX IX_Description_CveId ON Description (CveId);
-- 為 CveId 建立索引

-- 17. CveSupportingMedia 表 - 儲存 CVE 支援媒體
CREATE TABLE CveSupportingMedia
(
    CveSupportingMediaId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    DescriptionId        INT,                           -- 描述 ID
    Language             VARCHAR(10),                   -- 媒體語言
    Type                 VARCHAR(50),                   -- 媒體類型
    Base64               BIT,                           -- 是否為 Base64 編碼
    Value                NVARCHAR(MAX)                  -- 媒體內容值
);
CREATE INDEX IX_CveSupportingMedia_DescriptionId ON CveSupportingMedia (DescriptionId);
-- 為 DescriptionId 建立索引

-- 18. CveCvssScore 表 - 儲存 CVSS 評分資訊
CREATE TABLE CveCvssScore
(
    CveCvssScoreId              INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveId                       VARCHAR(20),                   -- CVE 識別碼
    Format                      VARCHAR(50),                   -- 評分格式
    Scenario                    VARCHAR(50),                   -- 評分情境
    Version                     VARCHAR(10),                   -- CVSS 版本
    BaseScore                   FLOAT,                         -- 基本分數
    BaseSeverity                VARCHAR(20),                   -- 基本嚴重性等級
    VectorString                VARCHAR(255),                  -- 向量字串
    AttackVector                VARCHAR(20),                   -- 攻擊途徑
    AttackComplexity            VARCHAR(20),                   -- 攻擊複雜度
    PrivilegesRequired          VARCHAR(20),                   -- 所需權限
    UserInteraction             VARCHAR(20),                   -- 使用者互動
    Scope                       VARCHAR(20),                   -- 影響範圍
    ConfidentialityImpact       VARCHAR(20),                   -- 機密性影響
    IntegrityImpact             VARCHAR(20),                   -- 完整性影響
    AvailabilityImpact          VARCHAR(20),                   -- 可用性影響
    Automatable                 VARCHAR(20),                   -- 是否可自動化利用
    Recovery                    VARCHAR(20),                   -- 復原難度
    Safety                      VARCHAR(20),                   -- 安全性影響
    AttackRequirements          VARCHAR(20),                   -- 攻擊需求
    ProviderUrgency             VARCHAR(20),                   -- 提供者緊急程度
    SubConfidentialityImpact    VARCHAR(20),                   -- 次要機密性影響
    SubIntegrityImpact          VARCHAR(20),                   -- 次要完整性影響
    SubAvailabilityImpact       VARCHAR(20),                   -- 次要可用性影響
    ValueDensity                VARCHAR(20),                   -- 價值密度
    VulnerabilityResponseEffort VARCHAR(20)                    -- 漏洞回應所需努力
);
CREATE INDEX IX_CveCvssScore_CveId ON CveCvssScore (CveId); -- 為 CveId 建立索引
CREATE INDEX IX_CveCvssScore_Version ON CveCvssScore (Version);
-- 為 Version 建立索引

-- 19. ProblemType 表 - 儲存問題類型
CREATE TABLE ProblemType
(
    ProblemTypeId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CnaId         INT                            -- CNA 容器 ID
);
CREATE INDEX IX_ProblemType_CnaId ON ProblemType (CnaId);
-- 為 CnaId 建立索引

-- 20. ProblemTypeDescription 表 - 儲存問題類型描述
CREATE TABLE ProblemTypeDescription
(
    ProblemTypeDescriptionId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    ProblemTypeId            INT,                           -- 問題類型 ID
    CveId                    VARCHAR(20),                   -- CVE 識別碼
    CweId                    VARCHAR(20),                   -- CWE 識別碼，如 CWE-79
    Description              NVARCHAR(MAX),                 -- 問題描述
    Language                 VARCHAR(10),                   -- 描述語言
    Type                     VARCHAR(50)                    -- 描述類型
);
CREATE INDEX IX_ProblemTypeDescription_ProblemTypeId ON ProblemTypeDescription (ProblemTypeId); -- 為 ProblemTypeId 建立索引
CREATE INDEX IX_ProblemTypeDescription_CveId ON ProblemTypeDescription (CveId); -- 為 CveId 建立索引
CREATE INDEX IX_ProblemTypeDescription_CweId ON ProblemTypeDescription (CweId);
-- 為 CweId 建立索引

-- 21. Metric 表 - 儲存 CNA 中的評分指標
CREATE TABLE Metric
(
    MetricId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CnaId    INT                            -- CNA 容器 ID
);
CREATE INDEX IX_Metric_CnaId ON Metric (CnaId);
-- 為 CnaId 建立索引

-- 22. CvssV4_0 表 - 儲存 CVSS v4.0 評分
CREATE TABLE CvssV4_0
(
    CvssV4_0Id   INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    MetricId     INT,                           -- 評分指標 ID
    Version      VARCHAR(10),                   -- CVSS 版本
    BaseScore    FLOAT,                         -- 基本分數
    VectorString VARCHAR(255),                  -- 向量字串
    BaseSeverity VARCHAR(20)                    -- 基本嚴重性等級
);
CREATE INDEX IX_CvssV4_0_MetricId ON CvssV4_0 (MetricId);
-- 為 MetricId 建立索引

-- 23. CvssV3_1 表 - 儲存 CVSS v3.1 評分
CREATE TABLE CvssV3_1
(
    CvssV3_1Id   INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    MetricId     INT,                           -- 評分指標 ID
    Version      VARCHAR(10),                   -- CVSS 版本
    BaseScore    FLOAT,                         -- 基本分數
    VectorString VARCHAR(255),                  -- 向量字串
    BaseSeverity VARCHAR(20)                    -- 基本嚴重性等級
);
CREATE INDEX IX_CvssV3_1_MetricId ON CvssV3_1 (MetricId);
-- 為 MetricId 建立索引

-- 24. CvssV3_0 表 - 儲存 CVSS v3.0 評分
CREATE TABLE CvssV3_0
(
    CvssV3_0Id   INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    MetricId     INT,                           -- 評分指標 ID
    Version      VARCHAR(10),                   -- CVSS 版本
    BaseScore    FLOAT,                         -- 基本分數
    VectorString VARCHAR(255),                  -- 向量字串
    BaseSeverity VARCHAR(20)                    -- 基本嚴重性等級
);
CREATE INDEX IX_CvssV3_0_MetricId ON CvssV3_0 (MetricId);
-- 為 MetricId 建立索引

-- 25. CvssV2_0 表 - 儲存 CVSS v2.0 評分
CREATE TABLE CvssV2_0
(
    CvssV2_0Id   INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    MetricId     INT,                           -- 評分指標 ID
    Version      VARCHAR(10),                   -- CVSS 版本
    BaseScore    FLOAT,                         -- 基本分數
    VectorString VARCHAR(255)                   -- 向量字串
);
CREATE INDEX IX_CvssV2_0_MetricId ON CvssV2_0 (MetricId);
-- 為 MetricId 建立索引

-- 26. AdpMetric 表 - 儲存 ADP 中的評分指標
CREATE TABLE AdpMetric
(
    AdpMetricId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    AdpId       INT                            -- ADP 容器 ID
);
CREATE INDEX IX_AdpMetric_AdpId ON AdpMetric (AdpId);
-- 為 AdpId 建立索引

-- 27. Ssvc 表 - 儲存 SSVC 評分
CREATE TABLE Ssvc
(
    SsvcId      INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    AdpMetricId INT,                           -- ADP 評分指標 ID
    Type        VARCHAR(50)                    -- SSVC 類型
);
CREATE INDEX IX_Ssvc_AdpMetricId ON Ssvc (AdpMetricId);
-- 為 AdpMetricId 建立索引

-- 28. SsvcContent 表 - 儲存 SSVC 內容
CREATE TABLE SsvcContent
(
    SsvcContentId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    SsvcId        INT,                           -- SSVC ID
    Id            VARCHAR(50),                   -- SSVC 識別碼
    Timestamp     DATETIME,                      -- 時間戳記
    Role          VARCHAR(50),                   -- 角色
    Version       VARCHAR(10)                    -- SSVC 版本
);
CREATE INDEX IX_SsvcContent_SsvcId ON SsvcContent (SsvcId);
-- 為 SsvcId 建立索引

-- 29. SsvcOption 表 - 儲存 SSVC 選項
CREATE TABLE SsvcOption
(
    SsvcOptionId    INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    SsvcContentId   INT,                           -- SSVC 內容 ID
    Exploitation    VARCHAR(20),                   -- 利用狀態
    Automatable     VARCHAR(20),                   -- 是否可自動化利用
    TechnicalImpact VARCHAR(50)                    -- 技術影響
);
CREATE INDEX IX_SsvcOption_SsvcContentId ON SsvcOption (SsvcContentId);
-- 為 SsvcContentId 建立索引

-- 30. TimelineEntry 表 - 儲存時間線事件
CREATE TABLE TimelineEntry
(
    TimelineEntryId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CnaId           INT,                           -- CNA 容器 ID
    CveId           VARCHAR(20),                   -- CVE 識別碼
    Time            DATETIME,                      -- 事件時間
    Language        VARCHAR(10),                   -- 事件描述語言
    Value           NVARCHAR(MAX)                  -- 事件描述內容
);
CREATE INDEX IX_TimelineEntry_CnaId ON TimelineEntry (CnaId); -- 為 CnaId 建立索引
CREATE INDEX IX_TimelineEntry_CveId ON TimelineEntry (CveId);
-- 為 CveId 建立索引

-- 31. Credit 表 - 儲存貢獻者資訊
CREATE TABLE Credit
(
    CreditId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CnaId    INT,                           -- CNA 容器 ID
    CveId    VARCHAR(20),                   -- CVE 識別碼
    Language VARCHAR(10),                   -- 貢獻者描述語言
    Type     VARCHAR(50),                   -- 貢獻類型
    Value    VARCHAR(500)                   -- 貢獻者資訊
);
CREATE INDEX IX_Credit_CnaId ON Credit (CnaId); -- 為 CnaId 建立索引
CREATE INDEX IX_Credit_CveId ON Credit (CveId);
-- 為 CveId 建立索引

-- 32. Reference 表 - 儲存參考資料
CREATE TABLE Reference
(
    ReferenceId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CnaId       INT,                           -- CNA 容器 ID
    CveId       VARCHAR(20),                   -- CVE 識別碼
    Url         NVARCHAR(MAX),                 -- 參考資料 URL
    Name        NVARCHAR(MAX)                  -- 參考資料名稱
);
CREATE INDEX IX_Reference_CnaId ON Reference (CnaId); -- 為 CnaId 建立索引
CREATE INDEX IX_Reference_CveId ON Reference (CveId);
-- 為 CveId 建立索引

-- 33. ReferenceTags 表 - 儲存參考資料標籤（多對多關係）
CREATE TABLE ReferenceTags
(
    ReferenceTagId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    ReferenceId    INT,                           -- 參考資料 ID
    Tag            NVARCHAR(MAX)                  -- 標籤
);
CREATE INDEX IX_ReferenceTags_ReferenceId ON ReferenceTags (ReferenceId);
-- 為 ReferenceId 建立索引

-- 34. CvePlatform 表 - 儲存 CVE 平台資訊
CREATE TABLE CvePlatform
(
    CvePlatformId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveId         VARCHAR(20),                   -- CVE 識別碼
    Platform      VARCHAR(100)                   -- 平台名稱
);
CREATE INDEX IX_CvePlatform_CveId ON CvePlatform (CveId);
-- 為 CveId 建立索引

-- 35. CveProgramFile 表 - 儲存 CVE 程式檔案資訊
CREATE TABLE CveProgramFile
(
    CveProgramFileId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveId            VARCHAR(20),                   -- CVE 識別碼
    FilePath         VARCHAR(255)                   -- 檔案路徑
);
CREATE INDEX IX_CveProgramFile_CveId ON CveProgramFile (CveId);
-- 為 CveId 建立索引

-- 36. CveProgramRoutine 表 - 儲存 CVE 程式例程資訊
CREATE TABLE CveProgramRoutine
(
    CveProgramRoutineId INT IDENTITY(1,1) PRIMARY KEY, -- 主鍵，自增識別碼
    CveId               VARCHAR(20),                   -- CVE 識別碼
    RoutineName         VARCHAR(100)                   -- 例程名稱
);
CREATE INDEX IX_CveProgramRoutine_CveId ON CveProgramRoutine (CveId); -- 為 CveId 建立索引

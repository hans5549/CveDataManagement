using Newtonsoft.Json;

namespace WebConsole.Models;

/// <summary>
/// CVE (Common Vulnerabilities and Exposures) 資料模型
/// </summary>
public class Cve
{
    /// <summary>
    /// CVE 資料的根結構
    /// </summary>
    public class RootCve
    {
        /// <summary>
        /// 資料類型
        /// </summary>
        public string? DataType { get; set; }

        /// <summary>
        /// 資料版本
        /// </summary>
        public string? DataVersion { get; set; }

        /// <summary>
        /// CVE 元資料
        /// </summary>
        public CveMetadata? CveMetadata { get; set; }

        /// <summary>
        /// 包含 CNA 和 ADP 的容器
        /// </summary>
        public Containers? Containers { get; set; }
    }

    /// <summary>
    /// CVE 元資料，包含基本識別資訊
    /// </summary>
    public class CveMetadata
    {
        /// <summary>
        /// CVE 識別碼，如 CVE-2023-1234
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 分配機構的組織 ID
        /// </summary>
        public string? AssignerOrgId { get; set; }

        /// <summary>
        /// 分配機構的簡稱
        /// </summary>
        public string? AssignerShortName { get; set; }

        /// <summary>
        /// CVE 記錄的狀態
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// CVE ID 保留日期
        /// </summary>
        public DateTime? DateReserved { get; set; }

        /// <summary>
        /// CVE 首次公開日期
        /// </summary>
        public DateTime? DatePublished { get; set; }

        /// <summary>
        /// CVE 最後更新日期
        /// </summary>
        public DateTime? DateUpdated { get; set; }
    }

    /// <summary>
    /// 包含 CNA 和 ADP 資料的容器結構
    /// </summary>
    public class Containers
    {
        /// <summary>
        /// CNA (CVE Numbering Authority) 容器
        /// </summary>
        public CnaContainer? Cna { get; set; }

        /// <summary>
        /// ADP (Authorized Data Publisher) 容器列表
        /// </summary>
        public List<AdpContainer>? Adp { get; set; }
    }

    /// <summary>
    /// CNA (CVE Numbering Authority) 容器，包含 CVE 的主要資訊
    /// </summary>
    public class CnaContainer
    {
        /// <summary>
        /// 提供者元資料
        /// </summary>
        public ProviderMetadata? ProviderMetadata { get; set; }

        /// <summary>
        /// CVE 標題
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 問題類型列表，如 CWE 分類
        /// </summary>
        public List<ProblemType>? ProblemTypes { get; set; }

        /// <summary>
        /// 受影響的產品列表
        /// </summary>
        public List<Affected>? Affected { get; set; }

        /// <summary>
        /// CVE 描述列表
        /// </summary>
        public List<Description>? Descriptions { get; set; }

        /// <summary>
        /// 評分指標列表，如 CVSS 分數
        /// </summary>
        public List<Metric>? Metrics { get; set; }

        /// <summary>
        /// 時間線事件列表
        /// </summary>
        public List<TimelineEntry>? Timeline { get; set; }

        /// <summary>
        /// 貢獻者列表
        /// </summary>
        public List<Credit>? Credits { get; set; }

        /// <summary>
        /// 參考資料列表
        /// </summary>
        public List<Reference>? References { get; set; }
    }

    /// <summary>
    /// 受影響的產品資訊
    /// </summary>
    public class Affected
    {
        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string? Vendor { get; set; }

        /// <summary>
        /// 產品名稱
        /// </summary>
        public string? Product { get; set; }

        /// <summary>
        /// 受影響的版本列表
        /// </summary>
        public List<Version>? Versions { get; set; }

        /// <summary>
        /// 受影響的模組列表
        /// </summary>
        public List<string>? Modules { get; set; }
    }

    /// <summary>
    /// 版本資訊
    /// </summary>
    public class Version
    {
        /// <summary>
        /// 版本值
        /// </summary>
        [JsonProperty("version")]
        public string? VersionValue { get; set; }

        /// <summary>
        /// 版本狀態，如 "affected" 或 "unaffected"
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// 小於等於的版本範圍
        /// </summary>
        public string? LessThanOrEqual { get; set; }

        /// <summary>
        /// 版本類型
        /// </summary>
        public string? VersionType { get; set; }
    }

    /// <summary>
    /// ADP (Authorized Data Publisher) 容器
    /// </summary>
    public class AdpContainer
    {
        /// <summary>
        /// ADP 提供的標題
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// ADP 提供的評分指標列表
        /// </summary>
        public List<AdpMetric>? Metrics { get; set; }

        /// <summary>
        /// ADP 提供者元資料
        /// </summary>
        public ProviderMetadata? ProviderMetadata { get; set; }
    }

    /// <summary>
    /// 提供者元資料
    /// </summary>
    public class ProviderMetadata
    {
        /// <summary>
        /// 組織 ID
        /// </summary>
        public string? OrgId { get; set; }

        /// <summary>
        /// 組織簡稱
        /// </summary>
        public string? ShortName { get; set; }

        /// <summary>
        /// 最後更新日期
        /// </summary>
        public DateTime? DateUpdated { get; set; }
    }

    /// <summary>
    /// 核心 CVE 記錄資訊
    /// </summary>
    public class CveRecord
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// CVE 標題
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 公開日期
        /// </summary>
        public DateTime? DatePublished { get; set; }

        /// <summary>
        /// 保留日期
        /// </summary>
        public DateTime? DateReserved { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? DateUpdated { get; set; }

        /// <summary>
        /// 公開披露日期
        /// </summary>
        public DateTime? DatePublic { get; set; }

        /// <summary>
        /// 拒絕日期
        /// </summary>
        public DateTime? DateRejected { get; set; }

        /// <summary>
        /// 分配機構的組織 ID
        /// </summary>
        public string? AssignerOrgId { get; set; }

        /// <summary>
        /// 分配機構的簡稱
        /// </summary>
        public string? AssignerShortName { get; set; }

        /// <summary>
        /// CVE 記錄狀態
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// 漏洞發現方式
        /// </summary>
        public string? Discovery { get; set; }
    }

    /// <summary>
    /// 受影響的產品詳細資訊
    /// </summary>
    public class CveAffectedProduct
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string? Vendor { get; set; }

        /// <summary>
        /// 產品名稱
        /// </summary>
        public string? Product { get; set; }

        /// <summary>
        /// 預設狀態
        /// </summary>
        public string? DefaultStatus { get; set; }

        /// <summary>
        /// 程式碼儲存庫
        /// </summary>
        public string? Repo { get; set; }

        /// <summary>
        /// CPE (Common Platform Enumeration) 列表
        /// </summary>
        public List<string>? Cpes { get; set; }

        /// <summary>
        /// 集合 URL
        /// </summary>
        public string? CollectionUrl { get; set; }

        /// <summary>
        /// 套件名稱
        /// </summary>
        public string? PackageName { get; set; }

        /// <summary>
        /// 模組列表
        /// </summary>
        public List<string>? Modules { get; set; }
    }

    /// <summary>
    /// 版本詳細資訊
    /// </summary>
    public class CveVersion
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 產品名稱
        /// </summary>
        public string? Product { get; set; }

        /// <summary>
        /// 版本號
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 版本狀態
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// 小於的版本範圍
        /// </summary>
        public string? LessThan { get; set; }

        /// <summary>
        /// 小於等於的版本範圍
        /// </summary>
        public string? LessThanOrEqual { get; set; }

        /// <summary>
        /// 版本類型
        /// </summary>
        public string? VersionType { get; set; }

        /// <summary>
        /// 變更時間
        /// </summary>
        public string? ChangeAt { get; set; }

        /// <summary>
        /// 變更狀態
        /// </summary>
        public string? ChangeStatus { get; set; }
    }

    /// <summary>
    /// CVE 相關模組
    /// </summary>
    public class CveModule
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 模組名稱
        /// </summary>
        public string? ModuleName { get; set; }
    }

    /// <summary>
    /// CVE 描述
    /// </summary>
    public class Description
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 描述語言
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// 描述文字內容
        /// </summary>
        [JsonProperty("value")]
        public string? DescriptionText { get; set; }

        /// <summary>
        /// 支援媒體列表
        /// </summary>
        public List<CveSupportingMedia>? SupportingMedia { get; set; }
    }

    /// <summary>
    /// CVE 支援媒體
    /// </summary>
    public class CveSupportingMedia
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 媒體語言
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// 媒體類型
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 是否為 Base64 編碼
        /// </summary>
        public bool Base64 { get; set; }

        /// <summary>
        /// 媒體內容值
        /// </summary>
        public string? Value { get; set; }
    }

    /// <summary>
    /// CVSS 評分資訊
    /// </summary>
    public class CveCvssScore
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 評分格式
        /// </summary>
        public string? Format { get; set; }

        /// <summary>
        /// 評分情境
        /// </summary>
        public string? Scenario { get; set; }

        /// <summary>
        /// CVSS 版本
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 基本分數
        /// </summary>
        public double BaseScore { get; set; }

        /// <summary>
        /// 基本嚴重性等級
        /// </summary>
        public string? BaseSeverity { get; set; }

        /// <summary>
        /// 向量字串
        /// </summary>
        public string? VectorString { get; set; }

        /// <summary>
        /// 攻擊途徑
        /// </summary>
        public string? AttackVector { get; set; }

        /// <summary>
        /// 攻擊複雜度
        /// </summary>
        public string? AttackComplexity { get; set; }

        /// <summary>
        /// 所需權限
        /// </summary>
        public string? PrivilegesRequired { get; set; }

        /// <summary>
        /// 使用者互動
        /// </summary>
        public string? UserInteraction { get; set; }

        /// <summary>
        /// 影響範圍
        /// </summary>
        public string? Scope { get; set; }

        /// <summary>
        /// 機密性影響
        /// </summary>
        public string? ConfidentialityImpact { get; set; }

        /// <summary>
        /// 完整性影響
        /// </summary>
        public string? IntegrityImpact { get; set; }

        /// <summary>
        /// 可用性影響
        /// </summary>
        public string? AvailabilityImpact { get; set; }

        /// <summary>
        /// 是否可自動化利用
        /// </summary>
        public string? Automatable { get; set; }

        /// <summary>
        /// 復原難度
        /// </summary>
        public string? Recovery { get; set; }

        /// <summary>
        /// 安全性影響
        /// </summary>
        public string? Safety { get; set; }

        /// <summary>
        /// 攻擊需求
        /// </summary>
        public string? AttackRequirements { get; set; }

        /// <summary>
        /// 提供者緊急程度
        /// </summary>
        public string? ProviderUrgency { get; set; }

        /// <summary>
        /// 次要機密性影響
        /// </summary>
        public string? SubConfidentialityImpact { get; set; }

        /// <summary>
        /// 次要完整性影響
        /// </summary>
        public string? SubIntegrityImpact { get; set; }

        /// <summary>
        /// 次要可用性影響
        /// </summary>
        public string? SubAvailabilityImpact { get; set; }

        /// <summary>
        /// 價值密度
        /// </summary>
        public string? ValueDensity { get; set; }

        /// <summary>
        /// 漏洞回應所需努力
        /// </summary>
        public string? VulnerabilityResponseEffort { get; set; }
    }

    /// <summary>
    /// 問題類型
    /// </summary>
    public class ProblemType
    {
        /// <summary>
        /// 問題類型描述列表
        /// </summary>
        public List<ProblemTypeDescription>? Descriptions { get; set; }
    }

    /// <summary>
    /// 問題類型描述
    /// </summary>
    public class ProblemTypeDescription
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// CWE 識別碼，如 CWE-79
        /// </summary>
        public string? CweId { get; set; }

        /// <summary>
        /// 問題描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 描述語言
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// 描述類型
        /// </summary>
        public string? Type { get; set; }
    }

    /// <summary>
    /// CNA 中的評分指標
    /// </summary>
    public class Metric
    {
        /// <summary>
        /// CVSS v4.0 評分
        /// </summary>
        public CvssV4_0? CvssV4_0 { get; set; }

        /// <summary>
        /// CVSS v3.1 評分
        /// </summary>
        public CvssV3_1? CvssV3_1 { get; set; }

        /// <summary>
        /// CVSS v3.0 評分
        /// </summary>
        public CvssV3_0? CvssV3_0 { get; set; }

        /// <summary>
        /// CVSS v2.0 評分
        /// </summary>
        public CvssV2_0? CvssV2_0 { get; set; }
    }

    /// <summary>
    /// CVSS v4.0 評分
    /// </summary>
    public class CvssV4_0
    {
        /// <summary>
        /// CVSS 版本
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 基本分數
        /// </summary>
        public double BaseScore { get; set; }

        /// <summary>
        /// 向量字串
        /// </summary>
        public string? VectorString { get; set; }

        /// <summary>
        /// 基本嚴重性等級
        /// </summary>
        public string? BaseSeverity { get; set; }
    }

    /// <summary>
    /// CVSS v3.1 評分
    /// </summary>
    public class CvssV3_1
    {
        /// <summary>
        /// CVSS 版本
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 基本分數
        /// </summary>
        public double BaseScore { get; set; }

        /// <summary>
        /// 向量字串
        /// </summary>
        public string? VectorString { get; set; }

        /// <summary>
        /// 基本嚴重性等級
        /// </summary>
        public string? BaseSeverity { get; set; }
    }

    /// <summary>
    /// CVSS v3.0 評分
    /// </summary>
    public class CvssV3_0
    {
        /// <summary>
        /// CVSS 版本
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 基本分數
        /// </summary>
        public double BaseScore { get; set; }

        /// <summary>
        /// 向量字串
        /// </summary>
        public string? VectorString { get; set; }

        /// <summary>
        /// 基本嚴重性等級
        /// </summary>
        public string? BaseSeverity { get; set; }
    }

    /// <summary>
    /// CVSS v2.0 評分
    /// </summary>
    public class CvssV2_0
    {
        /// <summary>
        /// CVSS 版本
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 基本分數
        /// </summary>
        public double BaseScore { get; set; }

        /// <summary>
        /// 向量字串
        /// </summary>
        public string? VectorString { get; set; }
    }

    /// <summary>
    /// ADP 中的評分指標
    /// </summary>
    public class AdpMetric
    {
        /// <summary>
        /// SSVC (Stakeholder-Specific Vulnerability Categorization) 評分
        /// </summary>
        public Ssvc? Other { get; set; }
    }

    /// <summary>
    /// SSVC 評分
    /// </summary>
    public class Ssvc
    {
        /// <summary>
        /// SSVC 類型
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// SSVC 內容
        /// </summary>
        public SsvcContent? Content { get; set; }
    }

    /// <summary>
    /// SSVC 內容
    /// </summary>
    public class SsvcContent
    {
        /// <summary>
        /// SSVC 識別碼
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 時間戳記
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// SSVC 選項列表
        /// </summary>
        public List<SsvcOption>? Options { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// SSVC 版本
        /// </summary>
        public string? Version { get; set; }
    }

    /// <summary>
    /// SSVC 選項
    /// </summary>
    public class SsvcOption
    {
        /// <summary>
        /// 利用狀態
        /// </summary>
        public string? Exploitation { get; set; }

        /// <summary>
        /// 是否可自動化利用
        /// </summary>
        public string? Automatable { get; set; }

        /// <summary>
        /// 技術影響
        /// </summary>
        [JsonProperty("Technical Impact")]
        public string? TechnicalImpact { get; set; }
    }

    /// <summary>
    /// 時間線事件
    /// </summary>
    public class TimelineEntry
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 事件時間
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 事件描述語言
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// 事件描述內容
        /// </summary>
        public string? Value { get; set; }
    }

    /// <summary>
    /// 貢獻者資訊
    /// </summary>
    public class Credit
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 貢獻者描述語言
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// 貢獻類型
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 貢獻者資訊
        /// </summary>
        public string? Value { get; set; }
    }

    /// <summary>
    /// 參考資料
    /// </summary>
    public class Reference
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 參考資料 URL
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 參考資料名稱
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 參考資料標籤列表
        /// </summary>
        public List<string>? Tags { get; set; }
    }

    /// <summary>
    /// CVE 平台資訊（保留以供未來擴展）
    /// </summary>
    public class CvePlatform
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 平台名稱
        /// </summary>
        public string? Platform { get; set; }
    }

    /// <summary>
    /// CVE 程式檔案資訊（保留以供未來擴展）
    /// </summary>
    public class CveProgramFile
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 檔案路徑
        /// </summary>
        public string? FilePath { get; set; }
    }

    /// <summary>
    /// CVE 程式例程資訊（保留以供未來擴展）
    /// </summary>
    public class CveProgramRoutine
    {
        /// <summary>
        /// CVE 識別碼
        /// </summary>
        public string? CveId { get; set; }

        /// <summary>
        /// 例程名稱
        /// </summary>
        public string? RoutineName { get; set; }
    }
}
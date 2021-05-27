using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum PosKInd
    {
        [Description("일반")]
        Normal,
        [Description("밴딩머신")]
        VendingMachine,
        [Description("무인점포")]
        SelfServiceStore,
        [Description("대리점POS")]
        AgencyPos,
        [Description("SISPOS")]
        SisPos,
    }

    public enum DeviceType
    {
        POS,        // POS
        Tablet,     // Tablet
        Mobile      // 모바일
    }

    public enum LanguageType
    {
        None,
        Kor,
        Eng,
        Jp,
        Chi,
    }

    public enum PosStatus
    {
        NotSet,
        NeedOpen,
        Opened,
        EndOfSale,
        ReSale,
    }

    public enum SaleMode
    {
        [Description("정상")]
        Normal = 0,                      // 정상
        [Description("현금전용")]
        CashOnlyPOS = 1,                  // 현금전용
        [Description("연습모드")]
        Train = 2,                       // 테스트

    }

    public enum StoreKind
    {
        Normal,
        GentleMonster,
        Tamburins,
        FnB,
    }

    public enum StoreType
    {
        [Description("직영")]
        DirectManagementStore = 0,

        [Description("임대")]
        RentStore,

        [Description("대리점")]
        AgencyStore,

        [Description("가맹점")]
        FranchiseStore,
    }


    public enum StoreBridge
    {
        [Description("대내")]
        Internal,
        [Description("대외")]
        External

    }

    public enum TenantChannelType
    {
        [Description("직영")]
        DirectManagementStore,

        [Description("백화점")]
        DepartmentStore,

        [Description("면세")]
        TaxFreeStore,

        [Description("대리점(SIS)")]
        SISAgencyStore,
    }

    public enum TenantSaleType
    {
        [Description("매출")]
        SaleShop,
        [Description("비매출")]
        NoneSaleShop
    }

    public enum VCatType
    {
        VCat7430,
        VCat3410,
    }

    public class ConfigModel
    {
        [JsonProperty("ICMCD")]
        [Description("회사코드(상위)")]
        public string InterCompanyCode { get; set; } = string.Empty;
        [JsonProperty("CMCD")]
        [Description("회사코드(하위)")]
        public string CompanyCode { get; set; } = string.Empty;
        [JsonProperty("STCD")]
        [Description("점포코드")]
        public string StoreCode { get; set; } = string.Empty;
        [JsonProperty("STNM")]
        [Description("점포명")]
        public string StoreName { get; set; } = string.Empty;
        [JsonProperty("STTEL")]
        [Description("점포전화번호")]
        public string StoreTel { get; set; } = string.Empty;
        [JsonProperty("STADDRSS")]
        [Description("점포주소")]
        public string StoreAddress { get; set; } = string.Empty;
        [JsonProperty("STZIPCODE")]
        [Description("점포우편번호")]
        public string StoreZipCode { get; set; } = string.Empty;
        [JsonProperty("TNCD")]
        [Description("매장코드")]
        public string TenantCode { get; set; } = string.Empty;
        [JsonProperty("PSNO")]
        [Description("포스번호")]
        public string PosNo { get; set; } = string.Empty;
        [JsonProperty]
        [Description("프린터명")]
        public string PrinterName { get; set; } = string.Empty;
        [JsonProperty]
        [Description("스캐너명")]
        public string ScannerName { get; set; } = string.Empty;
        [Description("포스구분")]
        public PosKInd PosKind { get; set; } = PosKInd.Normal;

        [JsonProperty]
        [Description("KTC S/W 인증번호")]
        public string KTCSoftwareAuthNo { get; set; } = string.Empty;

        [JsonProperty]
        [Description("KTC H/W 인증번호")]
        public string KTCTerminalAuthNo { get; set; } = string.Empty;

        [JsonProperty]
        [Description("로그인한 캐셔번호(비정상 종료시 캐셔오프 용도)")]
        public string LastCashierNo { get; set; } = string.Empty;

        [JsonProperty]
        [Description("메인URL")]
        public string MainUrl { get; set; } = string.Empty;
        [JsonProperty]
        [Description("서브URL")]
        public string SubUrl { get; set; } = string.Empty;
        [JsonProperty]
        [Description("교육URL")]
        public string TrainUrl { get; set; } = string.Empty;
        [JsonProperty]
        [Description("교육모드여부")]
        public bool IsTrain { get; set; } = false;

        // 중요 : 기동시 저장만 한다.
        [JsonIgnore]
        [Description("캐셔번호")]
        public string CashierID { get; set; } = string.Empty;

        // 중요 : 기동시 저장만 한다.
        [JsonProperty]
        [Description("영업일자")]
        public string SaleDate { get; set; } = string.Empty;

        // 중요 : 거래시마다 저장만 한다.
        [JsonIgnore]
        [Description("거래번호")]
        public string TranNo { get; set; } = string.Empty;

        // 중요 : 시연용 고정처리 상품탭으로 자동 이동할지 여부
        public bool IsGoodsTabAutoSwitch = true;

        public PosStatus PosStatus { get; set; } = PosStatus.NotSet;

        /// <summary>
        /// 클라이언트버전
        /// </summary>
        public string ClientVersion { get; set; } = string.Empty;
        // 다운로드 설명
        public string Description { get; set; } = string.Empty;

        public string GET_TRADE_UNIQUE
        {
            get => $"{CompanyCode.Trim()}{StoreCode.Trim()}{PosNo.Trim()}";
        }

        #region 카드리더기
        [JsonProperty]
        [Description("서명패드 포트")]
        public string SignPadPort { get; set; } = string.Empty;

        [JsonProperty]
        [Description("서명패드 속도")]
        public string SignPadBaudRate { get; set; } = "57600";

        [JsonProperty]
        [Description("동글 포트")]
        public string DonglePort { get; set; } = string.Empty;

        [JsonProperty]
        [Description("동글 BaudRate")]
        public string DongleBaudRate { get; set; } = "115200";

        [JsonProperty]
        [Description("동글종류")]
        public string DongleKind { get; set; } = "KIS_EDI";
        #endregion
    }
}

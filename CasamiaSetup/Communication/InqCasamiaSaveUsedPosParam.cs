using CasamiaSetup.Communication.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasamiaSetup.Communication
{
    public class InqCasamiaSaveUsedPosParam// : InquiryBase
    {
        [JsonProperty]
        public string StoreCode { get; set; } = string.Empty;

        [JsonProperty]
        public List<string> PosNoList { get; set; } = new List<string>();

        [JsonProperty]
        public string StoreName { get; set; } = string.Empty;

        [JsonProperty]
        public string InstallationDate { get; set; } = string.Empty;

        [JsonProperty]
        public string StoreType { get; set; } = string.Empty;

        [JsonProperty]
        public string DemolitionDate { get; set; } = string.Empty;

        [JsonProperty]
        public string Description { get; set; } = string.Empty;
    }
}

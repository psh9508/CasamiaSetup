using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasamiaSetup.Communication.Base
{
    [JsonObject]
    public class InquiryBase
    {
        [Description("POS정보")]
        [JsonProperty]
        public PosConfig PosConfig { get; set; }

        [Description("POS IP 어드레스")]
        [JsonProperty]
        public string ClientIPAddress { get; set; }

        [Description("전송 Token")]
        [JsonIgnore]
        public string ClientToken { get; set; }

        private Guid? senduniqueCode = null;
        [Description("전송 고유번호")]
        [JsonProperty]
        public Guid SendUniqueCode
        {
            get
            {
                if (senduniqueCode.HasValue == false)
                    senduniqueCode = Guid.NewGuid();

                return senduniqueCode.Value;
            }
            set
            {
                senduniqueCode = value;
            }
        }

        [Description("API버전")]
        [JsonProperty]
        public string Version { get; set; } = "1.0";
    }
}

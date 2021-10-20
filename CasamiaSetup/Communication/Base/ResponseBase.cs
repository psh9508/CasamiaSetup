using CasamiaSetup.Communication.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasamiaSetup.Communication.Base
{
    [JsonObject]
    public class ResponseBase
    {
        [Description("응답코드")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageCode MessageCode { get; set; } = MessageCode.UNKNOWN;

        [Description("응답코드(번호)")]
        [JsonProperty]
        public long MessageCodeNo { get; set; }

        [Description("응답메시지")]
        [JsonProperty]
        public string Message { get; set; } = string.Empty;

        private Guid? recvuniqueCode = null;
        [Description("수신 고유번호")]
        [JsonProperty]
        public Guid RecvUniqueCode
        {
            get
            {
                if (recvuniqueCode.HasValue == false)
                    recvuniqueCode = Guid.NewGuid();

                return recvuniqueCode.Value;
            }
            set
            {
                recvuniqueCode = value;
            }
        }

    }
}

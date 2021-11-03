using CasamiaSetup.Communication.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasamiaSetup.Communication
{
    public class InqCasamiaUsedPosParam// : InquiryBase
    {
        /// <summary>
        /// 설치 하려는 매장코드
        /// </summary>
        [JsonProperty]
        public string StoreCode { get; set; } = string.Empty;

        /// <summary>
        /// 설치 하려는 포스번호
        /// </summary>
        [JsonProperty]
        public string PosNo { get; set; } = string.Empty;
    }
}

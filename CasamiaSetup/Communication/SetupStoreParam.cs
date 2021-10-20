using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasamiaSetup.Communication
{
    [JsonObject]
    public class SetupStoreParam
    {
        /// <summary>
        /// 매장번호
        /// </summary>
        [JsonProperty]
        public string StoreCode { get; set; } = string.Empty;

        /// <summary>
        /// 포스번호
        /// </summary>
        [JsonProperty]
        public int PosNo { get; set; }
    }
}

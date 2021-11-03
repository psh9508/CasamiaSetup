using CasamiaSetup.Communication.Base;
using CasamiaSetup.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using Newtonsoft.Json;

namespace CasamiaSetup.Http
{
    public class HttpDataSender
    {
        private const string BASE_URL = "http://10.253.12.20";
        private static readonly HttpClient _httpClient;

        static HttpDataSender()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
        }

        public async Task<TResult> SendPostAsync<TResult, TParam>(string url, TParam param) where TResult : ResponseBase
                                                                                            where TParam : class
        {
            try
            {
                var jsonParam = JsonConvert.SerializeObject(param);
                var content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{BASE_URL}{url}", content);

                if(response.IsSuccessStatusCode == false)
                {
                    return (TResult)new ResponseBase() {
                        Message = "서버에서 응답값을 가져오는데 실패 했습니다.",
                    };
                }

                var responseString = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TResult>(responseString);
            }
            catch (Exception ex)
            {
                Logger.WriteError($"Post 실패 : {url}");
                Logger.WriteError($"{ex.Message}");
                Logger.WriteError($"{ex.StackTrace}");

                throw;
            }           
        }

        public async Task<TResult> SendGetAsync<TResult>(string url) where TResult : class
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}{url}");
                var responseString = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<TResult>(responseString);
            }
            catch (Exception ex)
            {
                Logger.WriteError($"Post 실패 : {url}");
                Logger.WriteError($"{ex.Message}");
                Logger.WriteError($"{ex.StackTrace}");

                throw;
            }
        }

        public void SetHeader(string name, string value)
        {
            _httpClient.DefaultRequestHeaders.Add("pos-api-token", value);
        }
    }
}

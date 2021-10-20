using CasamiaSetup.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
//using Newtonsoft.Json;

namespace CasamiaSetup.Http
{
    public class HttpDataSender
    {
        private const string BASE_URL = "http://10.253.12.20";
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<TResult> SendPost<TResult, TParam>(string url, TParam param) where TResult : class 
                                                                                       where TParam : class
        {
            try
            {
                var content = new FormUrlEncodedContent(param.ToDictionary());
                var response = await _httpClient.PostAsync(url, content);
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

        public async Task<TResult> SendGet<TResult>(string url) where TResult : class
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}url");
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

using CasamiaSetup.Communication;
using CasamiaSetup.Communication.Base;
using CasamiaSetup.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasamiaSetup
{
    public interface IUsedPosService
    {
        Task SetJWTTokenAsync();
        Task<bool> HasPosAsync(InqCasamiaUsedPosParam param);
        Task<bool> SavePos(InqCasamiaSaveUsedPosParam param);
    }

    public class SetupService : IUsedPosService
    {
        private static readonly HttpDataSender _httpDataSender = new HttpDataSender();

        public async Task SetJWTTokenAsync()
        {
            const string systemCode = @"CasamiaSetup";
            const string terminalCode = @"9999";

            var response = await _httpDataSender.SendGetAsync<ResponseBase>($@"/CloudPOS.Service.Casamia.Authentication/Authentication.svc/GetSecureToken/{systemCode}/{terminalCode}");

            if (response.MessageCode != Communication.Enum.MessageCode.SUCCESS)
                throw new System.Security.SecurityException("JWT 토큰 발급에 실패 했습니다.");

            _httpDataSender.SetHeader("pos-api-token", response.Message);
        }

        public async Task<bool> HasPosAsync(InqCasamiaUsedPosParam param)
        {
            var response = await _httpDataSender.SendPostAsync<InqCasamiaUsedPosResponse, InqCasamiaUsedPosParam>($@"/CloudPOS.Service.Casamia.Inquiry/Inquiry.svc/InquireUsedStore", param);

            if (response.MessageCode != Communication.Enum.MessageCode.SUCCESS)
                return false;

            return response?.HasPos == true;
        }

        public async Task<bool> SavePos(InqCasamiaSaveUsedPosParam param)
        {
            var response = await _httpDataSender.SendPostAsync<InqCasamiaSaveUsedPosResponse, InqCasamiaSaveUsedPosParam>(@"/CloudPOS.Service.Casamia.Inquiry/Inquiry.svc/SaveUsedStore", param);

            if (response.MessageCode != Communication.Enum.MessageCode.SUCCESS)
                return false;

            return true;
        }
    }
}

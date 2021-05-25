using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CasamiaSetup
{
    public interface IDonglePortFinder
    {
        /// <summary>
        /// 연결된 포트 번호를 반환한다.
        /// </summary>
        /// <returns>포트 번호 or -1(연결된 포트 찾기 실패)</returns>
        Task<int> GetConnectedPortAsync();
    }

    public class DonglePortFinder : IDonglePortFinder
    {
        [DllImport("KisDongleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int Dongle_Init();

        [DllImport("KisDongleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static void Dongle_Release();

        [DllImport("KisDongleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static void Dongle_Stop();

        [DllImport("KisDongleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int Dongle_Wait(int nType, int nTimeout);

        [DllImport("KisDongleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int Dongle_Approval(int nPortNo, int nBaudRate, string sCommandID, string inSendData, int nSendDataLen);

        [DllImport("KisDongleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int Dongle_State();

        public async Task<int> GetConnectedPortAsync()
        {
            const int FAILED = -1;

            try
            {
                Logger.Write($"{this.GetType().Name} 시작");

                var ports = SerialPort.GetPortNames().Where(x => x.Contains("COM")).Select(x => Convert.ToInt32(x.Replace("COM", "")));

                foreach (var port in ports)
                {
                    Logger.Write($"연결 시도 port : {port}");

                    bool closeResult = await CloseAsync();

                    if (closeResult == false)
                    {
                        Logger.Write($"close 실패");
                        continue; // 맞나?
                    }

                    bool openResult = await TryConnectAsync(port, 115200);

                    if (openResult)
                    {
                        Logger.Write($"찾은 port : {port}");
                        return port;
                    }
                }

                Logger.Write($"연결 된 포트를 찾이 못했습니다.");
                return FAILED;
            }
            catch (Exception ex)
            {
                Logger.WriteError($"연결된 포트를 찾는 중 예상하지 못한 에러 발생!! [{ex}]");

                return FAILED;
            }
        }

        private async Task<bool> CloseAsync()
        {
            try
            {
                Logger.Write($"{this.GetType().Name} 실행");

                return await Task.Run(() =>
                {
                    Dongle_Stop();
                    Dongle_Release();

                    Logger.Write($"CloseAsync 성공");
                    return true;
                });
            }
            catch (Exception ex)
            {
                Logger.WriteError($"[{this.GetType().Name}] {ex}");

                return false;
            }
        }

        //public bool ReqcmdASYNC(int nPortNo, int nBaudRate, string inSendData, int nSendDataLen)
        public async Task<bool> TryConnectAsync(int nPortNo, int nBaudRate)
        {
            const int SUCCESS = 0;

            try
            {
                Logger.Write($"{this.GetType().Name} 실행");

                return await Task.Run(() =>
                {
                    int nRet = 0;

                    nRet = Dongle_Init();

                    if (nRet != SUCCESS)
                    {
                        Logger.Write("Dongle_Init 실패");
                        return false;
                    }

                    //nRet = Dongle_Approval(nPortNo, nBaudRate, "31", inSendData, nSendDataLen);
                    nRet = Dongle_Approval(nPortNo, nBaudRate, "31", "", 0);

                    if (nRet != SUCCESS)
                    {
                        Logger.Write("Dongle_Approval 실패");
                        return false;
                    }

                    return true;
                });
            }
            catch (Exception ex)
            {
                Logger.WriteError($"[{this.GetType().Name}] {ex}");

                return false;
            }
            finally
            {
                Dongle_Release();
            }
        }
    }
}

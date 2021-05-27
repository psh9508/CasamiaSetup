using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace DongleSetup
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
        private enum STATE_CODE { STATE_INIT, STATE_REQ, STATE_WAIT, STATE_RECV, STATE_ERROR, STATE_CANCEL, STATE_TIMEOUT };

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

        [DllImport("isDongleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int Dongle_State();

        [DllImport("KisDongleDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int Dongle_GetData([MarshalAs(UnmanagedType.LPArray)] byte[] outRecvData);

        private const int SUCCESS = 0;

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

                    bool openResult = await TryConnectAsync(port, "31");

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

        public async Task<bool> TryConnectAsync(int portNo, string command)
        {
            try
            {
                Logger.Write($"{this.GetType().Name} 실행");

                return await Task.Run(() =>
                {
                    int nRet = 0;

                    nRet = Dongle_Init();

                    if (nRet != SUCCESS)
                    {
                        Dongle_Release(); // 한번 더 해제

                        nRet = Dongle_Init();

                        if (nRet != SUCCESS)
                        {
                            Logger.Write("Dongle_Init 실패");
                            return false;
                        }
                    }

                    nRet = Dongle_Approval(portNo, 115200, command, "", 0);

                    if (nRet != SUCCESS)
                    {
                        Logger.Write("Dongle_Approval 실패");
                        return false;
                    }

                    if (command == "1B")
                    {
                        Logger.Write("1B라서 return");
                        return true;
                    }

                    int state = 0;

                    Dongle_Wait(1, 30000);

                    // 무한루프 안도나?
                    while (true)
                    {
                        Dongle_Approval(portNo, 115200, "1B", "", 0);

                        Dongle_Stop();
                        Dongle_Release();

                        state = Dongle_State();

                        if (state >= (int)STATE_CODE.STATE_RECV)
                        {
                            Logger.Write("break");
                            break;
                        }
                    }

                    Logger.Write($"state == STATE_RECV? [{state == (int)STATE_CODE.STATE_RECV}]");

                    if (state != (int)STATE_CODE.STATE_RECV)
                    {
                        Logger.Write("state != (int)STATE_CODE.STATE_RECV");
                        return false;
                    }

                    byte[] recvData = new byte[2048];
                    int recvDataLength = Dongle_GetData(recvData);

                    var outByte = new byte[recvData.Length];
                    recvData.CopyTo(outByte, 0);
                    Array.Resize(ref outByte, recvDataLength);

                    if (recvDataLength < 2)
                    {
                        Logger.Write("recvDataLength < 2");
                        return false;
                    }

                    const string GET_DATA_SUCCESS = "00";
                    string outCode = Encoding.Default.GetString(recvData, 0, 2);

                    if (outCode != GET_DATA_SUCCESS)
                    {
                        Logger.Write($"outCode != SUCCESS [{outCode}]");
                        return false;
                    }

                    var outString = Encoding.Default.GetString(recvData, 0, recvDataLength);
                    var serialNo = outString?.Substring(2, 10);

                    Logger.Write($"serialNo = {serialNo}");

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

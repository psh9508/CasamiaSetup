using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CasamiaSetup
{
    /// <summary>
    /// InitWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InitWindow : Window
    {
        private string _msg = string.Empty;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public InitWindow()
        {
            InitializeComponent();
        }

        private async void Start(object sender, EventArgs e)
        {
            StartMessageUpdateThread(_cancellationTokenSource);

            await CreateSetupFolderAsync();
            await CopyResourceFilesAsync();
            await ExecuteSetupFileAsync();
            await ExecuteOPOSFileAsync();
            await SetPosPrinterAsync();
            Show정보입력View();

            await Task.Delay(TimeSpan.FromSeconds(1));

            //MessageBox.Show("설치를 완료 했습니다.");
            Logger.Write("설치 완료");

            await OpenCasamiaPOSAsync();

            _cancellationTokenSource.Cancel();
            this.Close();
        }

        private async Task CreateSetupFolderAsync()
        {
            _msg = "설치 폴더를 만들고 있습니다.";

            await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3));

                var dirInfo = new DirectoryInfo(Constants.SETUP_PATH);

                if (dirInfo.Exists)
                {
                    dirInfo.Delete(true);
                    dirInfo.Refresh();

                    Logger.Write($"{Constants.SETUP_PATH} 삭제");

                    while (dirInfo.Exists)
                    {
                        await Task.Delay(100);
                        dirInfo.Refresh();
                    }
                }
            }).ContinueWith(x =>
            {
                if (x.Exception != null)
                    Logger.WriteError(x.ToString());

                Directory.CreateDirectory(Constants.SETUP_PATH);
                Logger.Write($"{Constants.SETUP_PATH} 생성");
            });
        }

        private async Task CopyResourceFilesAsync()
        {
            _msg = "리소스 파일을 복사 중.";

            await Task.Delay(TimeSpan.FromSeconds(3));

            string copyPath = System.IO.Path.Combine(Constants.SETUP_PATH, "CloudPOS_Installer.msi");

            var tasks = new List<Task>()
            {
                CopyResource(@"CasamiaSetup.Resources.OPOSCashDrawer.ocx", Constants.CASHDRAWER_OCX_PATH),
                CopyResource(@"CasamiaSetup.Resources.OPOSPOSPrinter.ocx", Constants.PRINTER_OCX_PATH),
                CopyResource(@"CasamiaSetup.Resources.CloudPOS_Installer.msi", copyPath),
            };

            await Task.WhenAll(tasks.ToArray());

            //await CopyResource(@"CasamiaSetup.Resources.OPOSCashDrawer.ocx", Constants.CASHDRAWER_OCX_PATH);
            //await CopyResource(@"CasamiaSetup.Resources.OPOSPOSPrinter.ocx", Constants.PRINTER_OCX_PATH);

            //string copyPath = System.IO.Path.Combine(Constants.SETUP_PATH, "CloudPOS_Installer.msi");
            //await CopyResource(@"CasamiaSetup.Resources.CloudPOS_Installer.msi", copyPath);
        }

        private async Task ExecuteSetupFileAsync()
        {
            _msg = "기본 설치 파일을 실행 합니다.";

            await Task.Delay(TimeSpan.FromSeconds(3));

            await Task.Run(() =>
            {
                string msiPath = System.IO.Path.Combine(Constants.SETUP_PATH, "CloudPOS_Installer.msi");

                if(!File.Exists(msiPath))
                {
                    Logger.Write($"[{msiPath}]가 존재하지 않습니다.");
                    return;
                }

                Logger.Write($"기본설치파일을 실행합니다.");
                Process.Start(msiPath, null).WaitForExit();
                Logger.Write($"기본설치파일을 종료");
            });
        }

        private async Task ExecuteOPOSFileAsync()
        {
            _msg = "POSForDotNet을 설치 합니다.";

            await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3));

                if(File.Exists(@"C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.PointOfService\v4.0_1.14.1.0__31bf3856ad364e35\Microsoft.PointOfService.dll"))
                {
                    Logger.Write("POSForDotNet 설치 되어 있음");
                    return;
                }

                string filePath = @"C:\CloudPOS\Temp\POSforDotNet-1.14.msi";

                if (!File.Exists(filePath))
                {
                    Logger.Write($"POSforDotNet 설치에 실패 하였습니다. 이유 : [{filePath}]가 존재하지 않습니다.");
                    return;
                }

                Logger.Write($"POSForDotNet 설치를 실행합니다.");
                Process.Start(filePath, @"/quiet").WaitForExit();
                Logger.Write($"POSForDotNet 설치를 종료");
            });
        }

        private async Task SetPosPrinterAsync()
        {
            _msg = "POS Printer를 설정 합니다.";

            await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3));

                try
                {
                    Logger.Write($"레지스트리 등록을 시작");

                    using (var reg = Registry.LocalMachine.CreateSubKey("SOFTWARE").CreateSubKey("OLEforRetail").CreateSubKey("ServiceOPOS").CreateSubKey("POSPrinter"))
                    {
                        reg.SetValue("POSPrinter", "ThermalU");
                    }

                    using (var reg = Registry.LocalMachine.CreateSubKey("SOFTWARE").CreateSubKey("OLEforRetail").CreateSubKey("ServiceOPOS").CreateSubKey("POSPrinter").CreateSubKey("ThermalU"))
                    {
                        reg.SetValue("", "RecPrinter.POSPrinter.SOU");
                        reg.SetValue("ADKConfig", "Thermal1.0");
                        reg.SetValue("Baudrate", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("baudrateSel", "");
                        reg.SetValue("BitLength", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("BitLenghSel", "");
                        reg.SetValue("Description", "OLE POS Printer OPOS Service Object");
                        reg.SetValue("DeviceDesc", "Thermal POS Printer (USB)");
                        reg.SetValue("DeviceName", "ThermalU");
                        reg.SetValue("DrawerOpen", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("FontType", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("HandShake", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("HandShakeSel", "");
                        reg.SetValue("IdleSleep", 0x0000000a, RegistryValueKind.DWord);
                        reg.SetValue("InputBuf", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("InputSleep", 0x0000000a, RegistryValueKind.DWord);
                        reg.SetValue("IP", "");
                        reg.SetValue("Logo", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("OutputBuf", 0x00000400, RegistryValueKind.DWord);
                        reg.SetValue("Parity", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("ParitySel", "");
                        reg.SetValue("Port", "USB");
                        reg.SetValue("PortShare", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("Stop", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("StopSel", "");
                        reg.SetValue("Timeout", 0x000003e8, RegistryValueKind.DWord);
                        reg.SetValue("USBSerialNumber", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("Version", "1.0");
                        reg.SetValue("XonXoff", 0x00000000, RegistryValueKind.DWord);
                        reg.SetValue("XonXoffSel", "");
                    }

                    Logger.Write($"레지스트리 등록이 완료");
                }
                catch (Exception ex)
                {
                    Logger.WriteError(ex.ToString());

                    string exeFullPath = @"C:\Program Files (x86)\OPOS\StdOPOS2.74\SetupPOS.exe";

                    if (File.Exists(exeFullPath))
                    {
                        MessageBox.Show("레지스트리 등록에 실패했습니다. 직접 입력해주시기 바랍니다.");

                        try
                        {
                            Logger.Write("SetupPOS 실행");
                            Process.Start(exeFullPath).WaitForExit();
                            Logger.Write("SetupPOS 종료");
                        }
                        catch (Exception ex1)
                        {
                            Logger.WriteError("SetupPOS 실행 실패");
                            Logger.WriteError(ex1.ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("레지스트리 등록에 실패했습니다.");
                    }
                }
            });
        }

        private void Show정보입력View()
        {
            _msg = "점포 정보를 입력 받는 중입니다.";

            Logger.Write($"점포 정보 입력 시작");
            new 점포정보입력View() { Owner = this }.ShowDialog();
            Logger.Write($"점포 정보 입력 종료");
        }

        private async Task OpenCasamiaPOSAsync()
        {
            await Task.Run(() =>
            {
                var casamiaPOSFullPath = @"C:\CloudPOS\Start\CloudPOS.Tablet.CasamiaStart.exe";

                if (File.Exists(casamiaPOSFullPath))
                {
                    Logger.Write($"설치 완료 후 까사미아 POS 실행");
                    Process.Start(casamiaPOSFullPath, null);
                }
                else
                {
                    Logger.Write($"[{casamiaPOSFullPath}] 경로 없어 까사미아 POS 실행 실패");
                }
            });
        }

        private async Task CopyResource(string resourcePath, string copyPath)
        {
            await Task.Run(() =>
            {
                using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
                {
                    using (var file = new FileStream(copyPath, FileMode.Create, FileAccess.Write))
                    {
                        Logger.Write($"[{copyPath}]에 리소스를 복사합니다.");
                        resource.CopyTo(file);
                        Logger.Write($"복사 완료");
                    }
                }
            });
        }

        private void StartMessageUpdateThread(CancellationTokenSource tokenSource)
        {   
            Task.Run(() =>
            {   
                tokenSource.Token.ThrowIfCancellationRequested();

                Dispatcher.Invoke(async () =>
                {
                    while (true)
                    {
                        if (tokenSource.Token.IsCancellationRequested)
                            break;

                        _msg += ".";

                        if (_msg.Substring(_msg.Length - 5) == ".....")
                            _msg = _msg.Substring(0, _msg.Length - 5);

                        txtMessage.Text = _msg;

                        await Task.Delay(TimeSpan.FromSeconds(0.2));
                    }
                });
            }, tokenSource.Token);
        }
    }
}

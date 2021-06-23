using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
    /// 점포정보입력View.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class 점포정보입력View : Window
    {
        public 점포정보입력View()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbx점포코드.Text) || string.IsNullOrWhiteSpace(tbx포스번호.Text))
            {
                MessageBox.Show("점포코드와 포스번호가 모두 입력 되어야 합니다.");
                return;
            }

            string directory = @"C:\CloudPOS\Env\";
            string saveFullPath = System.IO.Path.Combine(directory, @"Config.json");

            if (!Directory.Exists(directory))
            {
                Logger.Write($@"[{directory}]경로가 없습니다.");
                Directory.CreateDirectory(directory);
                Logger.Write($@"[{directory}]경로를 만들었습니다.");
            }

            await CopyDeafultConfigFileAsync(saveFullPath);
            RegisterOCX(Constants.CASHDRAWER_OCX_PATH);
            RegisterOCX(Constants.PRINTER_OCX_PATH);

            try
            {
                Logger.Write($@"config 파일 작성 시작");

                var configModel = Deserialize<ConfigModel>(saveFullPath);

                configModel.StoreCode = tbx점포코드.Text;
                configModel.PosNo = tbx포스번호.Text;
                configModel.MainUrl = @"https://cloudposinternal.shinsegae.com";
                configModel.SubUrl = @"https://cloudposinternaldr.shinsegae.com";
                configModel.TrainUrl = @"http://10.253.12.20";
                configModel.PrinterName = "POSPrinter";
                configModel.InterCompanyCode = "0001";
                configModel.CompanyCode = "0001";
                configModel.TenantCode = "0001";
                configModel.DongleBaudRate = "115200";
                configModel.SignPadBaudRate = "57600";
                //configModel.DonglePort = "3";
                //configModel.LanguageType = LanguageType.Kor;
                //configModel.VCatType = VCatType.VCat3410;
                //configModel.StoreKind = StoreKind.GentleMonster;
                //configModel.StoreType = StoreType.DirectManagementStore;
                //configModel.StoreBridge = StoreBridge.External;
                //configModel.TenantSaleType = TenantSaleType.SaleShop;
                //configModel.IsUseCrm = true;
                //configModel.IsProductSerial = true;
                //configModel.SaleMode = SaleMode.Normal;
                //configModel.PosStatus = PosStatus.Opened;

                Save(configModel, saveFullPath);

                Logger.Write($@"config 파일 작성 완료");

                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("설정파일 저장에 실패 했습니다.");
                Logger.WriteError("설정파일 저장에 실패 했습니다.");
                Logger.WriteError(ex.ToString());
            }
        }

        private async Task CopyDeafultConfigFileAsync(string copyPath)
        {
            await Task.Run(() =>
            {
                using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(@"CasamiaSetup.Resources.Config.json"))
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

        public void RegisterOCX(string fullPath)
        {
            if(!File.Exists(fullPath))
            {
                Logger.Write($@"[{fullPath}]에 파일이 없습니다.");
                return;
            }

            try
            {
                Process.Start("regsvr32.exe", string.Format("-s \"{0}\"", fullPath));
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
            }
        }

        private async Task<bool> SetCardMachinePortAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.WriteError($@"카드 리더기 포트 설정에 실패 했습니다. [{ex}]");
                    return false;
                }
            });
        }

        private void Save<T>(T dataModel, string destPath)
        {
            string jsonText = JsonConvert.SerializeObject(dataModel);

            File.WriteAllText(destPath, jsonText);
        }

        private T Deserialize<T>(string path)
        {
            var jsonText = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<T>(jsonText);
        }

        private void tbx포스번호_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(null, null);
            }
        }
    }
}

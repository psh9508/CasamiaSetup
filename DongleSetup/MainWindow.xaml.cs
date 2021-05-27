using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using Common.Extensions;

namespace DongleSetup
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var configModel = Constants.CONFIG_PATH.Deserialize<ConfigModel>();

            var donglePortFinder = new DonglePortFinder();
            int port = donglePortFinder.GetConnectedPortAsync().Result;

            const int FAILED = -1;

            if (port != FAILED)
            {
                configModel.DonglePort = port.ToString();
                configModel.DongleBaudRate = "115200";
                configModel.DongleKind = "KIS_EDI";

                var signpadPorts = SerialPort.GetPortNames().Where(x => x.Contains("COM")).Select(x => Convert.ToInt32(x.Replace("COM", ""))).Where(x => x != port);

                Logger.Write($"signpadPorts = {string.Join(",", signpadPorts)}");

                if (signpadPorts.Count() > 1)
                {
                    var form = new 싸인패드찾기View(signpadPorts);
                    form.Owner = this;

                    form.SelectedPort += signpadPort => {
                        configModel.SignPadPort = signpadPort;
                        configModel.SignPadBaudRate = "57600";
                    };

                    form.ShowDialog();

                    Logger.Write("싸인패드 찾기 View 닫힘");
                }
            }
        }
    }
}

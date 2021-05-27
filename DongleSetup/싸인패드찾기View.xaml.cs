using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Common;

namespace DongleSetup
{
    /// <summary>
    /// 싸인패드찾기View.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class 싸인패드찾기View : Window
    {
        public event Action<string> SelectedPort;

        public 싸인패드찾기View(IEnumerable<int> ports)
        {
            InitializeComponent();

            lstPorts.ItemsSource = ports;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedPort = lstPorts.SelectedItem;

            if (selectedPort == null)
            {
                MessageBox.Show("포트를 선택해 주시기 바랍니다.");
                return;
            }

            Logger.Write($"선택된 사인패드 = {selectedPort}");
            SelectedPort?.Invoke(selectedPort.ToString());

            DialogResult = true;
            this.Close();
        }
    }
}

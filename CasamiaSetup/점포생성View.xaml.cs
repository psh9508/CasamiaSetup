using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace CasamiaSetup
{
    /// <summary>
    /// 점포생성View.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class 점포생성View : Window
    {
        public bool IsProcessing
        {
            get { return (bool)GetValue(IsProcessingProperty); }
            set { SetValue(IsProcessingProperty, value); }
        }

        public static readonly DependencyProperty IsProcessingProperty =
            DependencyProperty.Register("IsProcessing", typeof(bool), typeof(점포생성View), new PropertyMetadata(false));

        private readonly IUsedPosService _usedPosService;
        public 점포생성View()
        {
            InitializeComponent();
            _usedPosService = new SetupService();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsProcessing = true;

                if (ValidateParameters() == false)
                    return;

                var response = await _usedPosService.SavePos(new Communication.InqCasamiaSaveUsedPosParam()
                {
                    StoreCode = tbx점포코드.Text,
                    PosNoList = new List<string>(tbx포스번호.Text.Split(',').Select(x => x.Trim())),
                    StoreName = tbx점포코드.Text,
                    InstallationDate = tbx사용시작일.Text,
                    DemolitionDate = chk만료일.IsChecked == true ? tbx만료일.Text : null,
                    StoreType = chk매장타입.IsChecked == true ? cbo매장타입.Text : string.Empty,
                    Description = chk메모.IsChecked == true ? tbx메모.Text : null,
                });

                if (response == false)
                {
                    MessageBox.Show("실패");
                    return;
                }

                this.Close();
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(tbx점포코드.Text)
                || string.IsNullOrEmpty(tbx포스번호.Text)
                || string.IsNullOrEmpty(tbx점포이름.Text)
                || string.IsNullOrEmpty(tbx사용시작일.Text))
            {
                MessageBox.Show("필수정보가 입력되지 않았습니다.");
                return false;
            }

            if(DateTime.TryParseExact(tbx사용시작일.Text, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime _) == false)
            {
                MessageBox.Show("입력된 사용시작일이 날짜형식이 아닙니다.");
                return false;
            }

            else if(chk만료일.IsChecked == true)
            {
                if(string.IsNullOrEmpty(tbx만료일.Text))
                {
                    MessageBox.Show("만료일이 입력되지 않았습니다.");
                    return false;
                }

                if (DateTime.TryParseExact(tbx만료일.Text, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime _) == false)
                {
                    MessageBox.Show("입력된 만료일이 날짜형식이 아닙니다.");
                    return false;
                }
            }
            else if(chk매장타입.IsChecked == true)
            {
                if(string.IsNullOrEmpty(cbo매장타입.Text))
                {
                    MessageBox.Show("매장타입이 선택되지 않았습니다.");
                    return false;
                }
            }
            else if(chk메모.IsChecked == true)
            {
                if(string.IsNullOrEmpty(tbx메모.Text))
                {
                    MessageBox.Show("메모가 입력되지 않았습니다.");
                    return false;
                }
            }

            return true;
        }
    }
}

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

namespace VIPKS_3
{
    /// <summary>
    /// Логика взаимодействия для ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        ApiService _apiService;
        User _user;
        public ResetPasswordWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            tbPassword1.Focus();
        }

        private async void btnReset_Click(object sender, RoutedEventArgs e)
        {
            string password = tbPassword1.Password == tbPassword2.Password ? tbPassword1.Password : null;

            if (_apiService != null && !string.IsNullOrWhiteSpace(password))
            {
                try
                {
                    _user.UserPassword = password;

                    await _apiService.UpdateUserAsync(_user.UserId, _user);

                    DialogResult = true;
                    Close();
                }
                catch
                {
                    MessageBox.Show("Ошибка сохранения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Введите и подтвердите новый пароль", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _user = await _apiService.GetUserAsync(Data.user.UserId);
        }
    }
}

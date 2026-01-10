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
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        ApiService _apiService;
        public RegisterWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            tbUsername.Focus();
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text;
            string login = tbLogin.Text;
            string password = tbPassword1.Password == tbPassword2.Password ? tbPassword1.Password : null;
            bool admin = rbAdmin.IsChecked == true ? true : false;

            if (_apiService != null && !string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(username))
            {
                await _apiService.RegisterAsync(username, login, password);

                if (admin)
                {
                    await _apiService.ChangeUserRoleAsync(login, 1);       
                }

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Введите логин и пароль", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rbUser.IsChecked = true;
        }
    }
}

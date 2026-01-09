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
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbLogin.Text) && !string.IsNullOrWhiteSpace(tbPassword.Password))
            {
                Login(tbLogin.Text, tbPassword.Password);
            }
            else
            {
                MessageBox.Show("Введите логин и пароль", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void Login(string login, string password)
        {
            using (ApiService apiService = new ApiService())
            {
                bool loginResult = await apiService.LoginAsync(login, password);
                if (loginResult)
                {
                    MainWindow mainWindow = new();
                    mainWindow.Show();
                    mainWindow.Focus();
                    Close();
                }
                else MessageBox.Show("Неправильный логин или пароль", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbLogin.Focus();
        }
    }
}

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
using VIPKS_3.Classes;

namespace VIPKS_3
{
    /// <summary>
    /// Логика взаимодействия для EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        ApiService _apiService;
        User _user;
        public EditUserWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            tbUsername.Focus();
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text;

            if (_apiService != null && !string.IsNullOrWhiteSpace(username))
            {
                try
                {
                    _user.Username = username;

                    if (rbUser.IsChecked == true) _user.UserRole = 2;
                    else _user.UserRole = 1;

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
                MessageBox.Show("Введите username", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _user = await _apiService.GetUserAsync(Data.user.UserId);

            tbUsername.Text = _user.Username;

            if (_user.UserRole == 1) rbAdmin.IsChecked = true;
            else rbUser.IsChecked = true;
        }
    }
}

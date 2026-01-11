using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
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
using VIPKS_3.Classes;

namespace VIPKS_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class Data
    {
        public static User? user;
    }

    public partial class MainWindow : Window
    {
        Student _currentStudent;
        List<Student> _studentsList = new List<Student>();
        List<User> _usersList = new List<User>();
        readonly ApiService _apiService = new ApiService();

        public MainWindow()
        {
            InitializeComponent();
            LoadDBinDataGrid();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDBinDataGrid();
            if (!ApiService.IsAdmin)
            {
                btn_Add.IsEnabled = false;
                btn_Edit.IsEnabled = false;
                btn_Delete.IsEnabled = false;
                btn_UpCourse.Visibility = Visibility.Hidden;
                tabUsers.Visibility = Visibility.Hidden;
            }              
        }

        private async Task LoadDBinDataGrid()
        {
            _studentsList = await _apiService.GetStudentsAsync();

            _usersList = await _apiService.GetUsersAsync();

            dg_Users.ItemsSource = _usersList;

            int selectedIndex = dg_Students.SelectedIndex;
            dg_Students.ItemsSource = _studentsList;

            if (selectedIndex != -1)
            {
                if (selectedIndex >= dg_Students.Items.Count) selectedIndex = dg_Students.Items.Count - 1;

                dg_Students.SelectedIndex = selectedIndex;
                dg_Students.ScrollIntoView(dg_Students.SelectedItem);
            }

            dg_Students.Focus();
            tb_RecordCount.Text = $"Записей: {_studentsList.Count}";
        }

        private async void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (tabs.SelectedItem == tabStudents)
            {
                gb_AddEditForm.Header = "Добавление записи";

                gb_AddEditForm.Visibility = Visibility.Visible;

                _currentStudent = new Student() { AdmissionDate = new(1970, 1, 1) };

                DataContext = _currentStudent;
            }

            if (tabs.SelectedItem == tabUsers)
            {
                RegisterWindow w = new RegisterWindow(_apiService);

                if (w.ShowDialog() == true)
                {
                    MessageBox.Show("Пользователь успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                await LoadDBinDataGrid();
            }            
        }

        private async void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (tabs.SelectedItem == tabStudents)
            {
                if (dg_Students.SelectedIndex != -1)
                {
                    gb_AddEditForm.Header = "Изменение записи";

                    gb_AddEditForm.Visibility = Visibility.Visible;

                    var selectedStudent = (Student)dg_Students.SelectedItem;

                    _currentStudent = await _apiService.GetStudentAsync(selectedStudent.Id);

                    DataContext = _currentStudent;
                }
                else
                {
                    MessageBox.Show("Выберите запись для изменения", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            
            if (tabs.SelectedItem == tabUsers)
            {
                if (dg_Users.SelectedIndex != -1)
                {
                    Data.user = (User)dg_Users.SelectedItem;

                    EditUserWindow w = new(_apiService);

                    if (w.ShowDialog() == true)
                    {
                        MessageBox.Show("Пользователь успешно изменён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    await LoadDBinDataGrid();
                }
                else
                {
                    MessageBox.Show("Выберите пользователя для изменения", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (tabs.SelectedItem == tabStudents)
            {
                if (dg_Students.SelectedIndex != -1)
                {
                    MessageBoxResult res;

                    res = MessageBox.Show("Удалить запись?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (res == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Student row = (Student)dg_Students.SelectedItem;

                            await _apiService.RemoveStudentAsync(row.Id);
                            await LoadDBinDataGrid();
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            if (tabs.SelectedItem == tabUsers)
            {
                if (dg_Users.SelectedIndex != -1)
                {
                    MessageBoxResult res;

                    res = MessageBox.Show("Удалить пользователя?", "Удаление пользователя", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (res == MessageBoxResult.Yes)
                    {
                        try
                        {
                            User row = (User)dg_Users.SelectedItem;

                            await _apiService.RemoveUserAsync(row.UserId);
                            await LoadDBinDataGrid();
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadDBinDataGrid();
        }

        private void btn_ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            tb_Search.Clear();
            tb_Search.Focus();
        }

        private async void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_FullName.Text) && !string.IsNullOrEmpty(tb_Group.Text) && !string.IsNullOrEmpty(cb_Course.Text) && !string.IsNullOrEmpty(cb_StudyForm.Text))
            {
                try
                {                  
                    if ((string)gb_AddEditForm.Header == "Добавление записи")
                    {
                        await _apiService.CreateStudentAsync(_currentStudent);
                    }
                    else
                    {
                        await _apiService.UpdateStudentAsync(_currentStudent.Id, _currentStudent);
                    }
                    gb_AddEditForm.Visibility = Visibility.Hidden;

                    await LoadDBinDataGrid();
                }
                catch
                {
                    MessageBox.Show("Ошибка сохранения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля корректными значениями", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            gb_AddEditForm.Visibility = Visibility.Hidden;

            await LoadDBinDataGrid();
        }

        private void tb_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Student> listItem = (List<Student>)dg_Students.ItemsSource;

            var filtered = listItem.Where(p => p.FullName.Contains(tb_Search.Text, StringComparison.OrdinalIgnoreCase) || p.Group.Contains(tb_Search.Text, StringComparison.OrdinalIgnoreCase));

            if (filtered.Any())
            {
                var item = filtered.First();

                dg_Students.SelectedItem = item;

                dg_Students.ScrollIntoView(item);
            }
        }

        private async void cb_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_Students == null) return;

            ComboBox cb = (ComboBox)sender;

            _studentsList = await _apiService.GetStudentsAsync();


            switch (cb.SelectedIndex)
            {
                case 0:
                    LoadDBinDataGrid();
                    break;
                case 1:
                    var filtered = _studentsList.Where(p => p.StudyForm == "Очная");
                    dg_Students.ItemsSource = filtered.ToList();
                    tb_RecordCount.Text = $"Записей: {filtered.Count()}";
                    break;
                case 2:                   
                    filtered = _studentsList.Where(p => p.StudyForm == "Заочная");
                    dg_Students.ItemsSource = filtered.ToList();
                    tb_RecordCount.Text = $"Записей: {filtered.Count()}";
                    break;
            }
        }

        private async void btn_UpCourse_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res;

            res = MessageBox.Show("Вы уверены, что хотите перевести активных студентов на следующий курс?", "Перевод на следующий курс", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (res == MessageBoxResult.Yes)
            {
                await _apiService.UpCourseStudentsAsync();

                await LoadDBinDataGrid();

                MessageBox.Show("Курс студентов успешно повышен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }            
        }

        private async void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (dg_Users.SelectedIndex != -1)
            {
                MessageBoxResult res;

                res = MessageBox.Show("Вы уверены, что хотите поменять пароль?", "Сброс пароля", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (res == MessageBoxResult.Yes)
                {
                    Data.user = (User)dg_Users.SelectedItem;

                    ResetPasswordWindow w = new(_apiService);

                    if (w.ShowDialog() == true)
                    {
                        MessageBox.Show("Пароль успешно изменён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
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
using VIPKS_3.ModelsDB;

namespace VIPKS_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Student _currentStudent;
        List<Student> _studentsList = new List<Student>();
        readonly ApiService _apiService = new ApiService();

        public MainWindow()
        {
            InitializeComponent();
            LoadDBinDataGrid();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDBinDataGrid();
        }

        private async Task LoadDBinDataGrid()
        {
            _studentsList = await _apiService.GetStudentsAsync();

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

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            gb_AddEditForm.Header = "Добавление записи";

            gb_AddEditForm.Visibility = Visibility.Visible;

            _currentStudent = new Student();

            DataContext = _currentStudent;
        }

        private async void btn_Edit_Click(object sender, RoutedEventArgs e)
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

        private async void btn_Delete_Click(object sender, RoutedEventArgs e)
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
            await _apiService.UpCourseStudentsAsync();

            await LoadDBinDataGrid();

            MessageBox.Show("Курс студентов успешно повышен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
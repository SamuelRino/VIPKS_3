using Microsoft.EntityFrameworkCore;
using System.Text;
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
        StudentsContext _db = new StudentsContext();

        public MainWindow()
        {
            InitializeComponent();
            LoadDBinDataGrid();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDBinDataGrid();
        }

        private void LoadDBinDataGrid()
        {
            using (StudentsContext _db = new StudentsContext())
            {
                int selectedIndex = dg_Students.SelectedIndex;
                dg_Students.ItemsSource = _db.Students.ToList();

                if (selectedIndex != -1)
                {
                    if (selectedIndex >= dg_Students.Items.Count) selectedIndex = dg_Students.Items.Count - 1;

                    dg_Students.SelectedIndex = selectedIndex;

                    dg_Students.ScrollIntoView(dg_Students.SelectedItem);
                }

                dg_Students.Focus();

                tb_RecordCount.Text = $"Записей: {_db.Students.Count()}";
            }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            gb_AddEditForm.Header = "Добавление записи";

            gb_AddEditForm.Visibility = Visibility.Visible;

            _currentStudent = new Student();

            MainWindow f = this;

            f.DataContext = _currentStudent;
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {           
            if (dg_Students.SelectedIndex != -1)
            {
                gb_AddEditForm.Header = "Изменение записи";

                gb_AddEditForm.Visibility = Visibility.Visible;

                _currentStudent = _db.Students.Find(((Student)dg_Students.SelectedItem).Id);               

                MainWindow f = this;
                f.DataContext = _currentStudent;           
            }
            else
            {
                MessageBox.Show("Выберите запись для изменения", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
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
                        using (StudentsContext _db = new StudentsContext())
                        {
                            _db.Students.Remove(row);
                            _db.SaveChanges();
                        }
                        LoadDBinDataGrid();
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

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadDBinDataGrid();
        }

        private void btn_ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            tb_Search.Clear();
            tb_Search.Focus();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_FullName.Text) && !string.IsNullOrEmpty(tb_Group.Text) && !string.IsNullOrEmpty(cb_Course.Text) && !string.IsNullOrEmpty(cb_StudyForm.Text))
            {
                try
                {                  
                    if ((string)gb_AddEditForm.Header == "Добавление записи")
                    {
                        _db.Students.Add(_currentStudent);
                        _db.SaveChanges();
                    }
                    else
                    {
                        _db.SaveChanges();
                    }
                    gb_AddEditForm.Visibility = Visibility.Hidden;

                    LoadDBinDataGrid();
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

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            gb_AddEditForm.Visibility = Visibility.Hidden;

            LoadDBinDataGrid();
        }

        private void tb_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Student> listItem = (List<Student>)dg_Students.ItemsSource;

            var filtered = listItem.Where(p => p.FullName.Contains(tb_Search.Text) || p.Group.Contains(tb_Search.Text));

            if (filtered.Count() > 0)
            {
                var item = filtered.First();

                dg_Students.SelectedItem = item;

                dg_Students.ScrollIntoView(item);
            }
        }

        private void cb_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_Students == null) return;

            ComboBox cb = (ComboBox)sender;
            switch (cb.SelectedIndex)
            {
                case 0:
                    LoadDBinDataGrid();
                    break;
                case 1:
                    using (StudentsContext _db = new StudentsContext())
                    {
                        var filtered = _db.Students.Where(p => p.StudyForm == "Очная");

                        dg_Students.ItemsSource = filtered.ToList();
                    }
                    break;
                case 2:
                    using (StudentsContext _db = new StudentsContext())
                    {
                        var filtered = _db.Students.Where(p => p.StudyForm == "Заочная");

                        dg_Students.ItemsSource = filtered.ToList();
                    }
                    break;
            }
        }
    }
}
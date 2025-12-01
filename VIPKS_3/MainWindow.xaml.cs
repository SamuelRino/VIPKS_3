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

        public MainWindow()
        {
            InitializeComponent();
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
            }
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            gb_AddEditForm.Visibility = Visibility.Visible;

            _currentStudent = null;

            MainWindow f = this;
            f.DataContext = _currentStudent;
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dg_Students.SelectedIndex != -1)
            {
                gb_AddEditForm.Visibility = Visibility.Visible;

                _currentStudent = ((Student)dg_Students.SelectedItem);               

                MainWindow f = this;
                f.DataContext = _currentStudent;
            }           
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_ClearSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            gb_AddEditForm.Visibility = Visibility.Hidden; 

        }

        private void tb_Search_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cb_Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dg_Students_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
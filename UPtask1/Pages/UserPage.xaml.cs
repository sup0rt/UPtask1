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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UPtask1.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private Account _currentUser;
        public UserPage(Account user)
        {
            InitializeComponent();
            _currentUser = user;
            ListUser.ItemsSource = Entities.GetContext().User.ToList();
        }

        private void UpdateList()
        {
            if (!IsInitialized)
            {
                return;
            }
            try
            {
                List<User> selectedUsers = Entities.GetContext().User.ToList();

                if (!string.IsNullOrWhiteSpace(tbFioFilter.Text))
                {
                    selectedUsers = selectedUsers.Where(su =>
                    su.FIO.ToLower().Contains(tbFioFilter.Text.ToLower())
                    ).ToList();
                }
                if (cbAdminOnly.IsChecked.Value)
                {
                    List<int> adminUserIds = Entities.GetContext().Account
                    .Where(a => a.Role == 0)
                    .Select(a => a.UserID)
                    .ToList();

                    selectedUsers = selectedUsers.Where(su =>
                        adminUserIds.Contains(su.ID)  
                    ).ToList();
                }
                ListUser.ItemsSource = (cmbSort.SelectedIndex == 0) ? selectedUsers.OrderBy(su => su.FIO).ToList() : selectedUsers.OrderByDescending(su => su.FIO).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex);
            }
        }

        private void tbFioFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList();
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }

        private void onlyAdminCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void onlyAdminCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            tbFioFilter.Text = "";
            cmbSort.SelectedIndex = 0;
            cbAdminOnly.IsChecked = false;
        }
    }
}

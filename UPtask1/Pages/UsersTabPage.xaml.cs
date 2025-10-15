using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Data.Entity;

namespace UPtask1.Pages
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();
            LoadData();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void LoadData()
        {
            try
            {
                // Загрузка пользователей с данными Account
                DataGridUser.ItemsSource = Entities.GetContext()
                    .User
                    .Include(u => u.Account1)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                try
                {
                    Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления данных: {ex.Message}");
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService?.Navigate(new AddUserPage(null));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка перехода на страницу добавления: {ex.Message}");
            }
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var usersForRemoving = DataGridUser.SelectedItems.Cast<User>().ToList();
            if (!usersForRemoving.Any())
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы одного пользователя для удаления.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы уверены, что хотите удалить {usersForRemoving.Count} пользователя(ей)?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().User.RemoveRange(usersForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Пользователи успешно удалены!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления пользователей: {ex.Message}");
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var user = button?.DataContext as User;
                if (user != null)
                {
                    NavigationService?.Navigate(new AddUserPage(user));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка перехода на страницу редактирования: {ex.Message}");
            }
        }
    }
}
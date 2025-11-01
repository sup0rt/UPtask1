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

        public class UserWithAccount
        {
            public User User { get; set; }
            public Account Account { get; set; }
        }

        private void LoadData()
        {
            try
            {
                var context = Entities.GetContext();

                // ИСПРАВЛЕНО: Загрузка пользователей с данными аккаунтов
                DataGridUser.ItemsSource = context.User
                    .Join(context.Account,
                          u => u.ID,
                          a => a.UserID,
                          (u, a) => new UserWithAccount
                          {
                              User = u,
                              Account = a
                          })
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
            var selectedItems = DataGridUser.SelectedItems.Cast<UserWithAccount>().ToList();
            if (!selectedItems.Any())
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы одного пользователя для удаления.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ИСПРАВЛЕНО: Получаем пользователей из объектов UserWithAccount
            var usersForRemoving = selectedItems.Select(x => x.User).ToList();

            if (MessageBox.Show($"Вы уверены, что хотите удалить {usersForRemoving.Count} пользователя(ей)?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var context = Entities.GetContext();
                    var userIds = usersForRemoving.Select(u => u.ID).ToList();
                    var accountsForRemoving = context.Account
                        .Where(a => userIds.Contains(a.UserID))
                        .ToList();

                    context.Account.RemoveRange(accountsForRemoving);
                    context.User.RemoveRange(usersForRemoving);
                    context.SaveChanges();
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
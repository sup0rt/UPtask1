using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Data.Entity; // Для метода Include

namespace UPtask1.Pages
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();
            LoadUsers(); // Загрузка данных при инициализации
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void LoadUsers()
        {
            try
            {
                var context = Entities.GetContext();
                if (context == null)
                {
                    MessageBox.Show("Ошибка: Контекст базы данных не инициализирован.");
                    return;
                }

                // Загружаем пользователей с их связанными аккаунтами
                var users = context.User.Include(u => u.Account1).ToList();
                DataGridUser.ItemsSource = users;

                if (!users.Any())
                {
                    MessageBox.Show("Таблица User пуста или данные не загрузились.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                try
                {
                    var context = Entities.GetContext();
                    if (context == null)
                    {
                        MessageBox.Show("Ошибка: Контекст базы данных не инициализирован.");
                        return;
                    }

                    // Обновляем данные в контексте и подгружаем связанные аккаунты
                    context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                    DataGridUser.ItemsSource = context.User.Include(u => u.Account1).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddUserPage(null));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var usersForRemoving = DataGridUser.SelectedItems.Cast<User>().ToList();
            if (!usersForRemoving.Any())
            {
                MessageBox.Show("Выберите хотя бы одну запись для удаления.");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {usersForRemoving.Count()} элементов?",
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var context = Entities.GetContext();
                    // Удаляем пользователей, но не удаляем связанные аккаунты
                    context.User.RemoveRange(usersForRemoving);
                    context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");
                    LoadUsers(); // Перезагрузка данных после удаления
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении данных: {ex.Message}");
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = (sender as Button)?.DataContext as User;
            if (selectedUser != null)
            {
                NavigationService?.Navigate(new AddUserPage(selectedUser));
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.");
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminPage());
        }
    }
}
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace UPtask1.Pages
{
    public partial class AddUserPage : Page
    {
        private User _currentUser = new User();
        private Account _currentAccount = null; // Связанный аккаунт

        public AddUserPage(User selectedUser)
        {
            InitializeComponent();

            // Если передан существующий пользователь, редактируем его
            if (selectedUser != null)
            {
                _currentUser = selectedUser;
                _currentAccount = selectedUser.Account1; // Загружаем связанный аккаунт
            }
            else
            {
                _currentAccount = new Account(); // Новый аккаунт для нового пользователя
            }

            DataContext = _currentUser;

            // Установка значений для полей Account
            if (_currentAccount != null)
            {
                TBLogin.Text = _currentAccount.Login;
                TBPass.Text = _currentAccount.Password;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Валидация полей User
            if (string.IsNullOrWhiteSpace(_currentUser.FIO))
                errors.AppendLine("Укажите ФИО!");
            if (string.IsNullOrWhiteSpace(_currentUser.Photo))
                errors.AppendLine("Укажите путь к фото!");

            // Валидация полей Account
            if (string.IsNullOrWhiteSpace(TBLogin.Text))
                errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(TBPass.Text))
                errors.AppendLine("Укажите пароль!");

            // Проверка уникальности логина
            var context = Entities.GetContext();
            if (context == null)
            {
                MessageBox.Show("Ошибка: Контекст базы данных не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (context.Account.Any(a => a.Login == TBLogin.Text && a.ID != _currentAccount.ID))
            {
                errors.AppendLine("Логин уже занят!");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Обновление данных Account
                _currentAccount.Login = TBLogin.Text;
                _currentAccount.Password = TBPass.Text; // В реальном приложении используйте хеширование
                _currentAccount.Salt = null; // Если Salt не используется, устанавливаем null

                // Если это новый аккаунт, добавляем его
                if (_currentAccount.ID == 0)
                {
                    context.Account.Add(_currentAccount);
                    context.SaveChanges(); // Сохраняем аккаунт, чтобы получить ID
                }
                else
                {
                    context.Entry(_currentAccount).State = System.Data.Entity.EntityState.Modified;
                }

                // Установка внешнего ключа в User
                _currentUser.Account = _currentAccount.ID;

                // Если это новый пользователь, добавляем его
                if (_currentUser.ID == 0)
                {
                    context.User.Add(_currentUser);
                }
                else
                {
                    context.Entry(_currentUser).State = System.Data.Entity.EntityState.Modified;
                }

                context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack(); // Возврат на предыдущую страницу
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            // Очистка полей формы
            TBFio.Text = string.Empty;
            TBPhoto.Text = string.Empty;
            TBLogin.Text = string.Empty;
            TBPass.Text = string.Empty;

            // Сброс объектов
            _currentUser = new User();
            _currentAccount = new Account();
            DataContext = _currentUser;
        }

        private void TBPhoto_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
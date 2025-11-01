using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace UPtask1.Pages
{
    public partial class AddUserPage : Page
    {
        private User _currentUser;
        private Account _currentAccount;

        public User CurrentUser => _currentUser;
        public Account CurrentAccount => _currentAccount;

        public AddUserPage(User selectedUser)
        {
            InitializeComponent();

            if (selectedUser != null)
            {
                _currentUser = Entities.GetContext().User
                    .FirstOrDefault(u => u.ID == selectedUser.ID) ?? selectedUser;

                // ИЗМЕНЕНО: Загружаем связанный аккаунт через UserID
                _currentAccount = Entities.GetContext().Account
                    .FirstOrDefault(a => a.UserID == _currentUser.ID) ?? new Account();
            }
            else
            {
                _currentUser = new User();
                _currentAccount = new Account();
            }

            DataContext = this;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Валидация Account
            if (string.IsNullOrWhiteSpace(_currentAccount.Login))
                errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(TBPass.Text))
                errors.AppendLine("Укажите пароль!");
            var selectedRole = cmbRole.SelectedItem as ComboBoxItem;
            if (selectedRole != null)
            {
                _currentAccount.Role = selectedRole.Content.ToString() == "Admin" ? 1 : 2;
            }
            else
            {
                errors.AppendLine("Выберите роль!");
            }

            // Валидация User
            if (string.IsNullOrWhiteSpace(_currentUser.FIO))
                errors.AppendLine("Укажите ФИО!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = Entities.GetContext();

                // Хешируем пароль только если он был изменен
                if (!string.IsNullOrWhiteSpace(TBPass.Text))
                {
                    _currentAccount.Password = PasswordHasher.CreateHash(TBPass.Text, out string salt);
                    _currentAccount.Salt = salt;
                }

                if (_currentUser.ID == 0)
                {
                    // ИЗМЕНЕНО: Сначала создаем пользователя
                    context.User.Add(_currentUser);
                    context.SaveChanges(); // Сохраняем, чтобы получить ID пользователя

                    // Затем создаем аккаунт с ссылкой на UserID
                    _currentAccount.UserID = _currentUser.ID;
                    context.Account.Add(_currentAccount);
                }
                else
                {
                    // ИЗМЕНЕНО: Обновляем пользователя и аккаунт отдельно
                    context.Entry(_currentUser).State = EntityState.Modified;

                    // Для аккаунта проверяем, существует ли он уже
                    var existingAccount = context.Account.FirstOrDefault(a => a.UserID == _currentUser.ID);
                    if (existingAccount != null)
                    {
                        // Обновляем существующий аккаунт
                        existingAccount.Login = _currentAccount.Login;
                        existingAccount.Password = _currentAccount.Password;
                        existingAccount.Salt = _currentAccount.Salt;
                        existingAccount.Role = _currentAccount.Role;
                        context.Entry(existingAccount).State = EntityState.Modified;
                    }
                    else
                    {
                        // Создаем новый аккаунт
                        _currentAccount.UserID = _currentUser.ID;
                        context.Account.Add(_currentAccount);
                    }
                }

                context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBLogin.Text = "";
            TBPass.Text = "";
            cmbRole.SelectedIndex = -1;
            TBFio.Text = "";
            TBPhoto.Text = "";

            _currentAccount.Login = "";
            _currentAccount.Password = "";
            _currentUser.FIO = "";
            _currentUser.Photo = "";
        }
    }
}

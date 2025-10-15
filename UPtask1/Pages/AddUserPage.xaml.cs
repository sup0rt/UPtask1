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
                    .Include(u => u.Account1)
                    .FirstOrDefault(u => u.ID == selectedUser.ID) ?? selectedUser;
            }
            else
            {
                _currentUser = new User();
                _currentAccount = new Account();
                _currentUser.Account1 = _currentAccount;
            }

            _currentAccount = _currentUser.Account1 ?? new Account();
            _currentUser.Account1 = _currentAccount;

            DataContext = this;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Валидация Account
            if (string.IsNullOrWhiteSpace(_currentAccount.Login))
                errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(_currentAccount.Password))
                errors.AppendLine("Укажите пароль!");
            if (_currentAccount.Role == null)
            {
                // Преобразуем выбранную роль в int (например, Admin = 1, User = 2)
                var selectedRole = cmbRole.SelectedItem as ComboBoxItem;
                if (selectedRole != null)
                {
                    _currentAccount.Role = selectedRole.Content.ToString() == "Admin" ? 1 : 2;
                }
                else
                {
                    errors.AppendLine("Выберите роль!");
                }
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
                _currentAccount.Password = PasswordHasher.CreateHash(TBPass.Text, out string salt);
                _currentAccount.Salt = salt;

                if (_currentUser.ID == 0)
                {
                    context.Account.Add(_currentAccount);
                    context.User.Add(_currentUser);
                }
                else
                {
                    context.Entry(_currentAccount).State = EntityState.Modified;
                    context.Entry(_currentUser).State = EntityState.Modified;
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
            _currentAccount.Role = null;
            _currentUser.FIO = "";
            _currentUser.Photo = "";
        }
    }
}
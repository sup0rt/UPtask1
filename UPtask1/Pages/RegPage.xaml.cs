using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }

        private User _user = new User();
        private Account _userAccount = new Account();


        private void Registrate()
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(TbFullname.Text)) errors.AppendLine("Введите ФИО");
            if (string.IsNullOrEmpty(TbUsername.Text)) errors.AppendLine("Введите логин");
            if (string.IsNullOrEmpty(PbPassword.Password)) errors.AppendLine("Введите пароль");
            if (string.IsNullOrEmpty(PbPasswordCheck.Password)) errors.AppendLine("Повторите пароль");

            if (PbPassword.Password.Length > 0)
            {
                bool en = true;
                bool number = false;
                for (int i = 0; i < PbPassword.Password.Length; i++)
                {
                    if (PbPassword.Password[i] >= 'А' && PbPassword.Password[i] <= 'Я') en = false;
                    if (PbPassword.Password[i] >= '0' && PbPassword.Password[i] <= '9') number = true;
                }

                if (PbPassword.Password.Length < 6) errors.AppendLine("Пароль должен быть больше 6 символов");
                if (!en) errors.AppendLine("Пароль должен быть на английском языке");
                if (!number) errors.AppendLine("В пароле должна быть минимум 1 цифра");
            }

            if (PbPassword.Password != PbPasswordCheck.Password) errors.AppendLine("Пароли не совпадают");

            if (TbUsername.Text.Length > 0)
            {
                using (var db = new Entities())
                {
                    var user = db.Account.AsNoTracking().FirstOrDefault(u => u.Login == TbUsername.Text);
                    if (user != null) errors.AppendLine("Пользователь с таким логином уже существует");
                }
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            try
            {
                var context = Entities.GetContext();

                _user.FIO = TbFullname.Text;
                context.User.Add(_user);
                context.SaveChanges(); 

                _userAccount.Login = TbUsername.Text;
                _userAccount.Role = 2;
                _userAccount.Password = PasswordHasher.CreateHash(PbPassword.Password, out string salt);
                _userAccount.Salt = salt;
                _userAccount.UserID = _user.ID; 
                context.Account.Add(_userAccount);
                context.SaveChanges();

                MessageBox.Show("Вы зарегистрировались", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new UserPage(_userAccount));
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

                string fullErrorMessage = string.Join("\n", errorMessages);
                MessageBox.Show("Ошибки валидации:\n" + fullErrorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            Registrate();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }
    }
}

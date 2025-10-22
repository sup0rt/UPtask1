using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace UPtask1.Pages
{
    public partial class ChangePasswordPage : Page
    {
        private readonly Account _user;

        public ChangePasswordPage(Account user)
        {
            InitializeComponent();
            _user = user;
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
           
            if (string.IsNullOrEmpty(PbNewPassword.Password) || string.IsNullOrEmpty(PbConfirmPassword.Password))
            {
                MessageBox.Show("Введите новый пароль и подтверждение", "Ошибка");
                return;
            }

            
            if (PbNewPassword.Password != PbConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка");
                return;
            }

            
            

           
            var context = Entities.GetContext();
            var userToUpdate = context.Account.FirstOrDefault(u => u.Login == _user.Login);
            if (userToUpdate != null)
            {
               
                string newSalt;
                string hashedPassword = PasswordHasher.CreateHash(PbNewPassword.Password, out newSalt);

            
                userToUpdate.Password = hashedPassword;
                userToUpdate.Salt = newSalt;
                context.SaveChanges();

                MessageBox.Show("Пароль успешно изменен", "Успех");
                NavigationService.Navigate(new AuthPage());
            }
            else
            {
                MessageBox.Show("Пользователь не найден в базе данных", "Ошибка");
            }
        }

        private void BackToAuthLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }
    }
}
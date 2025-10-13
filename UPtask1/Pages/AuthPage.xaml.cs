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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void Authorize()
        {
            if (string.IsNullOrWhiteSpace(TbUsername.Text) | string.IsNullOrEmpty(PbPassword.Password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }
            var user = Entities.GetContext().Account.AsNoTracking().FirstOrDefault(u => u.Login == TbUsername.Text);
            if (user == null)
            {
                MessageBox.Show("Пользователь с такими данными не найден");
                return;
            }
            bool isValid = PasswordHasher.VerifyPassword(PbPassword.Password, user.Password, user.Salt);
            if (!isValid)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }
            else
            {
                TbUsername.Clear();
                PbPassword.Clear();
                if (user.Role == 1)
                {
                    MessageBox.Show("Добро пожаловать!");
                    NavigationService.Navigate(new UserPage(user));
                }
                else if (user.Role == 0)
                {
                    MessageBox.Show("Добро пожаловать!");
                    NavigationService.Navigate(new AdminPage());
                }
                else
                {
                    MessageBox.Show("Ошибка идентификации роли пользователя");
                    return;
                }
            }
        }

        private void btgLogin_Click(object sender, RoutedEventArgs e)
        {
            Authorize();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        
    }
}

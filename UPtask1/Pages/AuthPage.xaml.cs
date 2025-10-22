using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace UPtask1.Pages
{
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;
        private string currentCaptcha; 

        public AuthPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Authorize()
        {
            if (string.IsNullOrWhiteSpace(TbUsername.Text) || string.IsNullOrEmpty(PbPassword.Password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка");
                failedAttempts++;
                if (failedAttempts >= 3)
                {
                    CaptchaSwitch();
                }
                return;
            }

            var user = Entities.GetContext().Account.AsNoTracking().FirstOrDefault(u => u.Login == TbUsername.Text);
            if (user == null)
            {
                MessageBox.Show("Пользователь с такими данными не найден", "Ошибка");
                failedAttempts++;
                if (failedAttempts >= 3)
                {
                    CaptchaSwitch();
                }
                return;
            }

            bool isValid = PasswordHasher.VerifyPassword(PbPassword.Password, user.Password, user.Salt);
            if (!isValid)
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка");
                failedAttempts++;
                if (failedAttempts >= 3)
                {
                    CaptchaSwitch();
                }
                return;
            }

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
                MessageBox.Show("Ошибка идентификации роли пользователя", "Ошибка");
                return;
            }
        }

        public void CaptchaSwitch()
        {
            switch (captcha.Visibility)
            {
                case Visibility.Visible:
                    TbUsername.Clear();
                    PbPassword.Clear();

                    captcha.Visibility = Visibility.Hidden;
                    captchaInput.Visibility = Visibility.Hidden;
                    labelCaptcha.Visibility = Visibility.Hidden;
                    submitCaptcha.Visibility = Visibility.Hidden;

                    labelLogin.Visibility = Visibility.Visible;
                    labelPass.Visibility = Visibility.Visible;
                    TbUsername.Visibility = Visibility.Visible;
                    
                    PbPassword.Visibility = Visibility.Visible;
                    
                    ButtonEnter.Visibility = Visibility.Visible;
                    RegLink.Visibility = Visibility.Visible;
                    ChangePasswordLink.Visibility = Visibility.Visible;
                    break;

                case Visibility.Hidden:
                    CaptchaChange(); 
                    captcha.Visibility = Visibility.Visible;
                    captchaInput.Visibility = Visibility.Visible;
                    labelCaptcha.Visibility = Visibility.Visible;
                    submitCaptcha.Visibility = Visibility.Visible;

                    labelLogin.Visibility = Visibility.Hidden;
                    labelPass.Visibility = Visibility.Hidden;
                    TbUsername.Visibility = Visibility.Hidden;
                    PbPassword.Visibility = Visibility.Hidden;
                    ButtonEnter.Visibility = Visibility.Hidden;
                    RegLink.Visibility = Visibility.Hidden;
                    ChangePasswordLink.Visibility = Visibility.Hidden;
                    break;
            }
        }

        public void CaptchaChange()
        {
            string allowchar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random r = new Random();
            currentCaptcha = new string(Enumerable.Repeat(allowchar, 6).Select(s => s[r.Next(s.Length)]).ToArray());
            captcha.Text = currentCaptcha;
        }

        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != currentCaptcha)
            {
                MessageBox.Show("Неверно введена капча", "Ошибка");
                CaptchaChange();
                captchaInput.Clear();
            }
            else
            {
                MessageBox.Show("Капча введена успешно, можете продолжить авторизацию", "Успех");
                CaptchaSwitch();
                failedAttempts = 0;
            }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
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

        private void ChangePasswordLink_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbUsername.Text))
            {
                MessageBox.Show("Введите логин для смены пароля", "Ошибка");
                return;
            }

            var user = Entities.GetContext().Account.AsNoTracking().FirstOrDefault(u => u.Login == TbUsername.Text);
            if (user == null)
            {
                MessageBox.Show("Пользователь с таким логином не найден", "Ошибка");
                return;
            }
            if (user.Role == 0)
            {
                MessageBox.Show("Смена пароля для администраторов запрещена", "Ошибка");
                return;
            }
            NavigationService.Navigate(new ChangePasswordPage(user));
        }
    }
}
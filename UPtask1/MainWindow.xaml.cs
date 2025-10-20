using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace UPtask1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var frame = (Frame)this.FindName("MainFrame");
            MainFrame.Navigated += Frame_Navigated;
            MainFrame.NavigationService.Navigated += NavigationService_Navigated; // Отслеживаем навигацию
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            var frame = sender as Frame;
            if (frame.Content is Page page)
            {
                string pageName = page.GetType().Name;

                if (pageName == "AuthPage")
                    this.Title = "Страница авторизации";
                else if (pageName == "RegPage")
                    this.Title = "Страница регистрации";
                else if (pageName == "AdminPage")
                    this.Title = "Страница администратора";
                else if (pageName == "UserPage")
                    this.Title = "Страница пользователя";
                else if (pageName == "UserTabPage")
                    this.Title = "Страница таблицы пользователей";
                else if (pageName == "AdminTabPage")
                    this.Title = "Страница таблицы администраторов";
                else if (pageName == "PaymentTabPage")
                    this.Title = "Страница таблицы оплат";
                else if (pageName == "DiagrammTabPage")
                    this.Title = "Страница диаграмм";
                else if (pageName == "AddCategoryPage")
                    this.Title = "Страница управления категориями";
                else if (pageName == "AddPaymentPage")
                    this.Title = "Страница управления платежами";
                else if (pageName == "AddUserPage")
                    this.Title = "Страница управления пользователями";
                else
                    this.Title = pageName;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => { tbDateTimeNow.Text = DateTime.Now.ToString(); };
            timer.Start();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы уверенны, что хотите выйти из программы?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            btnBack.IsEnabled = MainFrame.CanGoBack; 
        }
    }
}
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Data.Entity;

namespace UPtask1.Pages
{
    public partial class PaymentTabPage : Page
    {
        public PaymentTabPage()
        {
            InitializeComponent();
            LoadData();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void LoadData()
        {
            try
            {
                // Загрузка платежей с данными User и Category
                DataGridPayment.ItemsSource = Entities.GetContext()
                    .Payment
                    .Include(p => p.User1)
                    .Include(p => p.Category1)
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
                NavigationService?.Navigate(new AddPaymentPage(null));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка перехода на страницу добавления: {ex.Message}");
            }
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var paymentsForRemoving = DataGridPayment.SelectedItems.Cast<Payment>().ToList();
            if (!paymentsForRemoving.Any())
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы один платеж для удаления.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы уверены, что хотите удалить {paymentsForRemoving.Count} платеж(ей)?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Payment.RemoveRange(paymentsForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Платежи успешно удалены!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления платежей: {ex.Message}");
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var payment = button?.DataContext as Payment;
                if (payment != null)
                {
                    NavigationService?.Navigate(new AddPaymentPage(payment));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка перехода на страницу редактирования: {ex.Message}");
            }
        }
    }
}
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using UPtask1; // Пространство имен контекста

namespace UPtask1.Pages
{
    public partial class AddPaymentPage : Page
    {
        private Payment _currentPayment = new Payment();

        public Payment CurrentPayment => _currentPayment;

        public AddPaymentPage(Payment selectedPayment = null)
        {
            InitializeComponent();
            if (selectedPayment != null)
            {
                _currentPayment = selectedPayment;
            }
            DataContext = this;
            cmbUser.ItemsSource = Entities.GetContext().User.ToList();
            cmbCategory.ItemsSource = Entities.GetContext().Category.ToList();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            // Простая проверка на заполненность обязательных полей
            if (cmbUser.SelectedValue == null || cmbCategory.SelectedValue == null || string.IsNullOrWhiteSpace(TBName.Text))
            {
                MessageBox.Show("Заполните все пустые поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Присвоение значений из интерфейса
            _currentPayment.UserID = (int?)cmbUser.SelectedValue;
            _currentPayment.CategoryID = (int?)cmbCategory.SelectedValue;
            _currentPayment.Name = TBName.Text;

            // Обработка опциональных полей
            if (int.TryParse(TBNum.Text, out int num)) _currentPayment.Num = num;
            if (decimal.TryParse(TBPrice.Text, out decimal price)) _currentPayment.Price = price;
            _currentPayment.Date = dpDate.SelectedDate;

            try
            {
                var context = Entities.GetContext();
                if (_currentPayment.ID == 0)
                    context.Payment.Add(_currentPayment);
                else
                    context.Entry(_currentPayment).State = EntityState.Modified;

                context.SaveChanges();
                MessageBox.Show("Сохранено!", "Успех", MessageBoxButton.OK);
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            _currentPayment = new Payment();
            dpDate.SelectedDate = null;
            TBName.Text = "";
            TBNum.Text = "";
            TBPrice.Text = "";
            cmbUser.SelectedIndex = -1;
            cmbCategory.SelectedIndex = -1;
            DataContext = this;
        }

    }
}
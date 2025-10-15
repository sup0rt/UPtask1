using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace UPtask1.Pages
{
    public partial class AddCategoryPage : Page
    {
        private Category _currentCategory;

        public Category CurrentCategory => _currentCategory;

        public AddCategoryPage(Category selectedCategory)
        {
            InitializeComponent();

            if (selectedCategory != null)
            {
                _currentCategory = Entities.GetContext().Category
                    .Include(c => c.Payment)
                    .FirstOrDefault(c => c.ID == selectedCategory.ID) ?? selectedCategory;
            }
            else
            {
                _currentCategory = new Category();
            }

            DataContext = this;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentCategory.Name))
                errors.AppendLine("Укажите название категории!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var context = Entities.GetContext();

                if (_currentCategory.ID == 0)
                {
                    context.Category.Add(_currentCategory);
                }
                else
                {
                    context.Entry(_currentCategory).State = EntityState.Modified;
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
            TBName.Text = "";
            _currentCategory.Name = "";
        }
    }
}
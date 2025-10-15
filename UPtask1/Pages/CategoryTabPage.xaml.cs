using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Data.Entity;

namespace UPtask1.Pages
{
    public partial class CategoryTabPage : Page
    {
        public CategoryTabPage()
        {
            InitializeComponent();
            LoadData();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void LoadData()
        {
            try
            {
                // Загрузка категорий с данными Payment
                DataGridCategory.ItemsSource = Entities.GetContext()
                    .Category
                    .Include(c => c.Payment)
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
                NavigationService?.Navigate(new AddCategoryPage(null));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка перехода на страницу добавления: {ex.Message}");
            }
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var categoriesForRemoving = DataGridCategory.SelectedItems.Cast<Category>().ToList();
            if (!categoriesForRemoving.Any())
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы одну категорию для удаления.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы уверены, что хотите удалить {categoriesForRemoving.Count} категорию(й)?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Category.RemoveRange(categoriesForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Категории успешно удалены!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления категорий: {ex.Message}");
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var category = button?.DataContext as Category;
                if (category != null)
                {
                    NavigationService?.Navigate(new AddCategoryPage(category));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка перехода на страницу редактирования: {ex.Message}");
            }
        }
    }
}
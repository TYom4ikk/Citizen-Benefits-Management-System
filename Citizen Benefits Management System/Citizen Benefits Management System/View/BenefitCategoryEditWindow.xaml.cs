using Citizen_Benefits_Management_System.Classes.Controllers;
using Citizen_Benefits_Management_System.Model;
using System;
using System.Windows;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для BenefitCategoryEditWindow.xaml
    /// </summary>
    public partial class BenefitCategoryEditWindow : Window
    {
        private readonly BenefitCategoriesController _categoriesController;
        private BenefitCategories _category;
        private bool _isEditMode;

        public string CategoryName { get; private set; }

        /// <summary>
        /// Конструктор для добавления новой категории
        /// </summary>
        public BenefitCategoryEditWindow()
        {
            InitializeComponent();
            _categoriesController = new BenefitCategoriesController();
            _isEditMode = false;
            TxtTitle.Text = "Добавление категории льгот";
        }

        /// <summary>
        /// Конструктор для редактирования существующей категории
        /// </summary>
        public BenefitCategoryEditWindow(BenefitCategories category) : this()
        {
            _category = category;
            _isEditMode = true;
            TxtTitle.Text = "Редактирование категории льгот";
            LoadCategoryData();
        }

        /// <summary>
        /// Загружает данные категории в форму
        /// </summary>
        private void LoadCategoryData()
        {
            if (_category != null)
            {
                TxtCategoryName.Text = _category.CategoryName;
                TxtDescription.Text = _category.Description;
            }
        }

        /// <summary>
        /// Валидация введенных данных
        /// </summary>
        private bool ValidateInput()
        {
            // Проверка названия категории
            if (string.IsNullOrWhiteSpace(TxtCategoryName.Text))
            {
                MessageBox.Show("Введите название категории", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCategoryName.Focus();
                return false;
            }

            if (TxtCategoryName.Text.Length < 3)
            {
                MessageBox.Show("Название категории должно содержать минимум 3 символа",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCategoryName.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Обработчик кнопки "Сохранить"
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (_isEditMode)
                {
                    // Редактирование существующей категории
                    _category.CategoryName = TxtCategoryName.Text.Trim();
                    _category.Description = string.IsNullOrWhiteSpace(TxtDescription.Text) 
                        ? null 
                        : TxtDescription.Text.Trim();

                    _categoriesController.Update(_category);
                    CategoryName = _category.CategoryName;
                    
                    MessageBox.Show("Категория успешно обновлена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Добавление новой категории
                    var newCategory = new BenefitCategories
                    {
                        CategoryName = TxtCategoryName.Text.Trim(),
                        Description = string.IsNullOrWhiteSpace(TxtDescription.Text) 
                            ? null 
                            : TxtDescription.Text.Trim()
                    };

                    _categoriesController.Add(newCategory);
                    CategoryName = newCategory.CategoryName;
                    
                    MessageBox.Show("Категория успешно добавлена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении категории: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Отмена"
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

using Citizen_Benefits_Management_System.Classes;
using Citizen_Benefits_Management_System.Classes.Controllers;
using Citizen_Benefits_Management_System.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для BenefitCategoriesPage.xaml
    /// </summary>
    public partial class BenefitCategoriesPage : Page
    {
        private readonly BenefitCategoriesController _categoriesController;
        private readonly EventLogController _eventLogController;
        private List<CategoryViewModel> _allCategories;

        public BenefitCategoriesPage()
        {
            InitializeComponent();
            _categoriesController = new BenefitCategoriesController();
            _eventLogController = new EventLogController();
        }

        /// <summary>
        /// Обработчик загрузки страницы
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
        }

        /// <summary>
        /// Загружает список категорий льгот
        /// </summary>
        private void LoadCategories()
        {
            try
            {
                var categories = _categoriesController.GetAll();
                
                _allCategories = categories.Select(c => new CategoryViewModel
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    CitizenCount = c.CitizenBenefits.Count(cb => cb.IsActive)
                }).ToList();

                DgCategories.ItemsSource = _allCategories;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обновляет статистику
        /// </summary>
        private void UpdateStatistics()
        {
            TxtTotalCategories.Text = _allCategories.Count.ToString();
            TxtTotalCitizens.Text = _allCategories.Sum(c => c.CitizenCount).ToString();
        }

        /// <summary>
        /// Обработчик изменения текста поиска
        /// </summary>
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterCategories();
        }

        /// <summary>
        /// Фильтрует категории по поисковому запросу
        /// </summary>
        private void FilterCategories()
        {
            if (_allCategories == null)
                return;

            string searchText = TxtSearch.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                DgCategories.ItemsSource = _allCategories;
            }
            else
            {
                var filtered = _allCategories.Where(c =>
                    c.CategoryName.ToLower().Contains(searchText) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchText))
                ).ToList();

                DgCategories.ItemsSource = filtered;
            }
        }

        /// <summary>
        /// Обработчик кнопки "Очистить"
        /// </summary>
        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Clear();
            DgCategories.ItemsSource = _allCategories;
        }

        /// <summary>
        /// Обработчик кнопки "Добавить категорию"
        /// </summary>
        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new BenefitCategoryEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                    "Добавление категории льгот",
                    $"Добавлена новая категория: {editWindow.CategoryName}");
                LoadCategories();
            }
        }

        /// <summary>
        /// Обработчик кнопки "Редактировать"
        /// </summary>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int categoryId)
            {
                var category = _categoriesController.GetCategoryById(categoryId);
                if (category != null)
                {
                    var editWindow = new BenefitCategoryEditWindow(category);
                    if (editWindow.ShowDialog() == true)
                    {
                        _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                            "Редактирование категории льгот",
                            $"Изменена категория: {category.CategoryName}");
                        LoadCategories();
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки "Удалить"
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int categoryId)
            {
                var category = _categoriesController.GetCategoryById(categoryId);
                if (category != null)
                {
                    // Проверка наличия связанных льгот
                    int citizenCount = category.CitizenBenefits.Count(cb => cb.IsActive);
                    if (citizenCount > 0)
                    {
                        MessageBox.Show($"Невозможно удалить категорию '{category.CategoryName}', " +
                            $"так как она используется у {citizenCount} граждан(а).\n\n" +
                            "Сначала необходимо удалить или изменить льготы граждан.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var result = MessageBox.Show(
                        $"Вы действительно хотите удалить категорию '{category.CategoryName}'?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _categoriesController.DeleteCategory(categoryId);
                            _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                                "Удаление категории льгот",
                                $"Удалена категория: {category.CategoryName}");
                            LoadCategories();
                            MessageBox.Show("Категория успешно удалена", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении категории: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Модель представления для категории льгот
    /// </summary>
    public class CategoryViewModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int CitizenCount { get; set; }
    }
}

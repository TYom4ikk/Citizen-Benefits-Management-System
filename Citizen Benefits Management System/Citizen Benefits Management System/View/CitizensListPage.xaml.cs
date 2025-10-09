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
    /// Логика взаимодействия для CitizensListPage.xaml
    /// </summary>
    public partial class CitizensListPage : Page
    {
        private readonly CitizensController _citizensController;
        private readonly EventLogController _eventLogController;
        private List<Citizens> _allCitizens;

        public CitizensListPage()
        {
            InitializeComponent();
            _citizensController = new CitizensController();
            _eventLogController = new EventLogController();
            
            LoadCitizens();
        }

        /// <summary>
        /// Загружает список граждан
        /// </summary>
        private void LoadCitizens()
        {
            try
            {
                _allCitizens = _citizensController.GetActiveCitizens();
                UpdateDataGrid(_allCitizens);

                // Логирование
                _eventLogController.LogEvent(
                    SessionManager.CurrentUser?.UserID,
                    "Просмотр списка граждан",
                    $"Пользователь просмотрел список граждан. Найдено записей: {_allCitizens.Count}"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", 
                    "Ошибка", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обновляет DataGrid
        /// </summary>
        private void UpdateDataGrid(List<Citizens> citizens)
        {
            DgCitizens.ItemsSource = citizens;

            if (citizens == null || citizens.Count == 0)
            {
                TxtNoData.Visibility = Visibility.Visible;
                DgCitizens.Visibility = Visibility.Collapsed;
            }
            else
            {
                TxtNoData.Visibility = Visibility.Collapsed;
                DgCitizens.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Обработчик изменения текста поиска
        /// </summary>
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allCitizens == null) return;

            string searchText = TxtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                UpdateDataGrid(_allCitizens);
                return;
            }

            var filtered = _allCitizens.Where(c =>
                c.LastName.ToLower().Contains(searchText) ||
                c.FirstName.ToLower().Contains(searchText) ||
                (!string.IsNullOrEmpty(c.MiddleName) && c.MiddleName.ToLower().Contains(searchText)) ||
                (!string.IsNullOrEmpty(c.SNILS) && c.SNILS.Contains(searchText)) ||
                (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchText))
            ).ToList();

            UpdateDataGrid(filtered);
        }

        /// <summary>
        /// Обработчик кнопки "Очистить поиск"
        /// </summary>
        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Clear();
            UpdateDataGrid(_allCitizens);
        }

        /// <summary>
        /// Обработчик кнопки "Обновить"
        /// </summary>
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Clear();
            LoadCitizens();
        }

        /// <summary>
        /// Обработчик кнопки "Добавить"
        /// </summary>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new CitizenEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                LoadCitizens();
            }
        }

        /// <summary>
        /// Обработчик кнопки "Льготы"
        /// </summary>
        private void BtnBenefits_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int citizenId = (int)button.Tag;
                var benefitsWindow = new CitizenBenefitsWindow(citizenId);
                benefitsWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Обработчик кнопки "Редактировать"
        /// </summary>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int citizenId = (int)button.Tag;
                var editWindow = new CitizenEditWindow(citizenId);
                if (editWindow.ShowDialog() == true)
                {
                    LoadCitizens();
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки "Удалить"
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int citizenId = (int)button.Tag;
                var citizen = _citizensController.GetById(citizenId);

                if (citizen == null)
                {
                    MessageBox.Show("Гражданин не найден", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Вы действительно хотите удалить гражданина:\n{citizen.LastName} {citizen.FirstName} {citizen.MiddleName}?\n\nЭто действие нельзя отменить.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (_citizensController.Delete(citizenId))
                    {
                        MessageBox.Show("Гражданин успешно удален", "Успех", 
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Логирование
                        _eventLogController.LogEvent(
                            SessionManager.CurrentUser?.UserID,
                            "Удаление гражданина",
                            $"Удален гражданин: {citizen.LastName} {citizen.FirstName} {citizen.MiddleName} (ID: {citizenId})",
                            "Citizens",
                            citizenId
                        );

                        LoadCitizens();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении гражданина", "Ошибка", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик выбора строки в DataGrid
        /// </summary>
        private void DgCitizens_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить дополнительную логику при выборе строки
        }
    }
}

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
    /// Логика взаимодействия для CertificatesPage.xaml
    /// </summary>
    public partial class CertificatesPage : Page
    {
        private readonly CertificatesController _certificatesController;
        private readonly EventLogController _eventLogController;
        private List<CertificateViewModel> _allCertificates;

        public CertificatesPage()
        {
            InitializeComponent();
            _certificatesController = new CertificatesController();
            _eventLogController = new EventLogController();
        }

        /// <summary>
        /// Обработчик загрузки страницы
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCertificateTypes();
            LoadCertificates();
        }

        /// <summary>
        /// Загружает типы справок в ComboBox
        /// </summary>
        private void LoadCertificateTypes()
        {
            var types = new List<string>
            {
                "Все типы",
                "Справка о льготах",
                "Справка о статусе",
                "Справка о доходах",
                "Справка о составе семьи",
                "Прочее"
            };

            CmbCertificateType.ItemsSource = types;
            CmbCertificateType.SelectedIndex = 0;
        }

        /// <summary>
        /// Загружает список справок
        /// </summary>
        private void LoadCertificates()
        {
            try
            {
                var certificates = _certificatesController.GetAllCertificates();
                
                _allCertificates = certificates.Select(c => new CertificateViewModel
                {
                    CertificateID = c.CertificateID,
                    CitizenFullName = $"{c.Citizens.LastName} {c.Citizens.FirstName} {c.Citizens.MiddleName}",
                    CertificateType = c.CertificateType,
                    IssueDate = c.IssueDate,
                    IssuedByUser = $"{c.Users.LastName} {c.Users.FirstName}",
                    Notes = c.Notes
                }).ToList();

                DgCertificates.ItemsSource = _allCertificates;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке справок: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обновляет статистику
        /// </summary>
        private void UpdateStatistics()
        {
            TxtTotalCertificates.Text = _allCertificates.Count.ToString();
            
            var currentItems = DgCertificates.ItemsSource as List<CertificateViewModel>;
            TxtFilteredCertificates.Text = currentItems?.Count.ToString() ?? "0";
        }

        /// <summary>
        /// Применяет фильтры к списку справок
        /// </summary>
        private void ApplyFilters()
        {
            if (_allCertificates == null)
                return;

            var filtered = _allCertificates.AsEnumerable();

            // Фильтр по поиску
            string searchText = TxtSearch.Text.ToLower().Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(c => c.CitizenFullName.ToLower().Contains(searchText));
            }

            // Фильтр по типу справки
            if (CmbCertificateType.SelectedIndex > 0)
            {
                string selectedType = CmbCertificateType.SelectedItem.ToString();
                filtered = filtered.Where(c => c.CertificateType == selectedType);
            }

            // Фильтр по дате начала
            if (DpStartDate.SelectedDate.HasValue)
            {
                filtered = filtered.Where(c => c.IssueDate >= DpStartDate.SelectedDate.Value);
            }

            // Фильтр по дате окончания
            if (DpEndDate.SelectedDate.HasValue)
            {
                filtered = filtered.Where(c => c.IssueDate <= DpEndDate.SelectedDate.Value);
            }

            DgCertificates.ItemsSource = filtered.ToList();
            UpdateStatistics();
        }

        /// <summary>
        /// Обработчик изменения текста поиска
        /// </summary>
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Обработчик изменения типа справки
        /// </summary>
        private void CmbCertificateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Обработчик изменения фильтра по дате
        /// </summary>
        private void DateFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Обработчик кнопки "Очистить фильтры"
        /// </summary>
        private void BtnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Clear();
            CmbCertificateType.SelectedIndex = 0;
            DpStartDate.SelectedDate = null;
            DpEndDate.SelectedDate = null;
            DgCertificates.ItemsSource = _allCertificates;
            UpdateStatistics();
        }

        /// <summary>
        /// Обработчик кнопки "Зарегистрировать справку"
        /// </summary>
        private void BtnAddCertificate_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new CertificateEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                    "Регистрация справки",
                    $"Зарегистрирована справка для гражданина");
                LoadCertificates();
            }
        }

        /// <summary>
        /// Обработчик кнопки "Просмотр"
        /// </summary>
        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int certificateId)
            {
                var certificate = _certificatesController.GetCertificateById(certificateId);
                if (certificate != null)
                {
                    var viewWindow = new CertificateViewWindow(certificate);
                    viewWindow.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки "Удалить"
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int certificateId)
            {
                var certificate = _certificatesController.GetCertificateById(certificateId);
                if (certificate != null)
                {
                    var result = MessageBox.Show(
                        $"Вы действительно хотите удалить справку №{certificate.CertificateID}?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _certificatesController.DeleteCertificate(certificateId);
                            _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                                "Удаление справки",
                                $"Удалена справка №{certificateId}");
                            LoadCertificates();
                            MessageBox.Show("Справка успешно удалена", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении справки: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Модель представления для справки
    /// </summary>
    public class CertificateViewModel
    {
        public int CertificateID { get; set; }
        public string CitizenFullName { get; set; }
        public string CertificateType { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuedByUser { get; set; }
        public string Notes { get; set; }
    }
}

using Citizen_Benefits_Management_System.Classes;
using Citizen_Benefits_Management_System.Classes.Controllers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Page
    {
        private readonly CitizensController _citizensController;
        private readonly BenefitCategoriesController _categoriesController;
        private readonly CertificatesController _certificatesController;
        private readonly RegionsController _regionsController;
        private readonly EventLogController _eventLogController;

        public ReportsPage()
        {
            InitializeComponent();
            _citizensController = new CitizensController();
            _categoriesController = new BenefitCategoriesController();
            _certificatesController = new CertificatesController();
            _regionsController = new RegionsController();
            _eventLogController = new EventLogController();
        }

        /// <summary>
        /// Обработчик загрузки страницы
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBenefitCategories();
            LoadRegions();
            LoadStatistics();
            
            // Установка дат по умолчанию
            DpCertStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
            DpCertEndDate.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Загружает категории льгот
        /// </summary>
        private void LoadBenefitCategories()
        {
            var categories = _categoriesController.GetAllCategories();
            var categoryList = categories.Select(c => new { c.CategoryID, c.CategoryName }).ToList();
            categoryList.Insert(0, new { CategoryID = 0, CategoryName = "Все категории" });
            
            CmbBenefitCategory.ItemsSource = categoryList;
            CmbBenefitCategory.DisplayMemberPath = "CategoryName";
            CmbBenefitCategory.SelectedValuePath = "CategoryID";
            CmbBenefitCategory.SelectedIndex = 0;
        }

        /// <summary>
        /// Загружает регионы
        /// </summary>
        private void LoadRegions()
        {
            var regions = _regionsController.GetAllRegions();
            var regionList = regions.Select(r => new { r.RegionID, r.RegionName }).ToList();
            regionList.Insert(0, new { RegionID = 0, RegionName = "Все регионы" });
            
            CmbRegion.ItemsSource = regionList;
            CmbRegion.DisplayMemberPath = "RegionName";
            CmbRegion.SelectedValuePath = "RegionID";
            CmbRegion.SelectedIndex = 0;
        }

        /// <summary>
        /// Загружает статистику
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                var allCitizens = _citizensController.GetAllCitizens();
                TxtTotalCitizens.Text = allCitizens.Count.ToString();

                var beneficiariesCount = allCitizens.Count(c => c.CitizenBenefits.Any(cb => cb.IsActive));
                TxtTotalBeneficiaries.Text = beneficiariesCount.ToString();

                var allCertificates = _certificatesController.GetAllCertificates();
                TxtTotalCertificates.Text = allCertificates.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Экспорт в Word" для отчета по льготам
        /// </summary>
        private void BtnExportBenefitsToWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int categoryId = (int)CmbBenefitCategory.SelectedValue;
                int regionId = (int)CmbRegion.SelectedValue;

                string categoryName = categoryId == 0 ? "Все категории" : CmbBenefitCategory.Text;
                string regionName = regionId == 0 ? "Все регионы" : CmbRegion.Text;

                MessageBox.Show(
                    $"Функция экспорта в Word будет реализована в следующей версии.\n\n" +
                    $"Параметры отчета:\n" +
                    $"Категория: {categoryName}\n" +
                    $"Регион: {regionName}",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                    "Формирование отчета",
                    $"Запрошен отчет по льготным категориям (Word): {categoryName}, {regionName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Экспорт в Excel" для отчета по льготам
        /// </summary>
        private void BtnExportBenefitsToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int categoryId = (int)CmbBenefitCategory.SelectedValue;
                int regionId = (int)CmbRegion.SelectedValue;

                string categoryName = categoryId == 0 ? "Все категории" : CmbBenefitCategory.Text;
                string regionName = regionId == 0 ? "Все регионы" : CmbRegion.Text;

                MessageBox.Show(
                    $"Функция экспорта в Excel будет реализована в следующей версии.\n\n" +
                    $"Параметры отчета:\n" +
                    $"Категория: {categoryName}\n" +
                    $"Регион: {regionName}",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                    "Формирование отчета",
                    $"Запрошен отчет по льготным категориям (Excel): {categoryName}, {regionName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Экспорт в Excel" для отчета по справкам
        /// </summary>
        private void BtnExportCertificatesToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!DpCertStartDate.SelectedDate.HasValue || !DpCertEndDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите период для формирования отчета",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var startDate = DpCertStartDate.SelectedDate.Value;
                var endDate = DpCertEndDate.SelectedDate.Value;

                if (startDate > endDate)
                {
                    MessageBox.Show("Дата начала не может быть больше даты окончания",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var count = _certificatesController.GetCertificateCountByDateRange(startDate, endDate);

                MessageBox.Show(
                    $"Функция экспорта в Excel будет реализована в следующей версии.\n\n" +
                    $"Параметры отчета:\n" +
                    $"Период: с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}\n" +
                    $"Количество справок: {count}",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                    "Формирование отчета",
                    $"Запрошен отчет по справкам за период {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Сформировать диаграмму"
        /// </summary>
        private void BtnGenerateChart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var statistics = _categoriesController.GetCategoryStatistics();

                string statsText = "Статистика по категориям:\n\n";
                foreach (var stat in statistics)
                {
                    statsText += $"{stat.Key}: {stat.Value} граждан(а)\n";
                }

                MessageBox.Show(
                    $"Функция формирования диаграмм будет реализована в следующей версии.\n\n" +
                    statsText,
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                    "Формирование отчета",
                    "Запрошена диаграмма по категориям льгот");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании диаграммы: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

using Citizen_Benefits_Management_System.Classes;
using Citizen_Benefits_Management_System.Classes.Controllers;
using Citizen_Benefits_Management_System.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для BenefitEditWindow.xaml
    /// </summary>
    public partial class BenefitEditWindow : Window
    {
        private readonly CitizenBenefitsController _benefitsController;
        private readonly BenefitCategoriesController _categoriesController;
        private readonly EventLogController _eventLogController;
        private readonly int _citizenId;
        private readonly int? _benefitId;
        private CitizenBenefits _currentBenefit;

        /// <summary>
        /// Конструктор для добавления новой льготы
        /// </summary>
        public BenefitEditWindow(int citizenId)
        {
            InitializeComponent();
            _benefitsController = new CitizenBenefitsController();
            _categoriesController = new BenefitCategoriesController();
            _eventLogController = new EventLogController();
            _citizenId = citizenId;
            _benefitId = null;
            
            TxtTitle.Text = "Добавление льготы";
            DpStartDate.SelectedDate = DateTime.Now;
            LoadCategories();
        }

        /// <summary>
        /// Конструктор для редактирования существующей льготы
        /// </summary>
        public BenefitEditWindow(int citizenId, int benefitId) : this(citizenId)
        {
            _benefitId = benefitId;
            TxtTitle.Text = "Редактирование льготы";
            LoadBenefitData();
        }

        /// <summary>
        /// Загружает список категорий льгот
        /// </summary>
        private void LoadCategories()
        {
            try
            {
                var categories = _categoriesController.GetActiveCategories();
                CbCategory.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Загружает данные льготы для редактирования
        /// </summary>
        private void LoadBenefitData()
        {
            if (!_benefitId.HasValue) return;

            try
            {
                _currentBenefit = _benefitsController.GetById(_benefitId.Value);
                
                if (_currentBenefit == null)
                {
                    MessageBox.Show("Льгота не найдена", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                // Заполнение полей
                CbCategory.SelectedValue = _currentBenefit.CategoryID;
                DpStartDate.SelectedDate = _currentBenefit.StartDate;
                DpEndDate.SelectedDate = _currentBenefit.EndDate;
                TxtBenefitNumber.Text = _currentBenefit.BenefitNumber;
                TxtDescription.Text = _currentBenefit.Description;
                CbStatus.Text = _currentBenefit.Status;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Сохранить"
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            try
            {
                var benefit = _currentBenefit ?? new CitizenBenefits();

                // Заполнение данных
                benefit.CitizenID = _citizenId;
                benefit.CategoryID = (int)CbCategory.SelectedValue;
                benefit.StartDate = DpStartDate.SelectedDate.Value;
                benefit.EndDate = DpEndDate.SelectedDate;
                benefit.BenefitNumber = string.IsNullOrWhiteSpace(TxtBenefitNumber.Text) ? null : TxtBenefitNumber.Text.Trim();
                benefit.Description = string.IsNullOrWhiteSpace(TxtDescription.Text) ? null : TxtDescription.Text.Trim();
                benefit.Status = ((ComboBoxItem)CbStatus.SelectedItem).Content.ToString();
                benefit.CreatedBy = SessionManager.CurrentUser?.UserID ?? 1;

                bool success;
                string action;

                if (_benefitId.HasValue)
                {
                    // Обновление
                    success = _benefitsController.Update(benefit);
                    action = "Редактирование льготы";
                }
                else
                {
                    // Добавление
                    success = _benefitsController.Add(benefit);
                    action = "Добавление льготы";
                }

                if (success)
                {
                    var category = _categoriesController.GetById(benefit.CategoryID);
                    
                    // Логирование
                    _eventLogController.LogEvent(
                        SessionManager.CurrentUser?.UserID,
                        action,
                        $"{action}: {category?.CategoryName} для гражданина ID: {_citizenId}",
                        "CitizenBenefits",
                        benefit.CitizenBenefitID
                    );

                    MessageBox.Show("Данные успешно сохранены", "Успех", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    ShowError("Ошибка при сохранении данных");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Валидация формы
        /// </summary>
        private bool ValidateForm()
        {
            // Проверка категории
            if (CbCategory.SelectedValue == null)
            {
                ShowError("Выберите категорию льготы");
                CbCategory.Focus();
                return false;
            }

            // Проверка даты начала
            if (!DpStartDate.SelectedDate.HasValue)
            {
                ShowError("Выберите дату начала");
                DpStartDate.Focus();
                return false;
            }

            // Проверка даты окончания
            if (DpEndDate.SelectedDate.HasValue)
            {
                if (DpEndDate.SelectedDate.Value < DpStartDate.SelectedDate.Value)
                {
                    ShowError("Дата окончания не может быть раньше даты начала");
                    DpEndDate.Focus();
                    return false;
                }
            }

            // Проверка на дублирование активной льготы той же категории
            int categoryId = (int)CbCategory.SelectedValue;
            if (!_benefitId.HasValue && _benefitsController.HasActiveCategory(_citizenId, categoryId))
            {
                ShowError("У гражданина уже есть активная льгота данной категории");
                CbCategory.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Показывает сообщение об ошибке
        /// </summary>
        private void ShowError(string message)
        {
            TxtError.Text = message;
            TxtError.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Обработчик кнопки "Отмена"
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

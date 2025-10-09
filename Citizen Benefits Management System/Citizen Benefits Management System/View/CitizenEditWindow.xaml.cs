using Citizen_Benefits_Management_System.Classes;
using Citizen_Benefits_Management_System.Classes.Controllers;
using Citizen_Benefits_Management_System.Model;
using System;
using System.Linq;
using System.Windows;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для CitizenEditWindow.xaml
    /// </summary>
    public partial class CitizenEditWindow : Window
    {
        private readonly CitizensController _citizensController;
        private readonly EventLogController _eventLogController;
        private readonly int? _citizenId;
        private Citizens _currentCitizen;

        /// <summary>
        /// Конструктор для добавления нового гражданина
        /// </summary>
        public CitizenEditWindow()
        {
            InitializeComponent();
            _citizensController = new CitizensController();
            _eventLogController = new EventLogController();
            _citizenId = null;
            
            TxtTitle.Text = "Добавление гражданина";
            LoadRegions();
        }

        /// <summary>
        /// Конструктор для редактирования существующего гражданина
        /// </summary>
        public CitizenEditWindow(int citizenId) : this()
        {
            _citizenId = citizenId;
            TxtTitle.Text = "Редактирование гражданина";
            LoadCitizenData();
        }

        /// <summary>
        /// Загружает список регионов
        /// </summary>
        private void LoadRegions()
        {
            try
            {
                using (var context = new BenefitsManagementSystemEntities())
                {
                    var regions = context.Regions.ToList();
                    CbRegion.ItemsSource = regions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке регионов: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Загружает данные гражданина для редактирования
        /// </summary>
        private void LoadCitizenData()
        {
            if (!_citizenId.HasValue) return;

            try
            {
                _currentCitizen = _citizensController.GetById(_citizenId.Value);
                
                if (_currentCitizen == null)
                {
                    MessageBox.Show("Гражданин не найден", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                // Заполнение полей
                TxtLastName.Text = _currentCitizen.LastName;
                TxtFirstName.Text = _currentCitizen.FirstName;
                TxtMiddleName.Text = _currentCitizen.MiddleName;
                DpBirthDate.SelectedDate = _currentCitizen.BirthDate;
                TxtSNILS.Text = _currentCitizen.SNILS;
                TxtPhone.Text = _currentCitizen.PhoneNumber;
                TxtEmail.Text = _currentCitizen.Email;
                TxtAddress.Text = _currentCitizen.Address;
                
                if (_currentCitizen.RegionID.HasValue)
                {
                    CbRegion.SelectedValue = _currentCitizen.RegionID.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик изменения текста СНИЛС (автоформатирование)
        /// </summary>
        private void TxtSNILS_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Автоформатирование СНИЛС при вводе
            string text = TxtSNILS.Text.Replace("-", "").Replace(" ", "");
            if (text.Length > 11)
            {
                text = text.Substring(0, 11);
            }

            if (text.Length >= 9)
            {
                TxtSNILS.TextChanged -= TxtSNILS_TextChanged;
                TxtSNILS.Text = $"{text.Substring(0, 3)}-{text.Substring(3, 3)}-{text.Substring(6, 3)} {text.Substring(9)}";
                TxtSNILS.CaretIndex = TxtSNILS.Text.Length;
                TxtSNILS.TextChanged += TxtSNILS_TextChanged;
            }
        }

        /// <summary>
        /// Обработчик изменения текста телефона (автоформатирование)
        /// </summary>
        private void TxtPhone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Простое форматирование телефона
            string text = TxtPhone.Text.Replace("+", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
            
            if (text.Length > 11)
            {
                text = text.Substring(0, 11);
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
                var citizen = _currentCitizen ?? new Citizens();

                // Заполнение данных
                citizen.LastName = TxtLastName.Text.Trim();
                citizen.FirstName = TxtFirstName.Text.Trim();
                citizen.MiddleName = string.IsNullOrWhiteSpace(TxtMiddleName.Text) ? null : TxtMiddleName.Text.Trim();
                citizen.BirthDate = DpBirthDate.SelectedDate.Value;
                citizen.SNILS = TxtSNILS.Text.Trim();
                citizen.PhoneNumber = string.IsNullOrWhiteSpace(TxtPhone.Text) ? null : TxtPhone.Text.Trim();
                citizen.Email = string.IsNullOrWhiteSpace(TxtEmail.Text) ? null : TxtEmail.Text.Trim();
                citizen.Address = string.IsNullOrWhiteSpace(TxtAddress.Text) ? null : TxtAddress.Text.Trim();
                citizen.RegionID = CbRegion.SelectedValue as int?;

                bool success;
                string action;

                if (_citizenId.HasValue)
                {
                    // Обновление
                    success = _citizensController.Update(citizen);
                    action = "Редактирование гражданина";
                }
                else
                {
                    // Добавление
                    success = _citizensController.Add(citizen);
                    action = "Добавление гражданина";
                }

                if (success)
                {
                    // Логирование
                    _eventLogController.LogEvent(
                        SessionManager.CurrentUser?.UserID,
                        action,
                        $"{action}: {citizen.LastName} {citizen.FirstName} {citizen.MiddleName}",
                        "Citizens",
                        citizen.CitizenID
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
            // Проверка обязательных полей
            if (!ValidationHelper.IsNotEmpty(TxtLastName.Text))
            {
                ShowError("Введите фамилию");
                TxtLastName.Focus();
                return false;
            }

            if (!ValidationHelper.IsOnlyLetters(TxtLastName.Text))
            {
                ShowError("Фамилия должна содержать только буквы");
                TxtLastName.Focus();
                return false;
            }

            if (!ValidationHelper.IsNotEmpty(TxtFirstName.Text))
            {
                ShowError("Введите имя");
                TxtFirstName.Focus();
                return false;
            }

            if (!ValidationHelper.IsOnlyLetters(TxtFirstName.Text))
            {
                ShowError("Имя должно содержать только буквы");
                TxtFirstName.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(TxtMiddleName.Text) && !ValidationHelper.IsOnlyLetters(TxtMiddleName.Text))
            {
                ShowError("Отчество должно содержать только буквы");
                TxtMiddleName.Focus();
                return false;
            }

            if (!DpBirthDate.SelectedDate.HasValue)
            {
                ShowError("Выберите дату рождения");
                DpBirthDate.Focus();
                return false;
            }

            if (!ValidationHelper.IsValidBirthDate(DpBirthDate.SelectedDate.Value))
            {
                ShowError("Некорректная дата рождения");
                DpBirthDate.Focus();
                return false;
            }

            if (!ValidationHelper.IsNotEmpty(TxtSNILS.Text))
            {
                ShowError("Введите СНИЛС");
                TxtSNILS.Focus();
                return false;
            }

            if (!ValidationHelper.IsValidSNILS(TxtSNILS.Text))
            {
                ShowError("Некорректный СНИЛС. Проверьте правильность ввода");
                TxtSNILS.Focus();
                return false;
            }

            // Проверка уникальности СНИЛС
            if (_citizensController.SNILSExists(TxtSNILS.Text.Trim(), _citizenId))
            {
                ShowError("Гражданин с таким СНИЛС уже существует");
                TxtSNILS.Focus();
                return false;
            }

            // Проверка email если заполнен
            if (!string.IsNullOrWhiteSpace(TxtEmail.Text) && !ValidationHelper.IsValidEmail(TxtEmail.Text))
            {
                ShowError("Некорректный email");
                TxtEmail.Focus();
                return false;
            }

            // Проверка телефона если заполнен
            if (!string.IsNullOrWhiteSpace(TxtPhone.Text) && !ValidationHelper.IsValidPhone(TxtPhone.Text))
            {
                ShowError("Некорректный номер телефона");
                TxtPhone.Focus();
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

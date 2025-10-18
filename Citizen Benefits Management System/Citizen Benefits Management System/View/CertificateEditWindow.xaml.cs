using Citizen_Benefits_Management_System.Classes;
using Citizen_Benefits_Management_System.Classes.Controllers;
using Citizen_Benefits_Management_System.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для CertificateEditWindow.xaml
    /// </summary>
    public partial class CertificateEditWindow : Window
    {
        private readonly CertificatesController _certificatesController;
        private int? _selectedCitizenId;

        public CertificateEditWindow()
        {
            InitializeComponent();
            _certificatesController = new CertificatesController();
            LoadCertificateTypes();
            DpIssueDate.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Загружает типы справок
        /// </summary>
        private void LoadCertificateTypes()
        {
            var types = new List<string>
            {
                "Справка о льготах",
                "Справка о статусе",
                "Справка о доходах",
                "Справка о составе семьи",
                "Прочее"
            };

            CmbCertificateType.ItemsSource = types;
        }

        /// <summary>
        /// Обработчик кнопки "Выбрать гражданина"
        /// </summary>
        private void BtnSelectCitizen_Click(object sender, RoutedEventArgs e)
        {
            var selectWindow = new CitizenSelectWindow();
            if (selectWindow.ShowDialog() == true)
            {
                _selectedCitizenId = selectWindow.SelectedCitizenId;
                TxtCitizenName.Text = selectWindow.SelectedCitizenName;
            }
        }

        /// <summary>
        /// Валидация введенных данных
        /// </summary>
        private bool ValidateInput()
        {
            if (!_selectedCitizenId.HasValue)
            {
                MessageBox.Show("Выберите гражданина", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (CmbCertificateType.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите тип справки", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbCertificateType.Focus();
                return false;
            }

            if (!DpIssueDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату выдачи", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                DpIssueDate.Focus();
                return false;
            }

            if (DpIssueDate.SelectedDate.Value > DateTime.Now)
            {
                MessageBox.Show("Дата выдачи не может быть в будущем", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                DpIssueDate.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Обработчик кнопки "Зарегистрировать"
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                var certificate = new Certificates
                {
                    CitizenID = _selectedCitizenId.Value,
                    CertificateType = CmbCertificateType.SelectedItem.ToString(),
                    IssueDate = DpIssueDate.SelectedDate.Value,
                    IssuedBy = SessionManager.CurrentUser.UserID,
                    Notes = string.IsNullOrWhiteSpace(TxtNotes.Text) ? null : TxtNotes.Text.Trim()
                };

                _certificatesController.AddCertificate(certificate);
                
                MessageBox.Show("Справка успешно зарегистрирована", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации справки: {ex.Message}",
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

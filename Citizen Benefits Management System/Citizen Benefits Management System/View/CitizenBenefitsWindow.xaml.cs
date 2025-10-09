using Citizen_Benefits_Management_System.Classes;
using Citizen_Benefits_Management_System.Classes.Controllers;
using Citizen_Benefits_Management_System.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для CitizenBenefitsWindow.xaml
    /// </summary>
    public partial class CitizenBenefitsWindow : Window
    {
        private readonly CitizenBenefitsController _benefitsController;
        private readonly CitizensController _citizensController;
        private readonly EventLogController _eventLogController;
        private readonly int _citizenId;
        private Citizens _citizen;

        public CitizenBenefitsWindow(int citizenId)
        {
            InitializeComponent();
            _benefitsController = new CitizenBenefitsController();
            _citizensController = new CitizensController();
            _eventLogController = new EventLogController();
            _citizenId = citizenId;

            LoadCitizenInfo();
            LoadBenefits();
        }

        /// <summary>
        /// Загружает информацию о гражданине
        /// </summary>
        private void LoadCitizenInfo()
        {
            _citizen = _citizensController.GetById(_citizenId);
            if (_citizen != null)
            {
                TxtCitizenInfo.Text = $"{_citizen.LastName} {_citizen.FirstName} {_citizen.MiddleName}";
            }
        }

        /// <summary>
        /// Загружает список льгот
        /// </summary>
        private void LoadBenefits()
        {
            try
            {
                var benefits = _benefitsController.GetByCitizenId(_citizenId);
                UpdateDataGrid(benefits);

                // Логирование
                _eventLogController.LogEvent(
                    SessionManager.CurrentUser?.UserID,
                    "Просмотр льгот гражданина",
                    $"Просмотр льгот гражданина: {_citizen?.LastName} {_citizen?.FirstName} (ID: {_citizenId})",
                    "CitizenBenefits",
                    _citizenId
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обновляет DataGrid
        /// </summary>
        private void UpdateDataGrid(List<CitizenBenefits> benefits)
        {
            DgBenefits.ItemsSource = benefits;

            if (benefits == null || benefits.Count == 0)
            {
                TxtNoData.Visibility = Visibility.Visible;
                DgBenefits.Visibility = Visibility.Collapsed;
            }
            else
            {
                TxtNoData.Visibility = Visibility.Collapsed;
                DgBenefits.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Обработчик кнопки "Добавить льготу"
        /// </summary>
        private void BtnAddBenefit_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new BenefitEditWindow(_citizenId);
            if (editWindow.ShowDialog() == true)
            {
                LoadBenefits();
            }
        }

        /// <summary>
        /// Обработчик кнопки "Редактировать"
        /// </summary>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button?.Tag != null)
            {
                int benefitId = (int)button.Tag;
                var editWindow = new BenefitEditWindow(_citizenId, benefitId);
                if (editWindow.ShowDialog() == true)
                {
                    LoadBenefits();
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки "Деактивировать"
        /// </summary>
        private void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button?.Tag != null)
            {
                int benefitId = (int)button.Tag;
                var benefit = _benefitsController.GetById(benefitId);

                if (benefit == null)
                {
                    MessageBox.Show("Льгота не найдена", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Вы действительно хотите деактивировать льготу:\n{benefit.BenefitCategories.CategoryName}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_benefitsController.Deactivate(benefitId))
                    {
                        MessageBox.Show("Льгота успешно деактивирована", "Успех", 
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Логирование
                        _eventLogController.LogEvent(
                            SessionManager.CurrentUser?.UserID,
                            "Деактивация льготы",
                            $"Деактивирована льгота: {benefit.BenefitCategories.CategoryName} для гражданина {_citizen?.LastName} {_citizen?.FirstName}",
                            "CitizenBenefits",
                            benefitId
                        );

                        LoadBenefits();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при деактивации льготы", "Ошибка", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки "Удалить"
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button?.Tag != null)
            {
                int benefitId = (int)button.Tag;
                var benefit = _benefitsController.GetById(benefitId);

                if (benefit == null)
                {
                    MessageBox.Show("Льгота не найдена", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Вы действительно хотите удалить льготу:\n{benefit.BenefitCategories.CategoryName}?\n\nЭто действие нельзя отменить.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (_benefitsController.Delete(benefitId))
                    {
                        MessageBox.Show("Льгота успешно удалена", "Успех", 
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Логирование
                        _eventLogController.LogEvent(
                            SessionManager.CurrentUser?.UserID,
                            "Удаление льготы",
                            $"Удалена льгота: {benefit.BenefitCategories.CategoryName} для гражданина {_citizen?.LastName} {_citizen?.FirstName}",
                            "CitizenBenefits",
                            benefitId
                        );

                        LoadBenefits();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении льготы", "Ошибка", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки "Обновить"
        /// </summary>
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBenefits();
        }

        /// <summary>
        /// Обработчик кнопки "Закрыть"
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

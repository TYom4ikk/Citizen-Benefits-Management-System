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
    /// Логика взаимодействия для EventLogPage.xaml
    /// </summary>
    public partial class EventLogPage : Page
    {
        private readonly EventLogController _eventLogController;
        private readonly UsersController _usersController;
        private List<EventLogViewModel> _allEvents;

        public EventLogPage()
        {
            InitializeComponent();
            _eventLogController = new EventLogController();
            _usersController = new UsersController();
        }

        /// <summary>
        /// Обработчик загрузки страницы
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsers();
            LoadEventTypes();
            LoadEvents();
        }

        /// <summary>
        /// Загружает список пользователей в ComboBox
        /// </summary>
        private void LoadUsers()
        {
            var users = _usersController.GetAllUsers();
            var userList = new List<UserComboItem>
            {
                new UserComboItem { UserID = null, DisplayName = "Все пользователи" }
            };

            userList.AddRange(users.Select(u => new UserComboItem
            {
                UserID = u.UserID,
                DisplayName = $"{u.LastName} {u.FirstName} ({u.Username})"
            }));

            CmbUser.ItemsSource = userList;
            CmbUser.DisplayMemberPath = "DisplayName";
            CmbUser.SelectedIndex = 0;
        }

        /// <summary>
        /// Загружает типы событий в ComboBox
        /// </summary>
        private void LoadEventTypes()
        {
            var eventTypes = new List<string>
            {
                "Все типы",
                "Вход в систему",
                "Выход из системы",
                "Добавление гражданина",
                "Редактирование гражданина",
                "Удаление гражданина",
                "Добавление льготы",
                "Редактирование льготы",
                "Удаление льготы",
                "Добавление категории льгот",
                "Редактирование категории льгот",
                "Удаление категории льгот",
                "Регистрация справки",
                "Удаление справки",
                "Добавление пользователя",
                "Редактирование пользователя",
                "Удаление пользователя"
            };

            CmbEventType.ItemsSource = eventTypes;
            CmbEventType.SelectedIndex = 0;
        }

        /// <summary>
        /// Загружает журнал событий
        /// </summary>
        private void LoadEvents()
        {
            try
            {
                var events = _eventLogController.GetAllEvents();
                
                _allEvents = events.Select(e => new EventLogViewModel
                {
                    EventID = e.EventID,
                    EventDate = e.EventDate,
                    UserFullName = $"{e.Users.LastName} {e.Users.FirstName}",
                    EventType = e.EventType,
                    EventDescription = e.EventDescription,
                    UserID = e.UserID
                }).ToList();

                DgEventLog.ItemsSource = _allEvents;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке журнала событий: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обновляет статистику
        /// </summary>
        private void UpdateStatistics()
        {
            TxtTotalEvents.Text = _allEvents.Count.ToString();
            
            var currentItems = DgEventLog.ItemsSource as List<EventLogViewModel>;
            TxtFilteredEvents.Text = currentItems?.Count.ToString() ?? "0";
        }

        /// <summary>
        /// Применяет фильтры к журналу событий
        /// </summary>
        private void ApplyFilters()
        {
            if (_allEvents == null)
                return;

            var filtered = _allEvents.AsEnumerable();

            // Фильтр по пользователю
            if (CmbUser.SelectedItem is UserComboItem selectedUser && selectedUser.UserID.HasValue)
            {
                filtered = filtered.Where(e => e.UserID == selectedUser.UserID.Value);
            }

            // Фильтр по типу события
            if (CmbEventType.SelectedIndex > 0)
            {
                string selectedType = CmbEventType.SelectedItem.ToString();
                filtered = filtered.Where(e => e.EventType == selectedType);
            }

            // Фильтр по дате начала
            if (DpStartDate.SelectedDate.HasValue)
            {
                var startDate = DpStartDate.SelectedDate.Value.Date;
                filtered = filtered.Where(e => e.EventDate >= startDate);
            }

            // Фильтр по дате окончания
            if (DpEndDate.SelectedDate.HasValue)
            {
                var endDate = DpEndDate.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1);
                filtered = filtered.Where(e => e.EventDate <= endDate);
            }

            DgEventLog.ItemsSource = filtered.ToList();
            UpdateStatistics();
        }

        /// <summary>
        /// Обработчик изменения фильтров
        /// </summary>
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Обработчик кнопки "Очистить фильтры"
        /// </summary>
        private void BtnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            CmbUser.SelectedIndex = 0;
            CmbEventType.SelectedIndex = 0;
            DpStartDate.SelectedDate = null;
            DpEndDate.SelectedDate = null;
            DgEventLog.ItemsSource = _allEvents;
            UpdateStatistics();
        }

        /// <summary>
        /// Обработчик кнопки "Экспорт в Excel"
        /// </summary>
        private void BtnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentItems = DgEventLog.ItemsSource as List<EventLogViewModel>;
                if (currentItems == null || currentItems.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show("Функция экспорта в Excel будет реализована в следующей версии.\n\n" +
                    $"Будет экспортировано записей: {currentItems.Count}",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>
    /// Модель представления для события журнала
    /// </summary>
    public class EventLogViewModel
    {
        public int EventID { get; set; }
        public DateTime EventDate { get; set; }
        public string UserFullName { get; set; }
        public string EventType { get; set; }
        public string EventDescription { get; set; }
        public int UserID { get; set; }
    }

    /// <summary>
    /// Элемент ComboBox для пользователей
    /// </summary>
    public class UserComboItem
    {
        public int? UserID { get; set; }
        public string DisplayName { get; set; }
    }
}

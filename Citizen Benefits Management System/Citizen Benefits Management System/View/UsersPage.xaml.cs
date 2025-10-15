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
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        private readonly UsersController _usersController;
        private readonly EventLogController _eventLogController;
        private List<UserViewModel> _allUsers;

        public UsersPage()
        {
            InitializeComponent();
            _usersController = new UsersController();
            _eventLogController = new EventLogController();
        }

        /// <summary>
        /// Обработчик загрузки страницы
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRoles();
            LoadUsers();
        }

        /// <summary>
        /// Загружает список ролей в ComboBox
        /// </summary>
        private void LoadRoles()
        {
            var roles = new List<string>
            {
                "Все роли",
                "Администратор",
                "Оператор",
                "Гражданин"
            };

            CmbRole.ItemsSource = roles;
            CmbRole.SelectedIndex = 0;
        }

        /// <summary>
        /// Загружает список пользователей
        /// </summary>
        private void LoadUsers()
        {
            try
            {
                var users = _usersController.GetAllUsers();
                
                _allUsers = users.Select(u => new UserViewModel
                {
                    UserID = u.UserID,
                    Username = u.Username,
                    LastName = u.LastName,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    Email = u.Email,
                    RoleName = u.UserRoles.RoleName,
                    RoleID = u.RoleID,
                    LastLogin = u.LastLogin
                }).ToList();

                DgUsers.ItemsSource = _allUsers;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обновляет статистику
        /// </summary>
        private void UpdateStatistics()
        {
            var currentItems = DgUsers.ItemsSource as List<UserViewModel>;
            TxtTotalUsers.Text = currentItems?.Count.ToString() ?? "0";
        }

        /// <summary>
        /// Применяет фильтры к списку пользователей
        /// </summary>
        private void ApplyFilters()
        {
            if (_allUsers == null)
                return;

            var filtered = _allUsers.AsEnumerable();

            // Фильтр по поиску
            string searchText = TxtSearch.Text.ToLower().Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(u =>
                    u.Username.ToLower().Contains(searchText) ||
                    u.LastName.ToLower().Contains(searchText) ||
                    u.FirstName.ToLower().Contains(searchText) ||
                    (u.MiddleName != null && u.MiddleName.ToLower().Contains(searchText)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchText))
                );
            }

            // Фильтр по роли
            if (CmbRole.SelectedIndex > 0)
            {
                string selectedRole = CmbRole.SelectedItem.ToString();
                filtered = filtered.Where(u => u.RoleName == selectedRole);
            }

            DgUsers.ItemsSource = filtered.ToList();
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
        /// Обработчик изменения роли
        /// </summary>
        private void CmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Обработчик кнопки "Очистить фильтры"
        /// </summary>
        private void BtnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Clear();
            CmbRole.SelectedIndex = 0;
            DgUsers.ItemsSource = _allUsers;
            UpdateStatistics();
        }

        /// <summary>
        /// Обработчик кнопки "Добавить пользователя"
        /// </summary>
        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new UserEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                    "Добавление пользователя",
                    $"Добавлен новый пользователь: {editWindow.Username}");
                LoadUsers();
            }
        }

        /// <summary>
        /// Обработчик кнопки "Редактировать"
        /// </summary>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int userId)
            {
                var user = _usersController.GetUserById(userId);
                if (user != null)
                {
                    var editWindow = new UserEditWindow(user);
                    if (editWindow.ShowDialog() == true)
                    {
                        _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                            "Редактирование пользователя",
                            $"Изменен пользователь: {user.Username}");
                        LoadUsers();
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки "Удалить"
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int userId)
            {
                // Проверка, что пользователь не удаляет сам себя
                if (userId == SessionManager.CurrentUser.UserID)
                {
                    MessageBox.Show("Вы не можете удалить свою учетную запись",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var user = _usersController.GetUserById(userId);
                if (user != null)
                {
                    var result = MessageBox.Show(
                        $"Вы действительно хотите удалить пользователя '{user.Username}'?\n\n" +
                        $"ФИО: {user.LastName} {user.FirstName} {user.MiddleName}\n" +
                        $"Роль: {user.UserRoles.RoleName}",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _usersController.DeleteUser(userId);
                            _eventLogController.LogEvent(SessionManager.CurrentUser.UserID,
                                "Удаление пользователя",
                                $"Удален пользователь: {user.Username}");
                            LoadUsers();
                            MessageBox.Show("Пользователь успешно удален", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Модель представления для пользователя
    /// </summary>
    public class UserViewModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int RoleID { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}

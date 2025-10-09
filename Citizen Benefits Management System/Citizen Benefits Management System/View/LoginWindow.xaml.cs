using Citizen_Benefits_Management_System.Classes;
using Citizen_Benefits_Management_System.Classes.Controllers;
using System;
using System.Windows;
using System.Windows.Input;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly UsersController _usersController;
        private readonly EventLogController _eventLogController;

        public LoginWindow()
        {
            InitializeComponent();
            _usersController = new UsersController();
            _eventLogController = new EventLogController();
            
            // Фокус на поле ввода имени пользователя
            TxtUsername.Focus();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Войти"
        /// </summary>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Отмена"
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Обработчик нажатия Enter в поле имени пользователя
        /// </summary>
        private void TxtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TxtPassword.Focus();
            }
        }

        /// <summary>
        /// Обработчик нажатия Enter в поле пароля
        /// </summary>
        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        /// <summary>
        /// Выполняет процесс авторизации
        /// </summary>
        private void PerformLogin()
        {
            try
            {
                // Скрываем предыдущие ошибки
                TxtError.Visibility = Visibility.Collapsed;

                // Валидация полей
                string username = TxtUsername.Text.Trim();
                string password = TxtPassword.Password;

                if (string.IsNullOrWhiteSpace(username))
                {
                    ShowError("Введите имя пользователя");
                    TxtUsername.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    ShowError("Введите пароль");
                    TxtPassword.Focus();
                    return;
                }

                // Хеширование пароля
                string passwordHash = PasswordHasher.HashPassword(password);

                // Попытка аутентификации
                var user = _usersController.Authenticate(username, passwordHash);

                if (user == null)
                {
                    ShowError("Неверное имя пользователя или пароль");
                    TxtPassword.Clear();
                    TxtPassword.Focus();

                    // Логирование неудачной попытки входа
                    _eventLogController.LogEvent(null, "Неудачная авторизация", 
                        $"Попытка входа с именем пользователя: {username}");
                    return;
                }

                // Успешная авторизация
                SessionManager.SetCurrentUser(user);
                _usersController.UpdateLastLogin(user.UserID);

                // Логирование успешного входа
                _eventLogController.LogEvent(user.UserID, "Авторизация", 
                    $"Пользователь {user.Username} вошел в систему");

                // Открытие главного окна
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при авторизации: {ex.Message}");
            }
        }

        /// <summary>
        /// Отображает сообщение об ошибке
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        private void ShowError(string message)
        {
            TxtError.Text = message;
            TxtError.Visibility = Visibility.Visible;
        }
    }
}

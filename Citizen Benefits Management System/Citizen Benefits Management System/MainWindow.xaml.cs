using Citizen_Benefits_Management_System.Classes;
using Citizen_Benefits_Management_System.Classes.Controllers;
using Citizen_Benefits_Management_System.View;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Citizen_Benefits_Management_System
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EventLogController _eventLogController;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _eventLogController = new EventLogController();
            
            // Инициализация таймера для отображения времени
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        /// <summary>
        /// Обработчик загрузки окна
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Проверка авторизации
            if (!SessionManager.IsAuthenticated)
            {
                MessageBox.Show("Необходима авторизация", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
                return;
            }

            // Отображение информации о пользователе
            TxtUserInfo.Text = $"Пользователь: {SessionManager.GetFullName()} ({SessionManager.CurrentUser.UserRoles.RoleName})";
            TxtWelcomeMessage.Text = $"Вы вошли в систему как {SessionManager.GetFullName()}";

            // Настройка доступа к функциям в зависимости от роли
            ConfigureAccessByRole();
        }

        /// <summary>
        /// Настраивает доступ к функциям в зависимости от роли пользователя
        /// </summary>
        private void ConfigureAccessByRole()
        {
            // Администратор имеет доступ ко всем функциям
            if (SessionManager.IsAdmin())
            {
                return;
            }

            // Оператор не имеет доступа к управлению пользователями
            if (SessionManager.IsOperator())
            {
                BtnUsers.IsEnabled = false;
                BtnUsers.Opacity = 0.5;
                return;
            }

            // Гражданин имеет ограниченный доступ
            if (SessionManager.IsCitizen())
            {
                BtnUsers.IsEnabled = false;
                BtnUsers.Opacity = 0.5;
                BtnCategories.IsEnabled = false;
                BtnCategories.Opacity = 0.5;
                BtnEventLog.IsEnabled = false;
                BtnEventLog.Opacity = 0.5;
            }
        }

        /// <summary>
        /// Обработчик тика таймера для обновления времени
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            TxtCurrentTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        /// <summary>
        /// Обработчик закрытия окна
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (SessionManager.IsAuthenticated)
            {
                _eventLogController.LogEvent(SessionManager.CurrentUser.UserID, 
                    "Выход из системы", 
                    $"Пользователь {SessionManager.CurrentUser.Username} вышел из системы");
            }

            _timer?.Stop();
        }

        /// <summary>
        /// Обработчик кнопки "Выход"
        /// </summary>
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти из системы?", 
                "Подтверждение", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _eventLogController.LogEvent(SessionManager.CurrentUser.UserID, 
                    "Выход из системы", 
                    $"Пользователь {SessionManager.CurrentUser.Username} вышел из системы");

                SessionManager.Logout();
                
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        /// <summary>
        /// Обработчик кнопки "Граждане"
        /// </summary>
        private void BtnCitizens_Click(object sender, RoutedEventArgs e)
        {
            PanelWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new CitizensListPage());
        }

        /// <summary>
        /// Обработчик кнопки "Льготы"
        /// </summary>
        private void BtnBenefits_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел 'Льготы' находится в разработке", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Обработчик кнопки "Категории льгот"
        /// </summary>
        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            PanelWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new BenefitCategoriesPage());
        }

        /// <summary>
        /// Обработчик кнопки "Справки"
        /// </summary>
        private void BtnCertificates_Click(object sender, RoutedEventArgs e)
        {
            PanelWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new CertificatesPage());
        }

        /// <summary>
        /// Обработчик кнопки "Журнал событий"
        /// </summary>
        private void BtnEventLog_Click(object sender, RoutedEventArgs e)
        {
            PanelWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new EventLogPage());
        }

        /// <summary>
        /// Обработчик кнопки "Отчеты"
        /// </summary>
        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            PanelWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new ReportsPage());
        }

        /// <summary>
        /// Обработчик кнопки "Пользователи"
        /// </summary>
        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            PanelWelcome.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new UsersPage());
        }
    }
}

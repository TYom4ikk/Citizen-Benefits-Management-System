using Citizen_Benefits_Management_System.Classes;
using Citizen_Benefits_Management_System.Classes.Controllers;
using Citizen_Benefits_Management_System.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Citizen_Benefits_Management_System.View
{
    /// <summary>
    /// Логика взаимодействия для UserEditWindow.xaml
    /// </summary>
    public partial class UserEditWindow : Window
    {
        private readonly UsersController _usersController;
        private Users _user;
        private bool _isEditMode;
        private List<UserRoles> _roles;

        public string Username { get; private set; }

        /// <summary>
        /// Конструктор для добавления нового пользователя
        /// </summary>
        public UserEditWindow()
        {
            InitializeComponent();
            _usersController = new UsersController();
            _isEditMode = false;
            TxtTitle.Text = "Добавление пользователя";
            LoadRoles();
        }

        /// <summary>
        /// Конструктор для редактирования существующего пользователя
        /// </summary>
        public UserEditWindow(Users user) : this()
        {
            _user = user;
            _isEditMode = true;
            TxtTitle.Text = "Редактирование пользователя";
            TxtPasswordLabel.Text = "Новый пароль (оставьте пустым, чтобы не менять)";
            LoadUserData();
        }

        /// <summary>
        /// Загружает список ролей
        /// </summary>
        private void LoadRoles()
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                _roles = context.UserRoles.ToList();
                CmbRole.ItemsSource = _roles;
                CmbRole.DisplayMemberPath = "RoleName";
                CmbRole.SelectedValuePath = "RoleID";
            }
        }

        /// <summary>
        /// Загружает данные пользователя в форму
        /// </summary>
        private void LoadUserData()
        {
            if (_user != null)
            {
                TxtUsername.Text = _user.Username;
                TxtUsername.IsEnabled = false; // Логин нельзя изменить
                TxtLastName.Text = _user.LastName;
                TxtFirstName.Text = _user.FirstName;
                TxtMiddleName.Text = _user.MiddleName;
                TxtEmail.Text = _user.Email;
                TxtPhone.Text = _user.Phone;
                CmbRole.SelectedValue = _user.RoleID;
            }
        }

        /// <summary>
        /// Валидация введенных данных
        /// </summary>
        private bool ValidateInput()
        {
            // Проверка логина
            if (string.IsNullOrWhiteSpace(TxtUsername.Text))
            {
                MessageBox.Show("Введите логин", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtUsername.Focus();
                return false;
            }

            if (TxtUsername.Text.Length < 3)
            {
                MessageBox.Show("Логин должен содержать минимум 3 символа",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtUsername.Focus();
                return false;
            }

            // Проверка уникальности логина при добавлении
            if (!_isEditMode && _usersController.IsUsernameExists(TxtUsername.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtUsername.Focus();
                return false;
            }

            // Проверка пароля при добавлении
            if (!_isEditMode && string.IsNullOrWhiteSpace(TxtPassword.Password))
            {
                MessageBox.Show("Введите пароль", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPassword.Focus();
                return false;
            }

            if (!_isEditMode && TxtPassword.Password.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать минимум 4 символа",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPassword.Focus();
                return false;
            }

            // Проверка пароля при редактировании (если введен)
            if (_isEditMode && !string.IsNullOrWhiteSpace(TxtPassword.Password) && TxtPassword.Password.Length < 4)
            {
                MessageBox.Show("Новый пароль должен содержать минимум 4 символа",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPassword.Focus();
                return false;
            }

            // Проверка фамилии
            if (string.IsNullOrWhiteSpace(TxtLastName.Text))
            {
                MessageBox.Show("Введите фамилию", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtLastName.Focus();
                return false;
            }

            // Проверка имени
            if (string.IsNullOrWhiteSpace(TxtFirstName.Text))
            {
                MessageBox.Show("Введите имя", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtFirstName.Focus();
                return false;
            }

            // Проверка email
            if (!string.IsNullOrWhiteSpace(TxtEmail.Text) && !ValidationHelper.IsValidEmail(TxtEmail.Text))
            {
                MessageBox.Show("Введите корректный email адрес", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail.Focus();
                return false;
            }

            // Проверка телефона
            if (!string.IsNullOrWhiteSpace(TxtPhone.Text) && !ValidationHelper.IsValidPhone(TxtPhone.Text))
            {
                MessageBox.Show("Введите корректный номер телефона", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPhone.Focus();
                return false;
            }

            // Проверка роли
            if (CmbRole.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите роль", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbRole.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Обработчик кнопки "Сохранить"
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (_isEditMode)
                {
                    // Редактирование существующего пользователя
                    _user.LastName = TxtLastName.Text.Trim();
                    _user.FirstName = TxtFirstName.Text.Trim();
                    _user.MiddleName = string.IsNullOrWhiteSpace(TxtMiddleName.Text) 
                        ? null 
                        : TxtMiddleName.Text.Trim();
                    _user.Email = string.IsNullOrWhiteSpace(TxtEmail.Text) 
                        ? null 
                        : TxtEmail.Text.Trim();
                    _user.Phone = string.IsNullOrWhiteSpace(TxtPhone.Text) 
                        ? null 
                        : TxtPhone.Text.Trim();
                    _user.RoleID = (int)CmbRole.SelectedValue;

                    // Обновление пароля, если введен новый
                    if (!string.IsNullOrWhiteSpace(TxtPassword.Password))
                    {
                        _user.PasswordHash = PasswordHasher.HashPassword(TxtPassword.Password);
                    }

                    _usersController.UpdateUser(_user);
                    Username = _user.Username;
                    
                    MessageBox.Show("Пользователь успешно обновлен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Добавление нового пользователя
                    var newUser = new Users
                    {
                        Username = TxtUsername.Text.Trim(),
                        PasswordHash = PasswordHasher.HashPassword(TxtPassword.Password),
                        LastName = TxtLastName.Text.Trim(),
                        FirstName = TxtFirstName.Text.Trim(),
                        MiddleName = string.IsNullOrWhiteSpace(TxtMiddleName.Text) 
                            ? null 
                            : TxtMiddleName.Text.Trim(),
                        Email = string.IsNullOrWhiteSpace(TxtEmail.Text) 
                            ? null 
                            : TxtEmail.Text.Trim(),
                        Phone = string.IsNullOrWhiteSpace(TxtPhone.Text) 
                            ? null 
                            : TxtPhone.Text.Trim(),
                        RoleID = (int)CmbRole.SelectedValue,
                        CreatedDate = DateTime.Now
                    };

                    _usersController.AddUser(newUser);
                    Username = newUser.Username;
                    
                    MessageBox.Show("Пользователь успешно добавлен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении пользователя: {ex.Message}",
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

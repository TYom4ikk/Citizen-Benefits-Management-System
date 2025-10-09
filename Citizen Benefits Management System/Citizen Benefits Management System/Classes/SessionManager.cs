using Citizen_Benefits_Management_System.Model;
using System;

namespace Citizen_Benefits_Management_System.Classes
{
    /// <summary>
    /// Класс для управления сессией пользователя
    /// </summary>
    public static class SessionManager
    {
        /// <summary>
        /// Текущий авторизованный пользователь
        /// </summary>
        public static Users CurrentUser { get; private set; }

        /// <summary>
        /// Проверяет, авторизован ли пользователь
        /// </summary>
        public static bool IsAuthenticated => CurrentUser != null;

        /// <summary>
        /// Устанавливает текущего пользователя
        /// </summary>
        /// <param name="user">Пользователь для установки</param>
        public static void SetCurrentUser(Users user)
        {
            CurrentUser = user;
        }

        /// <summary>
        /// Очищает текущую сессию (выход из системы)
        /// </summary>
        public static void Logout()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Проверяет, является ли текущий пользователь администратором
        /// </summary>
        /// <returns>True, если пользователь администратор</returns>
        public static bool IsAdmin()
        {
            return IsAuthenticated && CurrentUser.RoleID == 1;
        }

        /// <summary>
        /// Проверяет, является ли текущий пользователь оператором
        /// </summary>
        /// <returns>True, если пользователь оператор</returns>
        public static bool IsOperator()
        {
            return IsAuthenticated && CurrentUser.RoleID == 2;
        }

        /// <summary>
        /// Проверяет, является ли текущий пользователь гражданином
        /// </summary>
        /// <returns>True, если пользователь гражданин</returns>
        public static bool IsCitizen()
        {
            return IsAuthenticated && CurrentUser.RoleID == 3;
        }

        /// <summary>
        /// Получает полное имя текущего пользователя
        /// </summary>
        /// <returns>Полное имя пользователя</returns>
        public static string GetFullName()
        {
            if (!IsAuthenticated)
                return string.Empty;

            return $"{CurrentUser.LastName} {CurrentUser.FirstName} {CurrentUser.MiddleName}".Trim();
        }
    }
}

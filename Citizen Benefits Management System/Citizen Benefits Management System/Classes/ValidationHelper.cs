using System;
using System.Text.RegularExpressions;

namespace Citizen_Benefits_Management_System.Classes
{
    /// <summary>
    /// Класс для валидации вводимых данных
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Проверяет корректность email адреса
        /// </summary>
        /// <param name="email">Email для проверки</param>
        /// <returns>True, если email корректен</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет корректность номера телефона
        /// </summary>
        /// <param name="phone">Номер телефона для проверки</param>
        /// <returns>True, если номер телефона корректен</returns>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string pattern = @"^(\+7|8)?[\s\-]?\(?[489][0-9]{2}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$";
            return Regex.IsMatch(phone, pattern);
        }

        /// <summary>
        /// Проверяет корректность СНИЛС
        /// </summary>
        /// <param name="snils">СНИЛС для проверки</param>
        /// <returns>True, если СНИЛС корректен</returns>
        public static bool IsValidSNILS(string snils)
        {
            if (string.IsNullOrWhiteSpace(snils))
                return false;

            // Удаляем все символы кроме цифр
            string cleanSnils = Regex.Replace(snils, @"[^\d]", "");

            if (cleanSnils.Length != 11)
                return false;

            // Проверка контрольной суммы СНИЛС
            int[] digits = new int[9];
            for (int i = 0; i < 9; i++)
            {
                digits[i] = int.Parse(cleanSnils[i].ToString());
            }

            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += digits[i] * (9 - i);
            }

            int checkSum = 0;
            if (sum < 100)
            {
                checkSum = sum;
            }
            else if (sum > 101)
            {
                checkSum = sum % 101;
                if (checkSum == 100)
                    checkSum = 0;
            }

            int providedCheckSum = int.Parse(cleanSnils.Substring(9, 2));
            return checkSum == providedCheckSum;
        }

        /// <summary>
        /// Проверяет, что строка не пустая и не состоит только из пробелов
        /// </summary>
        /// <param name="value">Строка для проверки</param>
        /// <returns>True, если строка не пустая</returns>
        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Проверяет, что строка содержит только буквы и пробелы
        /// </summary>
        /// <param name="value">Строка для проверки</param>
        /// <returns>True, если строка содержит только буквы</returns>
        public static bool IsOnlyLetters(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return Regex.IsMatch(value, @"^[а-яА-ЯёЁa-zA-Z\s\-]+$");
        }

        /// <summary>
        /// Проверяет минимальную длину строки
        /// </summary>
        /// <param name="value">Строка для проверки</param>
        /// <param name="minLength">Минимальная длина</param>
        /// <returns>True, если длина строки больше или равна минимальной</returns>
        public static bool HasMinLength(string value, int minLength)
        {
            return !string.IsNullOrEmpty(value) && value.Length >= minLength;
        }

        /// <summary>
        /// Проверяет максимальную длину строки
        /// </summary>
        /// <param name="value">Строка для проверки</param>
        /// <param name="maxLength">Максимальная длина</param>
        /// <returns>True, если длина строки меньше или равна максимальной</returns>
        public static bool HasMaxLength(string value, int maxLength)
        {
            return string.IsNullOrEmpty(value) || value.Length <= maxLength;
        }

        /// <summary>
        /// Проверяет, что дата не в будущем
        /// </summary>
        /// <param name="date">Дата для проверки</param>
        /// <returns>True, если дата не в будущем</returns>
        public static bool IsNotFutureDate(DateTime date)
        {
            return date <= DateTime.Now;
        }

        /// <summary>
        /// Проверяет, что дата рождения корректна (возраст от 0 до 150 лет)
        /// </summary>
        /// <param name="birthDate">Дата рождения</param>
        /// <returns>True, если дата рождения корректна</returns>
        public static bool IsValidBirthDate(DateTime birthDate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthDate.Year;
            
            if (birthDate > now.AddYears(-age))
                age--;

            return age >= 0 && age <= 150;
        }

        /// <summary>
        /// Форматирует СНИЛС в стандартный вид XXX-XXX-XXX XX
        /// </summary>
        /// <param name="snils">СНИЛС для форматирования</param>
        /// <returns>Отформатированный СНИЛС</returns>
        public static string FormatSNILS(string snils)
        {
            if (string.IsNullOrWhiteSpace(snils))
                return string.Empty;

            string cleanSnils = Regex.Replace(snils, @"[^\d]", "");

            if (cleanSnils.Length == 11)
            {
                return $"{cleanSnils.Substring(0, 3)}-{cleanSnils.Substring(3, 3)}-{cleanSnils.Substring(6, 3)} {cleanSnils.Substring(9, 2)}";
            }

            return snils;
        }

        /// <summary>
        /// Форматирует номер телефона в стандартный вид +7 (XXX) XXX-XX-XX
        /// </summary>
        /// <param name="phone">Номер телефона для форматирования</param>
        /// <returns>Отформатированный номер телефона</returns>
        public static string FormatPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;

            string cleanPhone = Regex.Replace(phone, @"[^\d]", "");

            if (cleanPhone.Length == 11 && cleanPhone[0] == '8')
            {
                cleanPhone = "7" + cleanPhone.Substring(1);
            }

            if (cleanPhone.Length == 11 && cleanPhone[0] == '7')
            {
                return $"+7 ({cleanPhone.Substring(1, 3)}) {cleanPhone.Substring(4, 3)}-{cleanPhone.Substring(7, 2)}-{cleanPhone.Substring(9, 2)}";
            }

            return phone;
        }
    }
}

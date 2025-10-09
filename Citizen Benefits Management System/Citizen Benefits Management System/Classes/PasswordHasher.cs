using System;
using System.Security.Cryptography;
using System.Text;

namespace Citizen_Benefits_Management_System.Classes
{
    /// <summary>
    /// Класс для хеширования паролей с использованием SHA-256
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Хеширует пароль с использованием алгоритма SHA-256
        /// </summary>
        /// <param name="password">Пароль для хеширования</param>
        /// <returns>Хешированный пароль в виде строки</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Пароль не может быть пустым", nameof(password));
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                
                return builder.ToString();
            }
        }

        /// <summary>
        /// Проверяет соответствие пароля его хешу
        /// </summary>
        /// <param name="password">Введенный пароль</param>
        /// <param name="hash">Хеш пароля из базы данных</param>
        /// <returns>True, если пароль соответствует хешу</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            string hashedPassword = HashPassword(password);
            return hashedPassword.Equals(hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}

using Citizen_Benefits_Management_System.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Citizen_Benefits_Management_System.Classes.Controllers
{
    /// <summary>
    /// Контроллер для работы с пользователями системы
    /// </summary>
    public class UsersController
    {
        private readonly BenefitsManagementSystemEntities _context;

        public UsersController()
        {
            _context = new BenefitsManagementSystemEntities();
        }

        /// <summary>
        /// Получает всех пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        public List<Users> GetAll()
        {
            return _context.Users.Include(u => u.UserRoles).ToList();
        }

        /// <summary>
        /// Получает пользователя по ID
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Пользователь или null</returns>
        public Users GetById(int userId)
        {
            return _context.Users.Include(u => u.UserRoles).FirstOrDefault(u => u.UserID == userId);
        }

        /// <summary>
        /// Получает пользователя по имени пользователя
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <returns>Пользователь или null</returns>
        public Users GetByUsername(string username)
        {
            return _context.Users.Include(u => u.UserRoles)
                .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Проверяет учетные данные пользователя
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="passwordHash">Хеш пароля</param>
        /// <returns>Пользователь или null, если учетные данные неверны</returns>
        public Users Authenticate(string username, string passwordHash)
        {
            return _context.Users.Include(u => u.UserRoles)
                .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) 
                    && u.PasswordHash == passwordHash 
                    && u.IsActive == true);
        }

        /// <summary>
        /// Добавляет нового пользователя
        /// </summary>
        /// <param name="user">Пользователь для добавления</param>
        /// <returns>True, если добавление успешно</returns>
        public bool Add(Users user)
        {
            try
            {
                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Обновляет данные пользователя
        /// </summary>
        /// <param name="user">Пользователь с обновленными данными</param>
        /// <returns>True, если обновление успешно</returns>
        public bool Update(Users user)
        {
            try
            {
                var existingUser = _context.Users.Find(user.UserID);
                if (existingUser == null)
                    return false;

                existingUser.Username = user.Username;
                existingUser.PasswordHash = user.PasswordHash;
                existingUser.Email = user.Email;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.MiddleName = user.MiddleName;
                existingUser.RoleID = user.RoleID;
                existingUser.IsActive = user.IsActive;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Удаляет пользователя (мягкое удаление - деактивация)
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>True, если удаление успешно</returns>
        public bool Delete(int userId)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user == null)
                    return false;

                user.IsActive = false;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Обновляет время последнего входа пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        public void UpdateLastLogin(int userId)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user != null)
                {
                    user.LastLogin = DateTime.Now;
                    _context.SaveChanges();
                }
            }
            catch
            {
                // Игнорируем ошибки обновления времени входа
            }
        }

        /// <summary>
        /// Проверяет, существует ли пользователь с таким именем
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="excludeUserId">ID пользователя для исключения из проверки (для редактирования)</param>
        /// <returns>True, если пользователь существует</returns>
        public bool UsernameExists(string username, int? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.UserID != excludeUserId.Value);
            }

            return query.Any();
        }

        /// <summary>
        /// Получает активных пользователей
        /// </summary>
        /// <returns>Список активных пользователей</returns>
        public List<Users> GetActiveUsers()
        {
            return _context.Users.Include(u => u.UserRoles)
                .Where(u => u.IsActive == true)
                .ToList();
        }

        /// <summary>
        /// Получает пользователей по роли
        /// </summary>
        /// <param name="roleId">ID роли</param>
        /// <returns>Список пользователей с указанной ролью</returns>
        public List<Users> GetByRole(int roleId)
        {
            return _context.Users.Include(u => u.UserRoles)
                .Where(u => u.RoleID == roleId && u.IsActive == true)
                .ToList();
        }
    }
}

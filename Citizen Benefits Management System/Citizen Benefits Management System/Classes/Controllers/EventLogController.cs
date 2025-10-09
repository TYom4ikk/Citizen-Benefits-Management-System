using Citizen_Benefits_Management_System.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Citizen_Benefits_Management_System.Classes.Controllers
{
    /// <summary>
    /// Контроллер для работы с журналом событий
    /// </summary>
    public class EventLogController
    {
        private readonly BenefitsManagementSystemEntities _context;

        public EventLogController()
        {
            _context = new BenefitsManagementSystemEntities();
        }

        /// <summary>
        /// Получает все записи журнала событий
        /// </summary>
        /// <returns>Список записей журнала</returns>
        public List<EventLog> GetAll()
        {
            return _context.EventLog
                .Include(e => e.Users)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        /// <summary>
        /// Получает запись журнала по ID
        /// </summary>
        /// <param name="logId">ID записи</param>
        /// <returns>Запись журнала или null</returns>
        public EventLog GetById(int logId)
        {
            return _context.EventLog
                .Include(e => e.Users)
                .FirstOrDefault(e => e.LogID == logId);
        }

        /// <summary>
        /// Добавляет новую запись в журнал событий
        /// </summary>
        /// <param name="eventLog">Запись для добавления</param>
        /// <returns>True, если добавление успешно</returns>
        public bool Add(EventLog eventLog)
        {
            try
            {
                eventLog.CreatedAt = DateTime.Now;
                _context.EventLog.Add(eventLog);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Логирует событие в системе
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="eventType">Тип события</param>
        /// <param name="description">Описание события</param>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="entityId">ID сущности</param>
        public void LogEvent(int? userId, string eventType, string description, 
            string entityType = null, int? entityId = null)
        {
            try
            {
                var eventLog = new EventLog
                {
                    UserID = userId,
                    EventType = eventType,
                    EventDescription = description,
                    EntityType = entityType,
                    EntityID = entityId,
                    CreatedAt = DateTime.Now
                };

                Add(eventLog);
            }
            catch
            {
                // Игнорируем ошибки логирования
            }
        }

        /// <summary>
        /// Получает записи журнала по пользователю
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список записей журнала пользователя</returns>
        public List<EventLog> GetByUser(int userId)
        {
            return _context.EventLog
                .Include(e => e.Users)
                .Where(e => e.UserID == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        /// <summary>
        /// Получает записи журнала за период
        /// </summary>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <returns>Список записей журнала за период</returns>
        public List<EventLog> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.EventLog
                .Include(e => e.Users)
                .Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        /// <summary>
        /// Получает записи журнала по пользователю и периоду
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="startDate">Начальная дата</param>
        /// <param name="endDate">Конечная дата</param>
        /// <returns>Список записей журнала</returns>
        public List<EventLog> GetByUserAndDateRange(int userId, DateTime startDate, DateTime endDate)
        {
            return _context.EventLog
                .Include(e => e.Users)
                .Where(e => e.UserID == userId && e.CreatedAt >= startDate && e.CreatedAt <= endDate)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        /// <summary>
        /// Получает записи журнала по типу события
        /// </summary>
        /// <param name="eventType">Тип события</param>
        /// <returns>Список записей журнала</returns>
        public List<EventLog> GetByEventType(string eventType)
        {
            return _context.EventLog
                .Include(e => e.Users)
                .Where(e => e.EventType == eventType)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        /// <summary>
        /// Фильтрует записи журнала
        /// </summary>
        /// <param name="userId">ID пользователя (null для всех)</param>
        /// <param name="startDate">Начальная дата (null для всех)</param>
        /// <param name="endDate">Конечная дата (null для всех)</param>
        /// <param name="eventType">Тип события (null для всех)</param>
        /// <returns>Отфильтрованный список записей</returns>
        public List<EventLog> Filter(int? userId = null, DateTime? startDate = null, 
            DateTime? endDate = null, string eventType = null)
        {
            var query = _context.EventLog.Include(e => e.Users).AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(e => e.UserID == userId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(e => e.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(e => e.CreatedAt <= endDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(eventType))
            {
                query = query.Where(e => e.EventType == eventType);
            }

            return query.OrderByDescending(e => e.CreatedAt).ToList();
        }

        /// <summary>
        /// Получает последние N записей журнала
        /// </summary>
        /// <param name="count">Количество записей</param>
        /// <returns>Список последних записей</returns>
        public List<EventLog> GetLatest(int count = 100)
        {
            return _context.EventLog
                .Include(e => e.Users)
                .OrderByDescending(e => e.CreatedAt)
                .Take(count)
                .ToList();
        }
    }
}

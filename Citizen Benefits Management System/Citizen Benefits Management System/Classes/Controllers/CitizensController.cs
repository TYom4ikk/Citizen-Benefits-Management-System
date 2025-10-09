using Citizen_Benefits_Management_System.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Citizen_Benefits_Management_System.Classes.Controllers
{
    /// <summary>
    /// Контроллер для работы с гражданами
    /// </summary>
    public class CitizensController
    {
        private readonly BenefitsManagementSystemEntities _context;

        public CitizensController()
        {
            _context = new BenefitsManagementSystemEntities();
        }

        /// <summary>
        /// Получает всех граждан
        /// </summary>
        /// <returns>Список граждан</returns>
        public List<Citizens> GetAll()
        {
            return _context.Citizens.Include(c => c.Regions).ToList();
        }

        /// <summary>
        /// Получает гражданина по ID
        /// </summary>
        /// <param name="citizenId">ID гражданина</param>
        /// <returns>Гражданин или null</returns>
        public Citizens GetById(int citizenId)
        {
            return _context.Citizens.Include(c => c.Regions).FirstOrDefault(c => c.CitizenID == citizenId);
        }

        /// <summary>
        /// Получает гражданина по СНИЛС
        /// </summary>
        /// <param name="snils">СНИЛС</param>
        /// <returns>Гражданин или null</returns>
        public Citizens GetBySNILS(string snils)
        {
            return _context.Citizens.Include(c => c.Regions)
                .FirstOrDefault(c => c.SNILS == snils);
        }

        /// <summary>
        /// Добавляет нового гражданина
        /// </summary>
        /// <param name="citizen">Гражданин для добавления</param>
        /// <returns>True, если добавление успешно</returns>
        public bool Add(Citizens citizen)
        {
            try
            {
                citizen.CreatedAt = DateTime.Now;
                citizen.IsActive = true;
                _context.Citizens.Add(citizen);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Обновляет данные гражданина
        /// </summary>
        /// <param name="citizen">Гражданин с обновленными данными</param>
        /// <returns>True, если обновление успешно</returns>
        public bool Update(Citizens citizen)
        {
            try
            {
                var existingCitizen = _context.Citizens.Find(citizen.CitizenID);
                if (existingCitizen == null)
                    return false;

                existingCitizen.LastName = citizen.LastName;
                existingCitizen.FirstName = citizen.FirstName;
                existingCitizen.MiddleName = citizen.MiddleName;
                existingCitizen.BirthDate = citizen.BirthDate;
                existingCitizen.SNILS = citizen.SNILS;
                existingCitizen.PhoneNumber = citizen.PhoneNumber;
                existingCitizen.Email = citizen.Email;
                existingCitizen.Address = citizen.Address;
                existingCitizen.RegionID = citizen.RegionID;
                existingCitizen.IsActive = citizen.IsActive;
                existingCitizen.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Удаляет гражданина (мягкое удаление - деактивация)
        /// </summary>
        /// <param name="citizenId">ID гражданина</param>
        /// <returns>True, если удаление успешно</returns>
        public bool Delete(int citizenId)
        {
            try
            {
                var citizen = _context.Citizens.Find(citizenId);
                if (citizen == null)
                    return false;

                citizen.IsActive = false;
                citizen.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет, существует ли гражданин с таким СНИЛС
        /// </summary>
        /// <param name="snils">СНИЛС</param>
        /// <param name="excludeCitizenId">ID гражданина для исключения из проверки</param>
        /// <returns>True, если гражданин существует</returns>
        public bool SNILSExists(string snils, int? excludeCitizenId = null)
        {
            var query = _context.Citizens.Where(c => c.SNILS == snils);
            
            if (excludeCitizenId.HasValue)
            {
                query = query.Where(c => c.CitizenID != excludeCitizenId.Value);
            }

            return query.Any();
        }

        /// <summary>
        /// Получает активных граждан
        /// </summary>
        /// <returns>Список активных граждан</returns>
        public List<Citizens> GetActiveCitizens()
        {
            return _context.Citizens.Include(c => c.Regions)
                .Where(c => c.IsActive == true)
                .ToList();
        }

        /// <summary>
        /// Поиск граждан по ФИО
        /// </summary>
        /// <param name="searchText">Текст для поиска</param>
        /// <returns>Список найденных граждан</returns>
        public List<Citizens> SearchByName(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetActiveCitizens();

            searchText = searchText.ToLower();
            return _context.Citizens.Include(c => c.Regions)
                .Where(c => c.IsActive == true &&
                    (c.LastName.ToLower().Contains(searchText) ||
                     c.FirstName.ToLower().Contains(searchText) ||
                     c.MiddleName.ToLower().Contains(searchText)))
                .ToList();
        }

        /// <summary>
        /// Получает граждан по региону
        /// </summary>
        /// <param name="regionId">ID региона</param>
        /// <returns>Список граждан из указанного региона</returns>
        public List<Citizens> GetByRegion(int regionId)
        {
            return _context.Citizens.Include(c => c.Regions)
                .Where(c => c.RegionID == regionId && c.IsActive == true)
                .ToList();
        }

        /// <summary>
        /// Получает льготы гражданина
        /// </summary>
        /// <param name="citizenId">ID гражданина</param>
        /// <returns>Список льгот гражданина</returns>
        public List<CitizenBenefits> GetCitizenBenefits(int citizenId)
        {
            return _context.CitizenBenefits
                .Include(cb => cb.BenefitCategories)
                .Include(cb => cb.Users)
                .Where(cb => cb.CitizenID == citizenId)
                .OrderByDescending(cb => cb.CreatedAt)
                .ToList();
        }

        /// <summary>
        /// Получает документы гражданина
        /// </summary>
        /// <param name="citizenId">ID гражданина</param>
        /// <returns>Список документов гражданина</returns>
        public List<CitizenDocuments> GetCitizenDocuments(int citizenId)
        {
            return _context.CitizenDocuments
                .Include(cd => cd.DocumentTypes)
                .Where(cd => cd.CitizenID == citizenId)
                .ToList();
        }

        /// <summary>
        /// Получает справки гражданина
        /// </summary>
        /// <param name="citizenId">ID гражданина</param>
        /// <returns>Список справок гражданина</returns>
        public List<Certificates> GetCitizenCertificates(int citizenId)
        {
            return _context.Certificates
                .Include(c => c.BenefitCategories)
                .Include(c => c.Users)
                .Where(c => c.CitizenID == citizenId)
                .OrderByDescending(c => c.IssueDate)
                .ToList();
        }
    }
}

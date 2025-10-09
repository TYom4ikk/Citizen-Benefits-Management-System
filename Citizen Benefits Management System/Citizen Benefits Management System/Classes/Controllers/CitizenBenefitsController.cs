using Citizen_Benefits_Management_System.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Citizen_Benefits_Management_System.Classes.Controllers
{
    /// <summary>
    /// Контроллер для управления льготами граждан
    /// </summary>
    public class CitizenBenefitsController
    {
        /// <summary>
        /// Получить все льготы гражданина
        /// </summary>
        public List<CitizenBenefits> GetByCitizenId(int citizenId)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.CitizenBenefits
                    .Include(cb => cb.BenefitCategories)
                    .Where(cb => cb.CitizenID == citizenId)
                    .OrderByDescending(cb => cb.StartDate)
                    .ToList();
            }
        }

        /// <summary>
        /// Получить активные льготы гражданина
        /// </summary>
        public List<CitizenBenefits> GetActiveByCitizenId(int citizenId)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                var today = DateTime.Today;
                return context.CitizenBenefits
                    .Include(cb => cb.BenefitCategories)
                    .Where(cb => cb.CitizenID == citizenId && 
                                 cb.Status == "Активна" &&
                                 (!cb.EndDate.HasValue || cb.EndDate.Value >= today))
                    .OrderByDescending(cb => cb.StartDate)
                    .ToList();
            }
        }

        /// <summary>
        /// Получить льготу по ID
        /// </summary>
        public CitizenBenefits GetById(int benefitId)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.CitizenBenefits
                    .Include(cb => cb.BenefitCategories)
                    .Include(cb => cb.Citizens)
                    .FirstOrDefault(cb => cb.CitizenBenefitID == benefitId);
            }
        }

        /// <summary>
        /// Добавить новую льготу
        /// </summary>
        public bool Add(CitizenBenefits benefit)
        {
            try
            {
                using (var context = new BenefitsManagementSystemEntities())
                {
                    benefit.CreatedAt = DateTime.Now;
                    benefit.UpdatedAt = DateTime.Now;
                    benefit.Status = "Активна";
                    
                    context.CitizenBenefits.Add(benefit);
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Обновить льготу
        /// </summary>
        public bool Update(CitizenBenefits benefit)
        {
            try
            {
                using (var context = new BenefitsManagementSystemEntities())
                {
                    var existing = context.CitizenBenefits.Find(benefit.CitizenBenefitID);
                    if (existing == null) return false;

                    existing.CategoryID = benefit.CategoryID;
                    existing.EndDate = benefit.EndDate;
                    existing.BenefitNumber = benefit.BenefitNumber;
                    existing.Description = benefit.Description;
                    existing.Status = benefit.Status;
                    existing.UpdatedAt = DateTime.Now;

                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Деактивировать льготу
        /// </summary>
        public bool Deactivate(int benefitId)
        {
            try
            {
                using (var context = new BenefitsManagementSystemEntities())
                {
                    var benefit = context.CitizenBenefits.Find(benefitId);
                    if (benefit == null) return false;

                    benefit.Status = "Неактивна";
                    benefit.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Удалить льготу
        /// </summary>
        public bool Delete(int benefitId)
        {
            try
            {
                using (var context = new BenefitsManagementSystemEntities())
                {
                    var benefit = context.CitizenBenefits.Find(benefitId);
                    if (benefit == null) return false;

                    context.CitizenBenefits.Remove(benefit);
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Получить статистику по категориям льгот
        /// </summary>
        public Dictionary<string, int> GetStatisticsByCategory()
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.CitizenBenefits
                    .Where(cb => cb.Status == "Активна")
                    .GroupBy(cb => cb.BenefitCategories.CategoryName)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .ToDictionary(x => x.Category, x => x.Count);
            }
        }

        /// <summary>
        /// Проверить, есть ли у гражданина активная льгота данной категории
        /// </summary>
        public bool HasActiveCategory(int citizenId, int categoryId)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                var today = DateTime.Today;
                return context.CitizenBenefits.Any(cb => 
                    cb.CitizenID == citizenId && 
                    cb.CategoryID == categoryId &&
                    cb.Status == "Активна" &&
                    (!cb.EndDate.HasValue || cb.EndDate.Value >= today));
            }
        }

        /// <summary>
        /// Получить льготы с истекающим сроком действия (в течение указанного количества дней)
        /// </summary>
        public List<CitizenBenefits> GetExpiringBenefits(int daysAhead = 30)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                var today = DateTime.Today;
                var futureDate = today.AddDays(daysAhead);

                return context.CitizenBenefits
                    .Include(cb => cb.BenefitCategories)
                    .Include(cb => cb.Citizens)
                    .Where(cb => cb.Status == "Активна" &&
                                 cb.EndDate.HasValue &&
                                 cb.EndDate.Value >= today &&
                                 cb.EndDate.Value <= futureDate)
                    .OrderBy(cb => cb.EndDate)
                    .ToList();
            }
        }
    }
}

using Citizen_Benefits_Management_System.Model;
using System.Collections.Generic;
using System.Linq;

namespace Citizen_Benefits_Management_System.Classes.Controllers
{
    /// <summary>
    /// Контроллер для управления регионами
    /// </summary>
    public class RegionsController
    {
        /// <summary>
        /// Получить все регионы
        /// </summary>
        public List<Regions> GetAll()
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Regions.OrderBy(r => r.RegionName).ToList();
            }
        }

        /// <summary>
        /// Получить регион по ID
        /// </summary>
        public Regions GetById(int regionId)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Regions.Find(regionId);
            }
        }

        /// <summary>
        /// Добавить новый регион
        /// </summary>
        public bool Add(Regions region)
        {
            try
            {
                using (var context = new BenefitsManagementSystemEntities())
                {
                    context.Regions.Add(region);
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
        /// Обновить регион
        /// </summary>
        public bool Update(Regions region)
        {
            try
            {
                using (var context = new BenefitsManagementSystemEntities())
                {
                    var existing = context.Regions.Find(region.RegionID);
                    if (existing == null) return false;

                    existing.RegionName = region.RegionName;
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
        /// Получить количество граждан по регионам
        /// </summary>
        public Dictionary<string, int> GetCitizensCountByRegion()
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Citizens
                    .Where(c => c.IsActive == true && c.RegionID.HasValue)
                    .GroupBy(c => c.Regions.RegionName)
                    .Select(g => new { Region = g.Key, Count = g.Count() })
                    .ToDictionary(x => x.Region, x => x.Count);
            }
        }
    }
}

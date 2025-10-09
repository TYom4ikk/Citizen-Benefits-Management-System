using Citizen_Benefits_Management_System.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Citizen_Benefits_Management_System.Classes.Controllers
{
    /// <summary>
    /// Контроллер для работы с категориями льгот
    /// </summary>
    public class BenefitCategoriesController
    {
        private readonly BenefitsManagementSystemEntities _context;

        public BenefitCategoriesController()
        {
            _context = new BenefitsManagementSystemEntities();
        }

        /// <summary>
        /// Получает все категории льгот
        /// </summary>
        /// <returns>Список категорий льгот</returns>
        public List<BenefitCategories> GetAll()
        {
            return _context.BenefitCategories.ToList();
        }

        /// <summary>
        /// Получает категорию льгот по ID
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Категория льгот или null</returns>
        public BenefitCategories GetById(int categoryId)
        {
            return _context.BenefitCategories.Find(categoryId);
        }

        /// <summary>
        /// Добавляет новую категорию льгот
        /// </summary>
        /// <param name="category">Категория для добавления</param>
        /// <returns>True, если добавление успешно</returns>
        public bool Add(BenefitCategories category)
        {
            try
            {
                category.IsActive = true;
                _context.BenefitCategories.Add(category);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Обновляет категорию льгот
        /// </summary>
        /// <param name="category">Категория с обновленными данными</param>
        /// <returns>True, если обновление успешно</returns>
        public bool Update(BenefitCategories category)
        {
            try
            {
                var existingCategory = _context.BenefitCategories.Find(category.CategoryID);
                if (existingCategory == null)
                    return false;

                existingCategory.CategoryName = category.CategoryName;
                existingCategory.Description = category.Description;
                existingCategory.LegalBasis = category.LegalBasis;
                existingCategory.IsActive = category.IsActive;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Удаляет категорию льгот (мягкое удаление - деактивация)
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>True, если удаление успешно</returns>
        public bool Delete(int categoryId)
        {
            try
            {
                var category = _context.BenefitCategories.Find(categoryId);
                if (category == null)
                    return false;

                category.IsActive = false;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Получает активные категории льгот
        /// </summary>
        /// <returns>Список активных категорий</returns>
        public List<BenefitCategories> GetActiveCategories()
        {
            return _context.BenefitCategories
                .Where(c => c.IsActive == true)
                .ToList();
        }

        /// <summary>
        /// Проверяет, существует ли категория с таким названием
        /// </summary>
        /// <param name="categoryName">Название категории</param>
        /// <param name="excludeCategoryId">ID категории для исключения из проверки</param>
        /// <returns>True, если категория существует</returns>
        public bool CategoryNameExists(string categoryName, int? excludeCategoryId = null)
        {
            var query = _context.BenefitCategories
                .Where(c => c.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            
            if (excludeCategoryId.HasValue)
            {
                query = query.Where(c => c.CategoryID != excludeCategoryId.Value);
            }

            return query.Any();
        }

        /// <summary>
        /// Получает количество граждан по категории льгот
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Количество граждан</returns>
        public int GetCitizenCountByCategory(int categoryId)
        {
            return _context.CitizenBenefits
                .Where(cb => cb.CategoryID == categoryId && cb.Status == "Активна")
                .Select(cb => cb.CitizenID)
                .Distinct()
                .Count();
        }

        /// <summary>
        /// Получает статистику по категориям льгот
        /// </summary>
        /// <returns>Словарь: категория -> количество граждан</returns>
        public Dictionary<string, int> GetCategoryStatistics()
        {
            var statistics = new Dictionary<string, int>();
            var categories = GetActiveCategories();

            foreach (var category in categories)
            {
                int count = GetCitizenCountByCategory(category.CategoryID);
                statistics[category.CategoryName] = count;
            }

            return statistics;
        }
    }
}

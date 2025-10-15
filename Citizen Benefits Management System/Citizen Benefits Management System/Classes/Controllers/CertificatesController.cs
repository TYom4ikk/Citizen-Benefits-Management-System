using Citizen_Benefits_Management_System.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Citizen_Benefits_Management_System.Classes.Controllers
{
    /// <summary>
    /// Контроллер для работы со справками
    /// </summary>
    public class CertificatesController
    {
        /// <summary>
        /// Получает все справки
        /// </summary>
        public List<Certificates> GetAllCertificates()
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Certificates
                    .Include(c => c.Citizens)
                    .Include(c => c.Users)
                    .OrderByDescending(c => c.IssueDate)
                    .ToList();
            }
        }

        /// <summary>
        /// Получает справку по ID
        /// </summary>
        public Certificates GetCertificateById(int certificateId)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Certificates
                    .Include(c => c.Citizens)
                    .Include(c => c.Users)
                    .FirstOrDefault(c => c.CertificateID == certificateId);
            }
        }

        /// <summary>
        /// Получает справки по гражданину
        /// </summary>
        public List<Certificates> GetCertificatesByCitizen(int citizenId)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Certificates
                    .Include(c => c.Users)
                    .Where(c => c.CitizenID == citizenId)
                    .OrderByDescending(c => c.IssueDate)
                    .ToList();
            }
        }

        /// <summary>
        /// Получает справки за период
        /// </summary>
        public List<Certificates> GetCertificatesByDateRange(DateTime startDate, DateTime endDate)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Certificates
                    .Include(c => c.Citizens)
                    .Include(c => c.Users)
                    .Where(c => c.IssueDate >= startDate && c.IssueDate <= endDate)
                    .OrderByDescending(c => c.IssueDate)
                    .ToList();
            }
        }

        /// <summary>
        /// Получает справки по типу
        /// </summary>
        public List<Certificates> GetCertificatesByType(string certificateType)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Certificates
                    .Include(c => c.Citizens)
                    .Include(c => c.Users)
                    .Where(c => c.CertificateType == certificateType)
                    .OrderByDescending(c => c.IssueDate)
                    .ToList();
            }
        }

        /// <summary>
        /// Добавляет новую справку
        /// </summary>
        public void AddCertificate(Certificates certificate)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                context.Certificates.Add(certificate);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Обновляет справку
        /// </summary>
        public void UpdateCertificate(Certificates certificate)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                var existingCertificate = context.Certificates.Find(certificate.CertificateID);
                if (existingCertificate != null)
                {
                    existingCertificate.CitizenID = certificate.CitizenID;
                    existingCertificate.CertificateType = certificate.CertificateType;
                    existingCertificate.IssueDate = certificate.IssueDate;
                    existingCertificate.IssuedByUserID = certificate.IssuedByUserID;
                    existingCertificate.Notes = certificate.Notes;
                    
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Удаляет справку
        /// </summary>
        public void DeleteCertificate(int certificateId)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                var certificate = context.Certificates.Find(certificateId);
                if (certificate != null)
                {
                    context.Certificates.Remove(certificate);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Получает статистику по справкам
        /// </summary>
        public Dictionary<string, int> GetCertificateStatistics()
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Certificates
                    .GroupBy(c => c.CertificateType)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
        }

        /// <summary>
        /// Получает количество справок за период
        /// </summary>
        public int GetCertificateCountByDateRange(DateTime startDate, DateTime endDate)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Certificates
                    .Count(c => c.IssueDate >= startDate && c.IssueDate <= endDate);
            }
        }

        /// <summary>
        /// Поиск справок по ФИО гражданина
        /// </summary>
        public List<Certificates> SearchCertificatesByFullName(string searchText)
        {
            using (var context = new BenefitsManagementSystemEntities())
            {
                return context.Certificates
                    .Include(c => c.Citizens)
                    .Include(c => c.Users)
                    .Where(c => 
                        c.Citizens.LastName.Contains(searchText) ||
                        c.Citizens.FirstName.Contains(searchText) ||
                        c.Citizens.MiddleName.Contains(searchText))
                    .OrderByDescending(c => c.IssueDate)
                    .ToList();
            }
        }
    }
}

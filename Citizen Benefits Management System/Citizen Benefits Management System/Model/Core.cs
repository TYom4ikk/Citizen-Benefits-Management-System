using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citizen_Benefits_Management_System.Model
{
    /// <summary>
    /// Статический класс, предоставляющий доступ к контексту базы данных
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// Статический экземпляр контекста базы данных GardenStoreEntities
        /// </summary>
        public static BenefitsManagementSystemEntities context = new BenefitsManagementSystemEntities();
    }
}

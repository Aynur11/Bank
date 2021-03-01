using ClientLib.ClientFeatures;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Homework_17.Departments
{
    /// <summary>
    /// Расширения.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Возварщает список с именами клиентов.
        /// </summary>
        /// <param name="clients">Лист с клиентами.</param>
        /// <returns></returns>
        public static List<string> GetClientsNamesWithId(this ObservableCollection<ClientBase> clients)
        {
            return clients.Select(e => e.Name + ":" + e.Id).ToList();
        }
    }
}
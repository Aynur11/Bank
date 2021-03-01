using ClientLib.ClientFeatures;
using System;
using System.Collections.ObjectModel;

namespace Homework_17.Departments
{
    public class Department
    {
        public Department(string name)
        {
            Name = name;
            Clients = new ObservableCollection<ClientBase>();
        }

        public string Name { get; private set; }
        public ObservableCollection<ClientBase> Clients { get; set; }

        //protected double DepositPercent { get; set; }
        //protected double CreditPercent { get; set; }

        //public Client this[string name]
        //{
        //    get { return Clients.FirstOrDefault(e => e.Name == name); }
        //}

        public static event Action<object, DepartmentEventArgs> OnOperation;

        /// <summary>
        /// Добавляет клиента.
        /// </summary>
        /// <param name="client"></param>
        public void AddClient(ClientBase client)
        {
            Clients.Add(client);
            OnOperation?.Invoke(this, new DepartmentEventArgs(DateTime.Now, $"Выполняется добавление клиента {client.Name}.", Name));
        }


        /// <summary>
        /// Удаляет клиента.
        /// </summary>
        /// <param name="client">Клиент, который надо удалить.</param>
        /// <returns></returns>
        public bool RemoveClient(ClientBase client)
        {
            if (Clients.Remove(client))
            {
                OnOperation?.Invoke(this, new DepartmentEventArgs(DateTime.Now, $"Выполняется удаление клиента {client.Name}.", Name));
                return true;
            }
            return false;
        }
    }
}

using ClientLib.ClientFeatures;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Homework_17.Departments;

namespace Homework_17
{
    public class Bank : INotifyPropertyChanged
    {
        public Bank()
        {
            //LegalPersonsDepartment = new LegalPersonsDepartment();
            //PhysicalPersonsDepartment = new PhysicalPersonsDepartment();
            PhysicalPersonsDepartment = new Department("Physical persons");
            LegalPersonsDepartment = new Department("Legal persons");
        }

        /// <summary>
        /// Департамент для работы с юридическими лицами.
        /// </summary>
        public Department LegalPersonsDepartment { get; set; }

        /// <summary>
        /// Департамент для работы с физическими лицами.
        /// </summary>
        public Department PhysicalPersonsDepartment { get; set; }

        /// <summary>
        /// Количество клиентов.
        /// </summary>
        public int ClientsCount
        {
            get => LegalPersonsDepartment.Clients.Count +
                   PhysicalPersonsDepartment.Clients.Count;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Выполняет поиск клиента.
        /// </summary>
        /// <param name="name">Имя по которому осуществляется поиск.</param>
        /// <returns>Найденный клиент.</returns>
        public ClientBase FindClientById(int id)
        {
            if (PhysicalPersonsDepartment.Clients.Count(e => e.Id == id) == 1)
            {
                return PhysicalPersonsDepartment.Clients.First(e => e.Id == id);
            }
            if (LegalPersonsDepartment.Clients.Count(e => e.Id == id) == 1)
            {
                return LegalPersonsDepartment.Clients.First(e => e.Id == id);
            }
            return null;
        }
        /// <summary>
        /// Удаляет указанного клиента.
        /// </summary>
        /// <param name="client">Клиент.</param>
        /// <returns>Булевый результат удаления.</returns>
        public bool RemoveClient(ClientBase client)
        {
            if (PhysicalPersonsDepartment.RemoveClient(client))
            {
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClientsCount)));
                return true;
            }
            if (LegalPersonsDepartment.RemoveClient(client))
            {
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClientsCount)));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Для тестирования.
        ///// </summary>
        //public void AddNewClientMock()
        //{
        //    PhysicalPersonsDepartment.AddClient("Семен Григорьев");
        //    PhysicalPersonsDepartment["Семен Григорьев"].IsVip = true;
        //    PhysicalPersonsDepartment["Семен Григорьев"].Sum = 210000;
        //    PhysicalPersonsDepartment["Семен Григорьев"].OpenDeposit(100000.0, 12, false);
        //    PhysicalPersonsDepartment["Семен Григорьев"].OpenDeposit(100000.0, 12, true);
        //    PhysicalPersonsDepartment["Семен Григорьев"].Deposits[0].CalcTotalSum();
        //    PhysicalPersonsDepartment["Семен Григорьев"].Deposits[1].CalcTotalSum();
        //    PhysicalPersonsDepartment.AddClient("Катерина Рогозина");
        //    PhysicalPersonsDepartment["Семен Григорьев"].IssueCredit(55000, 7);
        //    PhysicalPersonsDepartment["Семен Григорьев"].Credits[0].CalcOverPayment();

        //    LegalPersonsDepartment.AddClient("ОАО СтройИнвест");
        //    LegalPersonsDepartment["ОАО СтройИнвест"].Sum = 50000;
        //}

        /// <summary>
        /// Добавление клиента.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="isPhysicalPerson"></param>
        public void AddNewClient(ClientBase client, bool isPhysicalPerson)
        {;
            if (isPhysicalPerson)
            {
                PhysicalPersonsDepartment.AddClient(client);
            }
            else
            {
                LegalPersonsDepartment.AddClient(client);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClientsCount)));
        }

        public List<string> GetAllClientsNamesWithId()
        {
            return LegalPersonsDepartment.Clients.GetClientsNamesWithId()
                .Concat(PhysicalPersonsDepartment.Clients.GetClientsNamesWithId()).Distinct().ToList();
        }
    }
}
namespace Homework_17
{
    public class BankProvider
    {
        /// <summary>
        /// Объект для взаимодействия с банком.
        /// </summary>
        public Bank BankObj { get; }

        public BankProvider()
        {
            BankObj = new Bank();
            //AddNewClientMock();
        }

        /// <summary>
        /// Перегрузка для сложения объектов с клиентами.
        /// </summary>
        /// <param name="bankProvider1"></param>
        /// <param name="bankProvider2"></param>
        /// <returns></returns>
        public static BankProvider operator +(BankProvider bankProvider1, BankProvider bankProvider2)
        {
            foreach (var client in bankProvider2.BankObj.LegalPersonsDepartment.Clients)
            {
                bankProvider1.BankObj.AddNewClient(client, false);
            }

            foreach (var client in bankProvider2.BankObj.PhysicalPersonsDepartment.Clients)
            {
                bankProvider1.BankObj.AddNewClient(client, true);
            }
            return bankProvider1;
        }
    }
}

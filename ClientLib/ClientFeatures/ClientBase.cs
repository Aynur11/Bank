using Homework_17.ClientFeatures;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ClientLib.ClientFeatures
{
    public abstract class ClientBase : INotifyPropertyChanged
    {
        private double sum { get; set; }

        private static int id;

        /// <summary>
        /// Базовая ставка.
        /// </summary>
        protected double baseDepositRate = 0.1;

        protected double baseCreditRate = 0.5;

        /// <summary>
        /// Все вклады клиента.
        /// </summary>
        public ObservableCollection<Deposit> Deposits;

        /// <summary>
        /// Все кредиты клиента.
        /// </summary>
        public ObservableCollection<Credit> Credits;

        public delegate void ClientOperationHandler(object sender, ClientEventArgs e);

        // Общее событие для всех клиентов.
        public static event ClientOperationHandler OnOperation;

        public event ClientOperationHandler OnClientInstanceOperation;

        /// <summary>
        /// Событие изменения значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        static ClientBase()
        {
            id = 0;
        }

        public ClientBase(string name)
        {
            Name = name;
            //DepositPercent = depositPercent;
            //CreditPercent = creditPercent;
            Deposits = new ObservableCollection<Deposit>();
            Credits = new ObservableCollection<Credit>();
            Id = id++;
        }

        public int Id { get; set; }

        /// <summary>
        /// VIP клиент?
        /// </summary>
        public bool Vip { get; set; }
        
        /// <summary>
        /// Имя клиента.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сумма денег на счету.
        /// </summary>
        public double Sum
        {
            get => sum;
            set
            {
                sum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sum)));
            }
        }

        /// <summary>
        /// Ставка для кладов.
        /// </summary>
        public double DepositPercent
        {
            get => baseDepositRate;
            set => baseDepositRate = value;
        }

        /// <summary>
        /// Ставка по кредитам.
        /// </summary>
        public double CreditPercent
        {
            get => baseCreditRate;
            set => baseCreditRate = value;
        }

        /// <summary>
        /// Используется для отображения в таблице.
        /// </summary>
        public string AllDeposits => String.Join("\n", Deposits);

        /// <summary>
        /// Используется для отображения в таблице.
        /// </summary>
        public string AllCredits => String.Join("\n", Credits);

        /// <summary>
        /// Выдача кредита.
        /// </summary>
        /// <param name="creditSum"></param>
        /// <param name="periodInMonth"></param>
        public void IssueCredit(double creditSum, int periodInMonth)
        {
            Credits.Add(new Credit(creditSum, CreditPercent, periodInMonth));
            OnClientInstanceOperation?.Invoke(this, new ClientEventArgs(DateTime.Now, Sum, "Выдан кредит"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Vip)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Credits)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllCredits)));
            OnOperation?.Invoke(this, new ClientEventArgs(DateTime.Now, creditSum, $"Клиенту {Name} выдан кредит на сумму: "));
        }

        /// <summary>
        /// Открытие вклада без капитализации.
        /// </summary>
        /// <param name="sum">Сумма вносимых денег.</param>
        /// <param name="periodInMonth">Период действия вклада.</param>
        /// <param name="withCapitalization"></param>
        public void OpenDeposit(double sum, int periodInMonth, bool withCapitalization)
        {
            if (sum <= Sum)
            {
                Deposits.Add(new Deposit(sum, DepositPercent, periodInMonth, withCapitalization));
                Sum -= sum;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllDeposits)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sum)));
                OnOperation?.Invoke(this, new ClientEventArgs(DateTime.Now, sum, $"Клиенту {Name} открыт вклад на сумму: "));
            }
            else
            {
                OnOperation?.Invoke(this, new ClientEventArgs(DateTime.Now, sum, "Недостаточно денег на счету для открытия вклада на сумму: "));
            }
        }

        /// <summary>
        /// Перевод денег клиенту банка.
        /// </summary>
        /// <param name="client">Клиент банка.</param>
        /// <param name="sumToTransfer">Сумма денег для перевода.</param>
        public void TransferMoney(ClientBase client, double sumToTransfer)
        {
            if (client == null)
            {
                OnOperation?.Invoke(this, new ClientEventArgs(DateTime.Now, sumToTransfer, "Клиент не найден для выполнения перевода на сумму: "));
                return;
            }
            if (sumToTransfer > Sum)
            {
                try
                {
                    throw new ClientInsufficientFundsException(Sum, "Недостаточно средств на счету");
                }
                catch (ClientInsufficientFundsException e)
                {
                    OnOperation?.Invoke(this, new ClientEventArgs(DateTime.Now, sumToTransfer, $"Баланс {e.Sum}. {e.Message} для выполнения перевода на сумму: "));

                    return;
                }
            }

            if (this == client)
            {
                try
                {
                    throw new ClientHimselfAccessException("Попытка перевести средства самому себе");
                }
                catch (ClientHimselfAccessException e)
                {
                    OnOperation?.Invoke(this, new ClientEventArgs(DateTime.Now, sumToTransfer, $"{e.Message} при выполнении перевода на сумму: "));
                    return;
                }
            }
            client.Sum += sumToTransfer;
            Sum -= sumToTransfer;
            OnOperation?.Invoke(this, new ClientEventArgs(DateTime.Now, sumToTransfer, $"Клиент {Name} перевёл клиенту {client.Name} сумму: "));
        }
    }
}

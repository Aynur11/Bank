using System.ComponentModel;

namespace ClientLib.ClientFeatures
{
    public class Client : ClientBase, INotifyPropertyChanged
    {
        public Client(string name) : base(name)
        {
            DepositPercent = baseDepositRate + 0.2;
            CreditPercent = baseCreditRate - 0.1;
            Vip = false;
        }
    }
}
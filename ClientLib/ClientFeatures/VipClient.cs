namespace ClientLib.ClientFeatures
{
    public class VipClient : ClientBase
    {
        public VipClient(string name) : base(name)
        {
            DepositPercent = baseDepositRate + 0.3;
            CreditPercent = baseCreditRate - 0.2;
            Vip = true;
        }
    }
}

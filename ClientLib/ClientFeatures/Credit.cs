namespace ClientLib.ClientFeatures
{
    public class Credit
    {
        public Credit(double sum, double percent, int periodInMonth)
        {
            Sum = sum;
            Percent = percent;
            PeriodInMonth = periodInMonth;
        }

        public double Sum { get; set; }
        public double Percent { get; set; }
        public int PeriodInMonth { get; set; }

        /// <summary>
        /// Вычисляет сумму денег, которая получится к концу срока действия вклада.
        /// </summary>
        /// <returns>Сумму денег к концу срока действия вклада.</returns>
        public double CalcOverPayment()
        {
            return Sum * (Percent / 12.0) * PeriodInMonth;
        }

        public override string ToString()
        {
            return $"{Sum:##,###}$, {Percent * 100}%, {PeriodInMonth} месяцев, переплата: {CalcOverPayment():##,###}$";
        }
    }
}
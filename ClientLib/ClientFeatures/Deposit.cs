namespace Homework_17.ClientFeatures
{
    public class Deposit
    {
        public Deposit(double sum, double percent, int periodInMonth, bool capitalization)
        {
            Sum = sum;
            Percent = percent;
            PeriodInMonth = periodInMonth;
            Сapitalization = capitalization;
        }

        public double Sum { get; set; }
        public double Percent { get; set; }
        public int PeriodInMonth { get; set; }
        public bool Сapitalization { get; set; }

        /// <summary>
        /// Вычисляет сумму денег, которая получится к концу срока действия вклада.
        /// </summary>
        /// <returns>Сумму денег к концу срока действия вклада.</returns>
        public double CalcTotalSum()
        {
            if (Сapitalization)
            {
                double tmpSum = Sum;
                for (int i = 0; i < PeriodInMonth; i++)
                {
                    tmpSum += tmpSum * (Percent / 12.0);
                }
                //MessageBox.Show($"На вашем счету за {PeriodInMonth} месяцев по ставке {DepositPercent * 100}% с капитализацией будет сумма " + tmpSum);

                return tmpSum;
            }
            //MessageBox.Show($"На вашем счету за {PeriodInMonth} месяцев по ставке {DepositPercent * 100}% будет сумма " + (Sum + Sum * (DepositPercent / 12.0) * PeriodInMonth));
            return Sum + Sum * (Percent / 12.0) * PeriodInMonth;
        }

        /// <summary>
        /// Подробное отображение объекта в UI.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string capitalizationText = Сapitalization ? "с капитализацией" : "без капитализации";
            return $"{Sum.ToString("##,###")}$, {Percent * 100}%, {PeriodInMonth} месяцев {capitalizationText}, ожидаемая сумма: {CalcTotalSum().ToString("##,###")}$";
        }
    }
}
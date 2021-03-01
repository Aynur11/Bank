using System;

namespace Homework_17.ClientFeatures
{
    /// <summary>
    /// Исключение возникает когда клиент пытается перевести отсутствующие деньги.
    /// </summary>
    public class ClientInsufficientFundsException : Exception
    {
        public double Sum;

        public ClientInsufficientFundsException(double sum, string msg) : base(msg)
        {
            Sum = sum;
        }
    }
}
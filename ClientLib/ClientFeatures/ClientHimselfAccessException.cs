using System;

namespace Homework_17.ClientFeatures
{
    /// <summary>
    /// Исключение при попытке обращение клиента самому себе.
    /// </summary>
    public class ClientHimselfAccessException : Exception
    {
        public ClientHimselfAccessException(string msg) : base(msg)
        {

        }
    }
}

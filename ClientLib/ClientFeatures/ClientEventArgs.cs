using System;

namespace Homework_17.ClientFeatures
{
    public class ClientEventArgs
    {
        public DateTime Time;
        public double Sum;
        public string Message;

        public ClientEventArgs(DateTime time, double sum, string message)
        {
            Time = time;
            Sum = sum;
            Message = message;
        }
    }
}

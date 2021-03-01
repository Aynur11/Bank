using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_17.DB
{
    public class DbConnectionEventArgs
    {
        public DateTime Time;
        public string Message;
        public DbConnectionEventArgs(DateTime time, string message)
        {
            Time = time;
            Message = message;
        }
    }
}

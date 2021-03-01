using System;

namespace Homework_17.Departments
{
    public class DepartmentEventArgs
    {
        public DateTime Time;
        public string Message;
        public string DepartmentName;

        public DepartmentEventArgs(DateTime time, string message, string departmentName)
        {
            Time = time;
            Message = message;
            DepartmentName = departmentName;
        }
    }
}

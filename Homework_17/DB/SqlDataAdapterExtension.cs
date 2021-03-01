using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Homework_17.DB
{
    public static class SqlDataAdapterExtension
    {
        /// <summary>
        /// Заполнение таблицы с обработкой исключения.
        /// </summary>
        /// <param name="sqlDataAdapter"></param>
        /// <param name="dataTable"></param>
        public static void SafelyFill(this SqlDataAdapter sqlDataAdapter, DataTable dataTable)
        {
            try
            {
                sqlDataAdapter.Fill(dataTable);
            }
            catch (SqlException e)
            {
                MessageBox.Show($"При заполнении таблицы произошла ошибка: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Обновление таблицы с обработкой исключения.
        /// </summary>
        /// <param name="sqlDataAdapter"></param>
        /// <param name="dataTable"></param>
        public static void SafelyUpdate(this SqlDataAdapter sqlDataAdapter, DataTable dataTable)
        {
            try
            {
                sqlDataAdapter.Update(dataTable);
            }
            catch (SqlException e)
            {
                MessageBox.Show($"При обновлении таблицы произошла ошибка: {e.Message}");
                throw;
            }
        }
    }
}

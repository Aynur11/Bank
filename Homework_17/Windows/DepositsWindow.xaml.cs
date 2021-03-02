using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Homework_17.DB;

namespace Homework_17.Windows
{
    /// <summary>
    /// Interaction logic for DepositsWindow.xaml
    /// </summary>
    public partial class DepositsWindow : Window
    {
        private DataRowView row;
        private readonly DbManager dbManager;

        public DepositsWindow(int clientId, string name)
        {
            name = name.Replace(" ", "");
            Title = $"All {name} deposits";
            InitializeComponent();
            dbManager = new DbManager();
            try
            {
                DepositsGrid.DataContext = dbManager.GetDepositsTable();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Не удалось обновить депозиты. {e.Message}");
                Close();
            }
            DataTable gridTable = (DataTable) DepositsGrid.DataContext;
            gridTable.DefaultView.RowFilter = $"ClientId = {clientId}";
        }
        
        private void DepositsGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (DepositsGrid.CurrentItem != null)
            {
                row = (DataRowView)DepositsGrid.SelectedItem;
                row.BeginEdit();
            }
        }

        private void DepositsGrid_OnCurrentCellChanged(object sender, EventArgs e)
        {
            if (row == null)
            {
                return;
            }
            row.EndEdit();
            dbManager.DepositsSqlDataAdapter.Update(dbManager.DepositsDataTable);
        }
    }
}

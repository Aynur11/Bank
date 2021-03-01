using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CreditsWindow : Window
    {
        private DataRowView row;
        private readonly DbManager dbManager;

        public CreditsWindow(int clientId, string name)
        {
            name = name.Replace(" ", "");
            Title = $"All {name} credits";
            InitializeComponent();
            dbManager = new DbManager();
            CreditsGrid.DataContext = dbManager.GetCreditsTable();
            DataTable gridTable = (DataTable)CreditsGrid.DataContext;
            gridTable.DefaultView.RowFilter = $"ClientId = {clientId}";
        }

        private void CreditsGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (CreditsGrid.CurrentItem != null)
            {
                row = (DataRowView)CreditsGrid.SelectedItem;
                row.BeginEdit();
            }
        }
        private void CreditsGrid_OnCurrentCellChanged(object sender, EventArgs e)
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

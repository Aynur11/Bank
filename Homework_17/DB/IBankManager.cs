using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Homework_17.DB
{
    public delegate void DbConnectionHandler(object sender, DbConnectionEventArgs e);

    public interface IBankManager
    {
        SqlDataAdapter ClientsSqlDataAdapter { get; set; }
        SqlDataAdapter DepositsSqlDataAdapter { get; set; }
        SqlDataAdapter CreditsSqlDataAdapter { get; set; }
        DataTable PhysDataTable { get; set; }
        DataTable LegalDataTable { get; set; }
        DataTable DepositsDataTable { get; set; }
        DataTable CreditsDataTable { get; set; }

        event DbConnectionHandler OnDbConnection;
        DataTable GetDepositsTable();
        DataTable GetCreditsTable();
        DataView ConnectPhysDataTable();
        DataView ConnectLegalDataTable();
        Dictionary<int, string> GetAllClientNamesWithId();
        void TransferMoney(int idFrom, int idTo, double sumToTransfer);
    }
}
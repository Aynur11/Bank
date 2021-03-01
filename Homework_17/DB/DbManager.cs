using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Homework_17.ClientFeatures;

namespace Homework_17.DB
{
    public class DbManager : IBankManager
    {
        public SqlDataAdapter ClientsSqlDataAdapter { get; set; }
        public SqlDataAdapter DepositsSqlDataAdapter { get; set; }
        public SqlDataAdapter CreditsSqlDataAdapter { get; set; }
        public DataTable PhysDataTable { get; set; }
        public DataTable LegalDataTable { get; set; }
        public DataTable DepositsDataTable { get; set; }
        public DataTable CreditsDataTable { get; set; }
        private readonly SqlConnection commonSqlConnection;
        public DbManager()
        {
            ClientsSqlDataAdapter = new SqlDataAdapter();
            DepositsSqlDataAdapter = new SqlDataAdapter();
            CreditsSqlDataAdapter = new SqlDataAdapter();
            PhysDataTable = new DataTable();
            LegalDataTable = new DataTable();
            DepositsDataTable = new DataTable();
            CreditsDataTable = new DataTable();
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = "Bank",
                IntegratedSecurity = true,
                Pooling = false
            };
            OnDbConnection?.Invoke(this, new DbConnectionEventArgs(DateTime.Now, $"Connection string: { connectionStringBuilder.ConnectionString}"));
            commonSqlConnection = new SqlConnection(connectionStringBuilder.ConnectionString);

        }
        //public delegate void DbConnectionHandler(object sender, DbConnectionEventArgs e);

        public event DbConnectionHandler OnDbConnection;

        /// <summary>
        /// Возвращает все депозиты клиента.
        /// </summary>
        /// <returns></returns>
        public DataTable GetDepositsTable()
        {
            string sql = @"SELECT
                           Clients.Id as 'ClientId',
                           Clients.Name,
                           Deposits.Id as 'DepositId',
                           Deposits.Capitalization,
                           Deposits.Rate,
                           Deposits.Period,
                           Deposits.Sum
                           FROM Clients, Deposits
                           WHERE Deposits.ClientId = Clients.Id";
            DepositsSqlDataAdapter.SelectCommand = new SqlCommand(sql, commonSqlConnection);

            // Insert.
            sql = @"INSERT INTO Deposits (ClientId, Sum, Rate, Period, Capitalization)
                           VALUES(@ClientId, @Sum, @Rate, @Period, @Capitalization)
                           SET @Id = @@IDENTITY";
            DepositsSqlDataAdapter.InsertCommand = new SqlCommand(sql, commonSqlConnection);
            DepositsSqlDataAdapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id").Direction = ParameterDirection.Output;
            DepositsSqlDataAdapter.InsertCommand.Parameters.Add("@ClientId", SqlDbType.Int, 4, "ClientId");
            DepositsSqlDataAdapter.InsertCommand.Parameters.Add("@Sum", SqlDbType.Float, 4, "Sum");
            DepositsSqlDataAdapter.InsertCommand.Parameters.Add("@Rate", SqlDbType.Float, 4, "Rate");
            DepositsSqlDataAdapter.InsertCommand.Parameters.Add("@Period", SqlDbType.Int, 8, "Period");
            DepositsSqlDataAdapter.InsertCommand.Parameters.Add("@Capitalization", SqlDbType.Bit, 4, "Capitalization");

            // Update.
            sql = @"UPDATE Deposits SET
                    Sum = @Sum,
                    Rate = @Rate,
                    Period = @Period,
                    Capitalization = @Capitalization
                    WHERE Id=@DepositId";
            DepositsSqlDataAdapter.UpdateCommand = new SqlCommand(sql, commonSqlConnection);
            DepositsSqlDataAdapter.UpdateCommand.Parameters.Add("@DepositId", SqlDbType.Int, 4, "DepositId").SourceVersion =
                DataRowVersion.Original;
            DepositsSqlDataAdapter.UpdateCommand.Parameters.Add("@Sum", SqlDbType.Float, 8, "Sum");
            DepositsSqlDataAdapter.UpdateCommand.Parameters.Add("@Rate", SqlDbType.Float, 4, "Rate");
            DepositsSqlDataAdapter.UpdateCommand.Parameters.Add("@Period", SqlDbType.Int, 8, "Period");
            DepositsSqlDataAdapter.UpdateCommand.Parameters.Add("@Capitalization", SqlDbType.Bit, 4, "Capitalization");

            DepositsSqlDataAdapter.SafelyFill(DepositsDataTable);
            return DepositsDataTable;
        }

        /// <summary>
        /// Возвращает все кредиты клиента.
        /// </summary>
        /// <returns></returns>
        public DataTable GetCreditsTable()
        {
            string sql = @"SELECT
                           Clients.Id as 'ClientId',
                           Clients.Name,
                           Credits.Id as 'CreditId',
                           Credits.Rate,
                           Credits.Period,
                           Credits.Sum
                           FROM Clients, Credits
                           WHERE Credits.ClientId = Clients.Id";
            CreditsSqlDataAdapter.SelectCommand = new SqlCommand(sql, commonSqlConnection);

            // Insert.
            sql = @"INSERT INTO Credits (ClientId, Sum, Rate, Period)
                           VALUES(@ClientId, @Sum, @Rate, @Period)
                           SET @Id = @@IDENTITY";
            CreditsSqlDataAdapter.InsertCommand = new SqlCommand(sql, commonSqlConnection);
            CreditsSqlDataAdapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id").Direction = ParameterDirection.Output;
            CreditsSqlDataAdapter.InsertCommand.Parameters.Add("@ClientId", SqlDbType.Int, 4, "ClientId");
            CreditsSqlDataAdapter.InsertCommand.Parameters.Add("@Sum", SqlDbType.Float, 4, "Sum");
            CreditsSqlDataAdapter.InsertCommand.Parameters.Add("@Rate", SqlDbType.Float, 4, "Rate");
            CreditsSqlDataAdapter.InsertCommand.Parameters.Add("@Period", SqlDbType.Int, 8, "Period");

            // Update.
            sql = @"UPDATE Credits SET
                    Sum = @Sum,
                    Rate = @Rate,
                    Period = @Period,
                    Capitalization = @Capitalization
                    WHERE Id=@CreditId";
            CreditsSqlDataAdapter.UpdateCommand = new SqlCommand(sql, commonSqlConnection);
            CreditsSqlDataAdapter.UpdateCommand.Parameters.Add("@CreditId", SqlDbType.Int, 4, "CreditId").SourceVersion =
                DataRowVersion.Original;
            CreditsSqlDataAdapter.UpdateCommand.Parameters.Add("@Sum", SqlDbType.Float, 8, "Sum");
            CreditsSqlDataAdapter.UpdateCommand.Parameters.Add("@Rate", SqlDbType.Float, 4, "Rate");
            CreditsSqlDataAdapter.UpdateCommand.Parameters.Add("@Period", SqlDbType.Int, 8, "Period");

            CreditsSqlDataAdapter.SafelyFill(CreditsDataTable);
            return CreditsDataTable;
        }

        /// <summary>
        /// Устанавливает связь с таблицей клиентов с физ лицами.
        /// </summary>
        /// <returns></returns>
        public DataView ConnectPhysDataTable()
        {
            return Prepare(PhysicalPersonsSql, PhysDataTable);
        }

        /// <summary>
        /// Устанавливает связь с таблицей клиентов с юр лицами.
        /// </summary>
        /// <returns></returns>
        public DataView ConnectLegalDataTable()
        {   
            return Prepare(LegalPersonsSql, LegalDataTable);
        }

        /// <summary>
        /// Возвращает словарь клиента виде имени и его идентифтикатора.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetAllClientNamesWithId()
        {
            DataTable clientNamesDataTable = new DataTable();
            string sql = @"SELECT 
                           Clients.Id,
                           Clients.Name
                           FROM Clients";
            ClientsSqlDataAdapter.SelectCommand = new SqlCommand(sql, commonSqlConnection);
            ClientsSqlDataAdapter.SafelyFill(clientNamesDataTable);
            return clientNamesDataTable
                .AsEnumerable()
                .ToDictionary(row => row.Field<int>("Id"), row => row.Field<string>("Name"))
                .ToDictionary(x => x.Key, y => y.Value.Trim());
        }

        /// <summary>
        /// Выполняет перевод денег другому клиенту.
        /// </summary>
        /// <param name="idFrom"></param>
        /// <param name="idTo"></param>
        /// <param name="sumToTransfer"></param>
        public void TransferMoney(int idFrom, int idTo, double sumToTransfer)
        {
            double purposeClientSum = GetClientSum(idTo);
            double currentClientSum = GetClientSum(idFrom);
            if (sumToTransfer > currentClientSum)
            {
                MessageBox.Show("Недостаточно средств на счету!");
                return;
            }

            if (idFrom == idTo)
            {
                MessageBox.Show("Попытка перевести средства самому себе!");
                return;
            }

            UpdateClientSum(idTo, purposeClientSum + sumToTransfer);
            UpdateClientSum(idFrom, currentClientSum - sumToTransfer);
        }

        private DataView Prepare(string commonTableSql, DataTable table)
        {
            ClientsSqlDataAdapter.SelectCommand = new SqlCommand(commonTableSql, commonSqlConnection);

            // Insert.
            string sql = @"INSERT INTO Clients (Vip, PhysicalPersonsDepartment, Name, Sum, DepositPercent, CreditPercent)
                           VALUES(@Vip, @PhysicalPersonsDepartment, @Name, @Sum, @DepositPercent, @CreditPercent)
                           SET @Id = @@IDENTITY";
            ClientsSqlDataAdapter.InsertCommand = new SqlCommand(sql, commonSqlConnection);
            ClientsSqlDataAdapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id").Direction = ParameterDirection.Output;
            ClientsSqlDataAdapter.InsertCommand.Parameters.Add("@Vip", SqlDbType.Bit, 4, "Vip");
            ClientsSqlDataAdapter.InsertCommand.Parameters.Add("@PhysicalPersonsDepartment", SqlDbType.Bit, 4, "PhysicalPersonsDepartment");
            ClientsSqlDataAdapter.InsertCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 30, "Name");
            ClientsSqlDataAdapter.InsertCommand.Parameters.Add("@Sum", SqlDbType.Int, 8, "Sum");
            ClientsSqlDataAdapter.InsertCommand.Parameters.Add("@DepositPercent", SqlDbType.Float, 4, "DepositPercent");
            ClientsSqlDataAdapter.InsertCommand.Parameters.Add("@CreditPercent", SqlDbType.Float, 4, "CreditPercent");

            // Update.
            sql = @"UPDATE Clients SET
                    Name = @Name,
                    Sum = @Sum
                    WHERE Id=@Id";
            ClientsSqlDataAdapter.UpdateCommand = new SqlCommand(sql, commonSqlConnection);
            ClientsSqlDataAdapter.UpdateCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id").SourceVersion =
                DataRowVersion.Original;
            ClientsSqlDataAdapter.UpdateCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 30, "Name");
            ClientsSqlDataAdapter.UpdateCommand.Parameters.Add("@Sum", SqlDbType.Int, 8, "Sum");

            // Delete.
            sql = "DELETE FROM Clients WHERE Id = @Id";
            ClientsSqlDataAdapter.DeleteCommand = new SqlCommand(sql, commonSqlConnection);
            ClientsSqlDataAdapter.DeleteCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id");

            ClientsSqlDataAdapter.SafelyFill(table);
            return table.DefaultView;
        }

        private void UpdateClientSum(int id, double sum)
        {
            foreach (DataRow row in PhysDataTable.Rows)
            {
                if (Int32.Parse(row["Id"].ToString()) == id)
                {
                    row["Sum"] = sum;
                    ClientsSqlDataAdapter.SafelyUpdate(PhysDataTable);
                    return;
                }
            }

            foreach (DataRow row in LegalDataTable.Rows)
            {
                if (Int32.Parse(row["Id"].ToString()) == id)
                {
                    row["Sum"] = sum;
                    ClientsSqlDataAdapter.SafelyUpdate(LegalDataTable);
                    return;
                }
            }
        }

        private double GetClientSum(int clientId)
        {
            DataTable clientSumDataTable = new DataTable();
            string sql = $@"SELECT
                            Clients.Sum
                            FROM Clients
                            WHERE Clients.Id = {clientId}";
            ClientsSqlDataAdapter.SelectCommand = new SqlCommand(sql, commonSqlConnection);
            ClientsSqlDataAdapter.SafelyFill(clientSumDataTable);
            return clientSumDataTable.Rows[0].Field<double>("Sum");
        }

        /// <summary>
        /// SQL основной таблицы для физ лиц.
        /// </summary>
        private static string PhysicalPersonsSql
        {
            get
            {
                string physicalPersonsSql = @"SELECT
                                              Clients.Id,
                                              Clients.Vip,
                                              Clients.PhysicalPersonsDepartment,
                                              Clients.Name,
                                              Clients.Sum,
                                              Clients.DepositPercent,
                                              Clients.CreditPercent
                                              FROM Clients
                                              WHERE Clients.PhysicalPersonsDepartment='True'";
                return physicalPersonsSql;
            }
        }

        /// <summary>
        /// SQL основной таблицы для юр лиц.
        /// </summary>
        private static string LegalPersonsSql
        {
            get
            {
                string legalPersonsSql = @"SELECT
                                           Clients.Id,
                                           Clients.Vip,
                                           Clients.PhysicalPersonsDepartment,
                                           Clients.Name,
                                           Clients.Sum,
                                           Clients.DepositPercent,
                                           Clients.CreditPercent
                                           FROM Clients
                                           WHERE Clients.PhysicalPersonsDepartment='False'";
                return legalPersonsSql;
            }
        }
        private void PrepareTables(SqlConnection sqlConnection)
        {
            string createClientSql = @"CREATE TABLE [dbo].[Clients] (
                                        [Id]                        INT        IDENTITY (1, 1) NOT NULL,
                                        [Vip]                       BIT        NOT NULL,
                                        [PhysicalPersonsDepartment] BIT        NOT NULL,
                                        [Name]                      NCHAR (50) NOT NULL,
                                        [Sum]                       FLOAT      NOT NULL,
                                        [DepositPercent]            FLOAT (53) NOT NULL,
                                        [CreditPercent]             FLOAT (53) NOT NULL
                                    );";
            string[] insertClientSqls =
            {
                "INSERT INTO Clients ([Vip], [PhysicalPersonsDepartment], [Name], [Sum], [DepositPercent], [CreditPercent]) values ('false', 'true', 'Alex', 4000, 0.31, 0.15)",
                "INSERT INTO Clients ([Vip], [PhysicalPersonsDepartment], [Name], [Sum], [DepositPercent], [CreditPercent]) values ('true', 'false', 'Vitaliy', 8000, 0.1, 0.5)",
                "INSERT INTO Clients ([Vip], [PhysicalPersonsDepartment], [Name], [Sum], [DepositPercent], [CreditPercent]) values ('false', 'false', 'Gregory', 940, 0.11, 0.45)"
            };
            string createCreditSql = @"CREATE TABLE [dbo].[Credits] (
                                          [Id]       INT        IDENTITY (1, 1) NOT NULL,
                                          [ClientId] INT        NOT NULL,
                                          [Sum]      FLOAT (53) NOT NULL,
                                          [Rate]     FLOAT (53) NOT NULL,
                                          [Period]   FLOAT (53) NOT NULL
                                      );";
            string[] insertCreditSqls =
            {
                "INSERT INTO Credits ([ClientId], [Sum], [Rate], [Period]) values ('1', '300', 0.51, 0.12)",
                "INSERT INTO Credits ([ClientId], [Sum], [Rate], [Period]) values ('2', '600', 0.38, 0.5)",
                "INSERT INTO Credits ([ClientId], [Sum], [Rate], [Period]) values ('3', '7700', 0.11, 0.75)",
                "INSERT INTO Credits ([ClientId], [Sum], [Rate], [Period]) values ('4', '900', 0.44, 0.22)"
            };
            string createDepositSql = @"CREATE TABLE [dbo].[Deposits] (
                                          [Id]       INT        IDENTITY (1, 1) NOT NULL,
                                          [ClientId] INT        NOT NULL,
                                          [Sum]      FLOAT (53) NOT NULL,
                                          [Rate]     FLOAT (53) NOT NULL,
                                          [Period]   FLOAT (53) NOT NULL,
                                          [Capitalization]   BIT NOT NULL
                                      );";
            string[] insertDepositSqls =
            {
                "INSERT INTO Deposits ([ClientId], [Sum], [Rate], [Period], [Capitalization]) values ('1', '600', 0.31, 0.19, 'true')",
                "INSERT INTO Deposits ([ClientId], [Sum], [Rate], [Period], [Capitalization]) values ('2', '700', 0.35, 0.25, 'false')",
                "INSERT INTO Deposits ([ClientId], [Sum], [Rate], [Period], [Capitalization]) values ('3', '1100', 0.54, 0.58, 'false')",
                "INSERT INTO Deposits ([ClientId], [Sum], [Rate], [Period], [Capitalization]) values ('4', '43500', 0.18, 0.05, 'true')"
            };
            ExecuteSqlNonQuery(sqlConnection, new[] { $"{createClientSql}" });
            ExecuteSqlNonQuery(sqlConnection, insertClientSqls);
            ExecuteSqlNonQuery(sqlConnection, new[] { $"{createCreditSql}" });
            ExecuteSqlNonQuery(sqlConnection, insertCreditSqls);
            ExecuteSqlNonQuery(sqlConnection, new[] { $"{createDepositSql}" });
            ExecuteSqlNonQuery(sqlConnection, insertDepositSqls);
        }

        private void ExecuteSqlNonQuery(SqlConnection sqlConnection, string[] sqls)
        {
            Connect(sqlConnection);
            foreach (string insertSql in sqls)
            {
                var sqlCommand = new SqlCommand(insertSql, sqlConnection);
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    MessageBox.Show($"Произошла ошибка при выполнении запроса: {e.Message}");
                    throw;
                }
            }
            sqlConnection.Close();
        }

        private void Connect(SqlConnection sqlConnection)
        {
            try
            {
                sqlConnection.Open();
            }
            catch (Exception e)
            {
                OnDbConnection?.Invoke(this, new DbConnectionEventArgs(DateTime.Now, e.Message));
                OnDbConnection?.Invoke(this, new DbConnectionEventArgs(DateTime.Now, sqlConnection.State.ToString()));
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using ClientLib.ClientFeatures;
using Homework_17.ClientFeatures;
using Homework_17.DB;
using Homework_17.Departments;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Homework_17.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BankProvider BankProv;
        private readonly DbManager dbManager;
        private DataRowView row;
        public MainWindow()
        {
            InitializeComponent();
            Title = "My bank";
            BankProv = new BankProvider();
            DataContext = BankProv;
            LogsTextBlock.Text = string.Empty;
            ClientBase.OnOperation += (o, e) =>
            {
                LogsTextBlock.Text += $"{e.Time.ToShortTimeString()} {e.Message} {e.Sum}$ {Environment.NewLine}";
            };
            ClientBase.OnOperation += (o, e) =>
                {
                    MessageBox.Show($"{e.Time.ToShortTimeString()} {e.Message} {e.Sum}$ {Environment.NewLine}");
                };
            Department.OnOperation += (o, e) =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    LogsTextBlock.Text += $"{e.Time.ToShortTimeString()} [{e.DepartmentName}] {e.Message} {Environment.NewLine}";

                }));
            };

            dbManager = new DbManager();
            dbManager.OnDbConnection += (o, e) =>
            {
                LogsTextBlock.Text += $"{e.Time.ToShortTimeString()} {e.Message} {Environment.NewLine}";
            };

            PhysicalPersonsDataGrid.DataContext = dbManager.ConnectPhysDataTable();
            LegalPersonsDataGrid.DataContext = dbManager.ConnectLegalDataTable();
        }

        /// <summary>
        /// Открывает вклад.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDepositButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //ClientBase client;

            //if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.SelectedItem is ClientBase)
            //{
            //    client = PhysicalPersonsDataGrid.SelectedItem as ClientBase;

            //}
            //else if (LegalTabItem.IsSelected && LegalPersonsDataGrid.SelectedItem is ClientBase)
            //{
            //    client = LegalPersonsDataGrid.SelectedItem as ClientBase;
            //}
            //else
            //{
            //    MessageBox.Show("Выберите нужного клиента!");
            //    return;
            //}

            //OpenDepositOrCreditWindow openDepositOrCreditWindow = new OpenDepositOrCreditWindow();
            //if (openDepositOrCreditWindow.ShowDialog() == true)
            //{
            //    client.OpenDeposit(openDepositOrCreditWindow.Sum, openDepositOrCreditWindow.Period, openDepositOrCreditWindow.CapitalizationCheckBox.IsEnabled);
            //}


            if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.CurrentItem != null)
            {
                row = (DataRowView)PhysicalPersonsDataGrid.SelectedItem;
            }
            else if (LegalTabItem.IsSelected && LegalPersonsDataGrid.CurrentItem != null)
            {
                row = (DataRowView)LegalPersonsDataGrid.SelectedItem;
            }
            else
            {
                MessageBox.Show("Выберите нужного клиента!");
                return;
            }
            OpenDepositOrCreditWindow openDepositOrCreditWindow = new OpenDepositOrCreditWindow();
            if (openDepositOrCreditWindow.ShowDialog() == true)
            {
                try
                {
                    if (Int32.Parse(row["Sum"].ToString()) < openDepositOrCreditWindow.Sum)
                    {
                        throw new ClientInsufficientFundsException(openDepositOrCreditWindow.Sum,
                            "Недостаточно средств для открытия депозита.");
                    }
                }
                catch (ClientInsufficientFundsException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                // Костыль для инициализации таблицы.
                dbManager.GetDepositsTable();

                DataRow dataRow = dbManager.DepositsDataTable.NewRow();
                dataRow["ClientId"] = Int32.Parse(row["Id"].ToString());
                dataRow["Sum"] = openDepositOrCreditWindow.Sum;
                dataRow["Rate"] = bool.Parse(row["Vip"].ToString()) ? 0.3 : 0.2;
                dataRow["Period"] = openDepositOrCreditWindow.Period;
                dataRow["Capitalization"] = openDepositOrCreditWindow.CapitalizationCheckBox.IsEnabled;
                dbManager.DepositsDataTable.Rows.Add(dataRow);
                dbManager.DepositsSqlDataAdapter.Update(dbManager.DepositsDataTable);
                row["Sum"] = Int32.Parse(row["Sum"].ToString()) - openDepositOrCreditWindow.Sum;
                dbManager.ClientsSqlDataAdapter.Update(dbManager.PhysDataTable);
            }
        }

        /// <summary>
        /// Выдает кредит.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssueCreditButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //ClientBase client;

            //if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.SelectedItem is ClientBase)
            //{
            //    client = PhysicalPersonsDataGrid.SelectedItem as ClientBase;

            //}
            //else if (LegalTabItem.IsSelected && LegalPersonsDataGrid.SelectedItem is ClientBase)
            //{
            //    client = LegalPersonsDataGrid.SelectedItem as ClientBase;
            //}
            //else
            //{
            //    MessageBox.Show("Выберите нужного клиента!");
            //    return;
            //}

            //OpenDepositOrCreditWindow openDepositOrCreditWindow = new OpenDepositOrCreditWindow();
            //openDepositOrCreditWindow.Title = "Данные для выдачи кредита";
            //openDepositOrCreditWindow.CapitalizationCheckBox.IsEnabled = false;
            //if (openDepositOrCreditWindow.ShowDialog() == true)
            //{
            //    client.IssueCredit(openDepositOrCreditWindow.Sum, openDepositOrCreditWindow.Period);
            //    client.OnClientInstanceOperation += (o, args) =>
            //    {
            //        if (!client.Vip && client.Credits.Count > 1)
            //        {
            //            client.Vip = true;
            //            client.CreditPercent -= 0.1;
            //            client.DepositPercent += 0.3;
            //            LogsTextBlock.Text += $"Клиент {client.Name} переведен на статус VIP" + Environment.NewLine;
            //        }
            //    };
            //}

            if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.CurrentItem != null)
            {
                row = (DataRowView)PhysicalPersonsDataGrid.SelectedItem;
            }
            else if (LegalTabItem.IsSelected && LegalPersonsDataGrid.CurrentItem != null)
            {
                row = (DataRowView)LegalPersonsDataGrid.SelectedItem;
            }
            else
            {
                MessageBox.Show("Выберите нужного клиента!");
                return;
            }

            OpenDepositOrCreditWindow openDepositOrCreditWindow = new OpenDepositOrCreditWindow();
            openDepositOrCreditWindow.CapitalizationCheckBox.IsEnabled = false;
            if (openDepositOrCreditWindow.ShowDialog() == true)
            {
                dbManager.IssueCredit(Int32.Parse(row["Id"].ToString()),
                    openDepositOrCreditWindow.Sum,
                    openDepositOrCreditWindow.Period);
                //client.OpenDeposit(openDepositOrCreditWindow.Sum, openDepositOrCreditWindow.Period, openDepositOrCreditWindow.CapitalizationCheckBox.IsEnabled);
            }
        }

        /// <summary>
        /// Сохраняет стуктуру компании.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCompanyStructButton_OnClick(object sender, RoutedEventArgs e)
        {
            string outputFileName = "SavedCompanyStruct.json";
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
            serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
            serializer.Formatting = Newtonsoft.Json.Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(outputFileName))
            using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                serializer.Serialize(writer, BankProv, typeof(BankProvider));
            }

            MessageBox.Show($"Структура компании успешно сохранена в файл {outputFileName}");
        }

        /// <summary>
        /// Генерирует большое количество данных.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateDataButton_OnClick(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            ObservableCollection<Client> newClients = new ObservableCollection<Client>();
            int n = 10000;
            ProgressManager progressManager = new ProgressManager();
            progressManager.BeginWaiting();
            progressManager.ChangeStatus("В отдельном потоке выполняем генерацию записей...");
            progressManager.SetProgressMaxValue(n);

            for (int i = 0; i < n; i++)
            {
                ClientBase client = Convert.ToBoolean(random.Next(1, 10) % 2 == 0) ?
                    (ClientBase)new Client(Guid.NewGuid().ToString()) :
                    new VipClient(Guid.NewGuid().ToString());


                newClients.Add(new Client(Guid.NewGuid().ToString()));
                BankProv.BankObj.AddNewClient(client, random.Next(1, 10) % 2 != 0);
                progressManager.ChangeProgress(i);
            }
            progressManager.EndWaiting();
        }

        /// <summary>
        /// Загружает структуру компании.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadCompanyStructButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "JSON Files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                BankProv += JsonConvert.DeserializeObject<BankProvider>(json, new Newtonsoft.Json.JsonSerializerSettings
                {
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                });
            }
            PhysicalPersonsDataGrid.Items.Refresh();
        }

        /// <summary>
        /// Класс - параметр для запуска потока.
        /// </summary>
        public class Data
        {
            public Data(string file)
            {
                File = file;
            }
            public string File { get; set; }
        }

        /// <summary>
        /// Загружает данные с рабочего каталога.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadDataFromFiles_OnClick(object sender, RoutedEventArgs e)
        {
            object lockObj = new object();
            BindingOperations.EnableCollectionSynchronization(BankProv.BankObj.LegalPersonsDepartment.Clients, lockObj);
            BindingOperations.EnableCollectionSynchronization(BankProv.BankObj.PhysicalPersonsDepartment.Clients, lockObj);
            Action<object> action = o =>
            {
                Data data = o as Data;
                string json = File.ReadAllText(data.File);
                var bankProvider = JsonConvert.DeserializeObject<BankProvider>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    NullValueHandling = NullValueHandling.Ignore
                });
                lock (lockObj)
                {
                    BankProv += bankProvider;
                }
            };

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.clients");

            // Готовим потоки.
            int threadsCount = Math.Min(files.Length, Environment.ProcessorCount - 1);
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < threadsCount; i++)
            {
                var parameterizedThreadStart = new ParameterizedThreadStart(action);
                threads.Add(new Thread(parameterizedThreadStart));
            }

            ProgressManager progressManager = new ProgressManager();
            progressManager.BeginWaiting();
            progressManager.SetProgressMaxValue(threads.Count);
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Start(new Data(files[i]));
            }
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Join();
                progressManager.ChangeProgress(i + 1);
            }
            Thread.Sleep(100);
            progressManager.EndWaiting();
        }

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowAllDepositsMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            DepositsWindow depositsWindow;
            if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.CurrentItem != null)
            {
                Int32.TryParse((PhysicalPersonsDataGrid.CurrentItem as DataRowView)?.Row[0].ToString(), out int clientId);
                depositsWindow = new DepositsWindow(clientId, (PhysicalPersonsDataGrid.CurrentItem as DataRowView)?.Row[3].ToString());
                depositsWindow.ShowDialog();
            }
            else if (LegalTabItem.IsSelected && LegalPersonsDataGrid.CurrentItem != null)
            {
                Int32.TryParse((LegalPersonsDataGrid.CurrentItem as DataRowView)?.Row[0].ToString(), out int clientId);
                depositsWindow = new DepositsWindow(clientId, (LegalPersonsDataGrid.CurrentItem as DataRowView)?.Row[3].ToString());
                depositsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите нужного клиента!");
            }
        }

        private void ShowAllCreditsMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            CreditsWindow creditsWindow;
            if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.CurrentItem != null)
            {
                Int32.TryParse((PhysicalPersonsDataGrid.CurrentItem as DataRowView)?.Row[0].ToString(), out int clientId);
                creditsWindow = new CreditsWindow(clientId, (PhysicalPersonsDataGrid.CurrentItem as DataRowView)?.Row[3].ToString());
                creditsWindow.ShowDialog();
            }
            else if (LegalTabItem.IsSelected && LegalPersonsDataGrid.CurrentItem != null)
            {
                Int32.TryParse((LegalPersonsDataGrid.CurrentItem as DataRowView)?.Row[0].ToString(), out int clientId);
                creditsWindow = new CreditsWindow(clientId, (LegalPersonsDataGrid.CurrentItem as DataRowView)?.Row[3].ToString());
                creditsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите нужного клиента!");
            }
        }

        /// <summary>
        /// Добавляет нового клиента.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewClientButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ClientNameWindow clientNameWindow = new ClientNameWindow();
            clientNameWindow.IsVipClientCheckBox.IsEnabled = true;
            clientNameWindow.IsLegalClientCheckBox.IsEnabled = true;
            clientNameWindow.ClientNamesComboBox.IsEnabled = false;
            if (clientNameWindow.ShowDialog() == true)
            {
                DataRow dataRow = Convert.ToBoolean(clientNameWindow.IsLegalClientCheckBox.IsChecked) ?
                    dbManager.LegalDataTable.NewRow() :
                    dbManager.PhysDataTable.NewRow();
                dataRow["Name"] = clientNameWindow.NameTextBox.Text;
                dataRow["Sum"] = clientNameWindow.Sum;
                dataRow["Vip"] = Convert.ToBoolean(clientNameWindow.IsVipClientCheckBox.IsChecked);
                dataRow["PhysicalPersonsDepartment"] = !Convert.ToBoolean(clientNameWindow.IsLegalClientCheckBox.IsChecked);
                if (Convert.ToBoolean(clientNameWindow.IsLegalClientCheckBox.IsChecked))
                {
                    dbManager.LegalDataTable.Rows.Add(dataRow);
                    dbManager.ClientsSqlDataAdapter.Update(dbManager.LegalDataTable);
                }
                else
                {
                    dbManager.PhysDataTable.Rows.Add(dataRow);
                    dbManager.ClientsSqlDataAdapter.Update(dbManager.PhysDataTable);
                }
            }

            //ClientNameWindow clientNameWindow = new ClientNameWindow();
            //clientNameWindow.IsVipClientCheckBox.IsEnabled = true;
            //clientNameWindow.IsLegalClientCheckBox.IsEnabled = true;
            //clientNameWindow.ClientNamesComboBox.IsEnabled = false;
            //if (clientNameWindow.ShowDialog() == true)
            //{
            //    ClientBase client = Convert.ToBoolean(clientNameWindow.IsVipClientCheckBox.IsChecked) ?
            //        (ClientBase)new VipClient(clientNameWindow.NameTextBox.Text) :
            //        new Client(clientNameWindow.NameTextBox.Text);
            //    client.Sum = clientNameWindow.Sum;

            //    BankProv.BankObj.AddNewClient(client, !Convert.ToBoolean(clientNameWindow.IsLegalClientCheckBox.IsChecked));
            //}

        }

        /// <summary>
        /// Определение завершения ввода и получение введенного значения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhysicalPersonsDataGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.CurrentItem != null)
            {
                row = (DataRowView)PhysicalPersonsDataGrid.SelectedItem;
                row.BeginEdit();
            }
        }

        /// <summary>
        /// Обновление БД в соответствии с изменениями в таблице.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhysicalPersonsDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
        {
            if (row == null)
            {
                return;
            }
            row.EndEdit();
            dbManager.ClientsSqlDataAdapter.Update(dbManager.PhysDataTable);
        }

        private void LegalPersonsDataGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (LegalTabItem.IsSelected && LegalPersonsDataGrid.CurrentItem != null)
            {
                row = (DataRowView)LegalPersonsDataGrid.SelectedItem;
                row.BeginEdit();
            }
        }

        private void LegalPersonsDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
        {
            if (row == null)
            {
                return;
            }
            row.EndEdit();
            dbManager.ClientsSqlDataAdapter.Update(dbManager.LegalDataTable);
        }

        /// <summary>
        /// Обработчик события удаления клиента.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveClientButton_OnClick(object sender, RoutedEventArgs e)
        {
            //ClientBase client;

            //if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.SelectedItem is ClientBase)
            //{
            //    client = PhysicalPersonsDataGrid.SelectedItem as ClientBase;

            //}
            //else if (LegalTabItem.IsSelected && LegalPersonsDataGrid.SelectedItem is ClientBase)
            //{
            //    client = LegalPersonsDataGrid.SelectedItem as ClientBase;
            //}
            //else
            //{
            //    MessageBox.Show("Выберите нужного клиента!");
            //    return;
            //}

            //if (BankProv.BankObj.RemoveClient(client))
            //{
            //    MessageBox.Show("Клиент удален.");
            //}
            //else
            //{
            //    MessageBox.Show("Клиент не найден.");
            //}
            if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.CurrentItem != null)
            {
                row = (DataRowView)PhysicalPersonsDataGrid.SelectedItem;
                row.Delete();
                dbManager.ClientsSqlDataAdapter.Update(dbManager.PhysDataTable);
            }
            else if (LegalTabItem.IsSelected && LegalPersonsDataGrid.CurrentItem != null)
            {
                row = (DataRowView)LegalPersonsDataGrid.SelectedItem;
                row.Delete();
                dbManager.ClientsSqlDataAdapter.Update(dbManager.LegalDataTable);
            }

        }

        /// <summary>
        /// Перевод денег выбранного клиента другому клиенту.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransferMoneyButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //ClientBase client;

            //if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.SelectedItem is ClientBase)
            //{
            //    client = PhysicalPersonsDataGrid.SelectedItem as ClientBase;

            //}
            //else if (LegalTabItem.IsSelected && LegalPersonsDataGrid.SelectedItem is ClientBase)
            //{
            //    client = LegalPersonsDataGrid.SelectedItem as ClientBase;
            //}
            //else
            //{
            //    MessageBox.Show("Выберите нужного клиента!");
            //    return;
            //}

            //ClientNameWindow clientNameWindow = new ClientNameWindow();
            //clientNameWindow.ClientNamesComboBox.ItemsSource = BankProv.BankObj.GetAllClientsNamesWithId();
            //clientNameWindow.NameTextBox.IsEnabled = false;
            //if (clientNameWindow.ShowDialog() == true)
            //{
            //    //client?.TransferMoney(BankProv.BankObj.FindClient(clientNameWindow.ClientNamesComboBox.Text), clientNameWindow.Sum);
            //    client?.TransferMoney(BankProv.BankObj.FindClientById(Convert.ToInt32(clientNameWindow.ClientNamesComboBox.Text.Split(':')[1])), clientNameWindow.Sum);
            //}

            if (PhysicalTabItem.IsSelected && PhysicalPersonsDataGrid.CurrentItem != null)
            {
                row = (DataRowView)PhysicalPersonsDataGrid.SelectedItem;

            }
            else if (LegalTabItem.IsSelected && LegalPersonsDataGrid.CurrentItem != null)
            {
                row = (DataRowView)LegalPersonsDataGrid.SelectedItem;
            }
            else
            {
                MessageBox.Show("Выберите нужного клиента!");
                return;
            }

            ClientNameWindow clientNameWindow = new ClientNameWindow
            {
                ClientNamesComboBox = { ItemsSource = dbManager.GetAllClientNamesWithId() },
                NameTextBox = { IsEnabled = false }
            };
            if (clientNameWindow.ShowDialog() == true)
            {
                Int32.TryParse(row[0].ToString(), out int currentClientId);
                int purposeClientId = ((KeyValuePair<int, string>)clientNameWindow.ClientNamesComboBox.SelectedValue).Key;
                dbManager.TransferMoney(currentClientId, purposeClientId, clientNameWindow.Sum);
            }
        }
    }
}
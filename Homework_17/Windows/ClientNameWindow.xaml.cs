using System;
using System.Windows;
using Homework_17.Windows;

namespace Homework_17
{
    /// <summary>
    /// Interaction logic for ClientNameWindow.xaml
    /// </summary>
    public partial class ClientNameWindow : Window
    {
        public ClientNameWindow()
        {
            InitializeComponent();
            Title = "Данные клиента";
        }

        /// <summary>
        /// Сумма денег не счету клиента.
        /// </summary>
        public double Sum => OpenDepositOrCreditWindow.IsDouble(SumTextBox.Text) ? Convert.ToDouble(SumTextBox.Text) : 0;

        /// <summary>
        /// Обработчик нажатия на ОК.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Sum == 0 || NameTextBox.Text == string.Empty)
            {
                MessageBox.Show("Введите корректные данные!");
                return;
            }
            this.DialogResult = true;
        }

        /// <summary>
        /// Обработчик нажатия на отмену.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Очистка текст-бокса.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Очистка текст-бокса.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SumTextBox_OnGotFocusTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            SumTextBox.Text = String.Empty;
        }
    }
}

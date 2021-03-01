using System;
using System.Windows;

namespace Homework_17.Windows
{
    /// <summary>
    /// Interaction logic for OpenDepositOrCreditWindow.xaml
    /// </summary>
    public partial class OpenDepositOrCreditWindow : Window
    {
        public OpenDepositOrCreditWindow()
        {
            InitializeComponent();
            Title = "Данные для открытия/выдачи вклада/кредита";
        }

        /// <summary>
        /// Сумма денег с проверкой коррекности ввода.
        /// </summary>
        public double Sum => IsDouble(SumTextBox.Text) ? Convert.ToDouble(SumTextBox.Text) : 0;

        /// <summary>
        /// Период ведения счета с проверкой корректности ввода.
        /// </summary>
        public int Period => IsDouble(PeriodTextBox.Text) ? Convert.ToInt32(PeriodTextBox.Text) : 0;


        /// <summary>
        /// Предварительная проверка что значение является типом Double.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Булевыйй результат.</returns>
        public static bool IsDouble(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            return double.TryParse(text, out _);
        }

        /// <summary>
        /// Очистка текст-бокса.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SumTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            SumTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Очистка текст-бокса.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PeriodTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            PeriodTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Обработчик нажатия на ОК.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Sum == 0 || Period == 0)
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
    }
}

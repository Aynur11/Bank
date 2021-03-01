using System;
using System.Threading;
using System.Windows.Threading;
using Homework_17.Windows;

namespace Homework_17
{
    public class ProgressManager
    {
        private Thread thread;
        private bool canAbortThread = false;
        private ProgressWindow progressWindow;

        /// <summary>
        /// Запускает окно ожидания.
        /// </summary>
        public void BeginWaiting()
        {
            thread = new Thread(RunThread)
            {
                IsBackground = true
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            Thread.Sleep(100);
        }

        /// <summary>
        /// Закрытие окна загрузки.
        /// </summary>
        public void EndWaiting()
        {
            progressWindow?.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                progressWindow.Close();
            }));
            //while (!canAbortThread) { };
            //thread.Abort();
        }

        /// <summary>
        /// Обновление значения загрузки.
        /// </summary>
        /// <param name="value"></param>
        public void ChangeProgress(double value)
        {
            progressWindow?.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                progressWindow.ProgressBar.Value = value;
            }));
        }

        public void ChangeStatus(string text)
        {
            progressWindow?.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    progressWindow.TextBlock.Text = text;
                }));
        }

        /// <summary>
        /// Установка диапазона прогресс бара.
        /// </summary>
        /// <param name="maxValue">Максимальное значение для контрола.</param>
        public void SetProgressMaxValue(double maxValue)
        {
            Thread.Sleep(100);
            progressWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    progressWindow.ProgressBar.Minimum = 0;
                    progressWindow.ProgressBar.Maximum = maxValue;

                }));
        }

        private void RunThread()
        {
            progressWindow = new ProgressWindow();
            progressWindow.Closed += ProgressWindow_Closed;
            progressWindow.ShowDialog();
        }

        private void ProgressWindow_Closed(object sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.InvokeShutdown();
            canAbortThread = true;
        }
    }
}

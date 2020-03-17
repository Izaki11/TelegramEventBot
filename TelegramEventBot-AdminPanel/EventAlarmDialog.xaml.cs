using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TelegramEventBot_AdminPanel
{
    /// <summary>
    /// Interaction logic for EventAlarmDialog.xaml
    /// </summary>
    public partial class EventAlarmDialog : Window
    {
        public EventAlarmDialog()
        {
            InitializeComponent();
        }

        public DateTime ResponseDate
        {
            get { return AddAlarmTime(); }
            set { AlarmDateTextBox.Text = value.ToString(); }
        }

        public TimeSpan ResponseTime
        {
            private get
            {
                try
                {
                    return TimeSpan.Parse(AlarmTimeTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Invalid syntax", "Alarm Time", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new TimeSpan(0);
                }
            }

            set { AlarmTimeTextBox.Text = value.ToString(); }
        }

        private DateTime AddAlarmTime()
        {
            try
            {
                DateTime finalDateTime = DateTime.Parse(AlarmDateTextBox.Text);
                finalDateTime = finalDateTime.Add(ResponseTime);
                return finalDateTime;
            }
            catch
            {
                MessageBox.Show("Invalid syntax", "Alarm Date", MessageBoxButton.OK, MessageBoxImage.Error);
                return DateTime.Now;
            }
        }

        private void AlarmDateTextBox_CalendarOpened(object sender, RoutedEventArgs e)
        {
            AlarmDateTextBox.BlackoutDates.AddDatesInPast();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlarmDateTextBox.Text == "" || AlarmTimeTextBox.Text == "")
            {
                return;
            }
            else
            {
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

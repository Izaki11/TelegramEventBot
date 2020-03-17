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
    /// Interaction logic for EventDialog.xaml
    /// </summary>
    public partial class EventDialog : Window
    {
        public EventDialog()
        {
            InitializeComponent();
        }

        public string ResponseName
        {
            get { return EventNameTextBox.Text; }
            set { EventNameTextBox.Text = value; }
        }

        public DateTime ResponseDate
        {
            get { return AddEventTime(); }
            set { EventDateTextBox.Text = value.ToString(); }
        }

        public TimeSpan ResponseTime
        {
            get
            {
                try
                {
                    return TimeSpan.Parse(EventTimeTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Invalid syntax", "Event Time", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new TimeSpan(0);
                }
            }

            set { EventTimeTextBox.Text = value.ToString(); }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResponseName == "" || ResponseDate.ToString() == "" || ResponseTime.ToString() == "")
            {
                return;
            }
            else
            { 
                DialogResult = true;
            }
        }

        private DateTime AddEventTime()
        {
            try
            {
                DateTime finalDateTime = DateTime.Parse(EventDateTextBox.Text);
                finalDateTime = finalDateTime.Add(ResponseTime);
                return finalDateTime;
            }
            catch
            {
                MessageBox.Show("Invalid syntax", "Event Date", MessageBoxButton.OK, MessageBoxImage.Error);
                return DateTime.Now;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void EventDateTextBox_CalendarOpened(object sender, RoutedEventArgs e)
        {
            EventDateTextBox.BlackoutDates.AddDatesInPast();
        }
    }
}

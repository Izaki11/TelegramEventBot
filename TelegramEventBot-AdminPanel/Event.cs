using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace TelegramEventBot_AdminPanel
{
    class Event
    {
        public string eventName { get; set; }
        public DateTime eventDate { get; set; }
        public DateTime eventAlarmDate { get ; private set; }
        public bool hasAlarm { get; private set; }
        public long chatId { get; private set; }

        private DispatcherTimer timer;

        public delegate void AlarmEventHandler(object sender, EventArgs e);

        public event AlarmEventHandler AlarmActivated;

        public Event(string name, DateTime date, long id)
        {
            eventName = name;
            eventDate = date;
            chatId = id;
        }

        public void SetAlarm(DateTime date)
        {
            if (timer != null)
            {
                RemoveAlarm();
            }

            eventAlarmDate = date;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Tick += Timer_Tick;
            timer.IsEnabled = true;
            hasAlarm = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (eventAlarmDate.CompareTo(DateTime.Now) <= 0)
            {
                OnAlarmActivated();
            }
        }

        public void RemoveAlarm()
        {
            if (timer == null)
            {
                return;
            }
            timer.Stop();
            timer.IsEnabled = false;
            timer = null;
            hasAlarm = false;
        }

        protected virtual void OnAlarmActivated()
        {
            if (AlarmActivated != null)
            {
                AlarmActivated(this, EventArgs.Empty);
                RemoveAlarm();
            }
        }
    }
}

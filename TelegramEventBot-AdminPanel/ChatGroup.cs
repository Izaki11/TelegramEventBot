using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TelegramEventBot_AdminPanel
{
    class ChatGroup
    {
        public string chatName { get; set; }
        public long chatId { get; set; }

        public ObservableCollection<Event> eventList { get; private set; }

        public ChatGroup(string name, long id)
        {
            chatName = name;
            chatId = id;
            eventList = new ObservableCollection<Event>();
        }

        public void AddEvent(Event e)
        {
            eventList.Add(e);
        }

        public void RemoveEvent(Event e)
        {
            eventList.Remove(e);
        }
    }
}

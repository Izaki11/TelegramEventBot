using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramEventBot
{
    class Event
    {
        public DateTime eventDate;
        public string eventName;
        public DateTime alarmDate;
        public System.Timers.Timer eventAlarm;
        public long chatToMessage;
        public async void SetTimer(DateTime alarmDate, long groupToMessage)
        {
            if (alarmDate >= eventDate)
            {
                throw new ArgumentException("alarmDate cannot be past the event date");
            }
            else
            {
                this.alarmDate = alarmDate;
                TimeSpan finalDate = alarmDate - DateTime.Now;
                double finalDateConverted = finalDate.TotalMilliseconds;
                eventAlarm = new System.Timers.Timer(finalDateConverted);
                eventAlarm.Elapsed += OnAlarmEvent;
                eventAlarm.Enabled = true;
                chatToMessage = groupToMessage;
            }
        }
        private async void OnAlarmEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                Console.WriteLine("Reminder for the event '" + eventName + "' was just sent at: " + DateTime.Now);
                await BotMain.botClient.SendTextMessageAsync(chatId: chatToMessage, text: "This is a reminder for the event '" + eventName + "' at " + eventDate.ToString());
                eventAlarm.Stop();
                eventAlarm.Dispose();
            }
            catch (Exception)
            {
                Console.WriteLine("Error sending reminder text message for event: " + eventName + " to chatId: " + chatToMessage);
            }
        }
    }
    class BotMain
    {
        private static string botID;
        private static long privateId;
        public static ITelegramBotClient botClient;
        private static List<Event> eventList;

        public static void Init(string botId, long id)
        {
            botID = botId;
            botClient = new TelegramBotClient(botID);
            eventList = new List<Event>();
            privateId = id;
            var me = botClient.GetMeAsync().Result;
            Console.Clear();
            Console.WriteLine("Hello World! Bot user: " + me.Id + " with name: " + me.Username + " has initialized!");

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message.Date.ToLocalTime() + " - " + e.Message.From.FirstName + " @ " + e.Message.Chat.Type + " : " + e.Message.Text);

            if (e.Message.Text == "/pingid")
            {
                Console.WriteLine("Ping Id: " + e.Message.From.FirstName + " @ " + e.Message.Chat.Type + " Id = " + e.Message.Chat.Id);
            }

            if(privateId == e.Message.Chat.Id)
            {
                string[] args = e.Message.Text.Split(' ');

                switch (args[0])
                {
                    case "/newevent":

                        string eventText = ParseEventText(args);

                        if (eventText != null)
                        {
                            try
                            {
                                Event newEvent = new Event();
                                newEvent.eventName = args[1];
                                newEvent.eventDate = DateTime.Parse(eventText);
                                eventList.Add(newEvent);
                                Console.WriteLine("New event '" + newEvent.eventName + "' date set for: " + newEvent.eventDate.ToString());
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, disableNotification: true, text: "Creating new event '" + newEvent.eventName + "' set on date: " + newEvent.eventDate.ToString());
                            }
                            catch (Exception)
                            {
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, disableNotification: true, text: "Date parsing error - acceptable format: mm/dd/yyyy 0:00:00 AM");
                            }
                        }
                        else if (eventText == null)
                        {
                            await botClient.SendTextMessageAsync(chatId: e.Message.Chat, disableNotification: true, text: "Usage: '/newevent mm/dd/yyyy 0:00:00 AM'");
                        }
                        break;

                    case "/currentevents":

                        await botClient.SendTextMessageAsync(chatId: e.Message.Chat, disableNotification: true, text: ListAllEvents());
                        break;

                    case "/setreminder":

                        eventText = ParseEventText(args);

                        if (eventText != null)
                        {
                            try
                            {
                                string eventNameArg = args[1];
                                ListSearch(eventNameArg).SetTimer(DateTime.Parse(eventText), e.Message.Chat.Id);
                                Console.WriteLine("Set new alarm for: " + eventText);
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, disableNotification: true, text: "Setting alarm for event " + ListSearch(eventNameArg).eventName + " on: " + eventText);
                            }
                            catch (ArgumentException)
                            {
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, disableNotification: true, text: "Reminder date cannot be set past the event date");
                            }
                            catch (Exception)
                            {
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, disableNotification: true, text: "Reminder date parsing error - acceptable format : mm/dd/yyyy 0:00:00 AM");
                            }
                        }
                        break;

                    case "/deleteevent":

                        if (args.Length > 1 && args[1] != null)
                        {
                            Event eventToDelete = ListSearch(args[1]);

                            if (eventList.Contains(eventToDelete))
                            {
                                string eventToDeleteName = eventToDelete.eventName;
                                eventToDelete.eventAlarm.Stop();
                                eventToDelete.eventAlarm.Dispose();
                                eventList.Remove(eventToDelete);
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat.Id, disableNotification: true, text: "Event '" + eventToDeleteName + "' was deleted");
                            }
                            else if (!eventList.Contains(eventToDelete))
                            {
                                await botClient.SendTextMessageAsync(chatId: e.Message.Chat.Id, disableNotification: true, text: "Event '" + args[1] + "' was not found");
                            }
                        }
                        else if (args.Length <= 1)
                        {
                            await botClient.SendTextMessageAsync(chatId: e.Message.Chat.Id, disableNotification: true, text: "Usage: '/deleteevent EVENTNAME'");
                        }
                        break;

                    case "/help":

                        await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text:
                            "Available options: \n " +
                            "/newevent - Sets a new event date (Format: mm/dd/yyyy 0:00:00 AM) \n " +
                            "/currentevents - Displays the list of current events \n" +
                            "/setreminder - Sets a new date to remind the group of an event \n" +
                            "/deleteevent - Removes an event from the list \n" +
                            "/pingid - Displays the id of the chat in the host console \n" +
                            "/help - Lists all commands");
                        break;
                }
            }
        }

        //Rebuilds and returns the string, omitting the first word (command)
        private static string ParseEventText(string[] text)
        {
            string temp = null;

            for (int i = 2; i < text.Length; i++)
            {
                temp += text[i] + " ";
            }

            return temp;
        }

        private static string ListAllEvents()
        {
            string listOfEvents = "";

            if (eventList.Count == 0)
            {
                return "There are currently no events";
            }
            else
            {
                foreach (Event e in eventList)
                {
                    listOfEvents += "Event: " + e.eventName + " - Date: " + e.eventDate.ToString() + "\n";
                }

                return listOfEvents;
            }

        }

        private static Event ListSearch(string s)
        {
            return eventList.Find(x => x.eventName == s);
        }
    }
}

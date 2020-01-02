using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramEventBot
{
    class BotMain
    {
        private static readonly string botID = "Insert your botID here";
        private static ITelegramBotClient botClient;
        private static DateTime eventDate;
        private static long chatToMessage;
        private static System.Timers.Timer eventAlarm;

        public static void Init()
        {
            botClient = new TelegramBotClient(botID);
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine("Hello World! Bot user: " + me.Id + "with name: " + me.Username + "has initialized!");

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message.Date.ToLocalTime() + " - " + e.Message.From.FirstName + " @ " + e.Message.Chat.Type + " : " + e.Message.Text);
            string[] args = e.Message.Text.Split(' ');

            switch (args[0])
            {
                case "/newevent":

                    string eventText = ParseEventText(args);

                    if(eventText != null)
                    {
                        try
                        {
                            eventDate = DateTime.Parse(eventText);
                            Console.WriteLine("Set new event date for: " + eventDate.ToString());
                            await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: "Setting date for new event on: " + eventDate.ToString());
                        }
                        catch (Exception)
                        {
                            await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: "Date parsing error - acceptable format: mm/dd/yyyy 0:00:00 AM");
                        }
                    }
                    break;

                case "/currentevent":

                    await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: "Current event date is set for: " + eventDate.ToString());
                    break;

                case "/setreminder":

                    string reminderText = ParseEventText(args);

                    if (reminderText != null)
                    {
                        try
                        {
                            SetTimer(DateTime.Parse(reminderText));
                            chatToMessage = e.Message.Chat.Id;
                            Console.WriteLine("Set new alarm for: " + reminderText);
                            await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: "Setting alarm for event on: " + reminderText);
                        }
                        catch (Exception)
                        {
                            await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: "Reminder date parsing error - acceptable format : mm/dd/yyyy 0:00:00 AM");
                        }
                    }
                    break;

                case "/help":

                    await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: 
                        "Available options: \n " +
                        "/newevent - Sets a new event date (Format: mm/dd/yyyy 0:00:00 AM) \n " +
                        "/currentevent - Checks the currently set event date \n" +
                        "/setreminder - Sets a new date to remind the group of an event \n" +
                        "/help - Lists all commands");
                    break;
            }
        }

        //Rebuilds and returns the string, omitting the first word (command)
        private static string ParseEventText(string[] text)
        {
            string temp = null;

            for (int i = 1; i < text.Length; i++)
            {
                temp += text[i] + " ";
            }

            return temp;
        }

       private static async void SetTimer(DateTime alarmDate)
        {
            if (alarmDate >= eventDate)
            {
                return;
            }
            else
            {
                TimeSpan finalDate = alarmDate - DateTime.Now;
                double finalDateConverted = finalDate.TotalMilliseconds;
                eventAlarm = new System.Timers.Timer(finalDateConverted);
                eventAlarm.Elapsed += OnAlarmEvent;
                eventAlarm.Enabled = true;
            }
        }

        private static async void OnAlarmEvent(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Reminder was just sent at: " + DateTime.Now);
            await botClient.SendTextMessageAsync(chatId: chatToMessage, text: "This is a reminder for your event at " + eventDate.ToString());
            eventAlarm.Stop();
            eventAlarm.Dispose();
        }
    }
}

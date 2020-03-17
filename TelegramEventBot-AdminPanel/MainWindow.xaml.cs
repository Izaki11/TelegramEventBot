using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace TelegramEventBot_AdminPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ITelegramBotClient bot;
        public delegate void UpdateUIDelegate(string s);
        private bool autoScroll = true;
        private ObservableCollection<ChatGroup> chatGroupList = new ObservableCollection<ChatGroup>();

        public MainWindow()
        {
            InitializeComponent();
            ChatListBox.ItemsSource = chatGroupList;
            DisableControlPanel();
            ConsoleLogMessage("Welcome to TelegramEventBot! Enter a Telegram bot token to begin");
        }

        private void DisableControlPanel()
        {
            ControlPanelParent.IsEnabled = false;
            //ControlPanelParent.Background = new SolidColorBrush(Colors.LightGray);
            ShutdownButton.IsEnabled = false;
            InitButton.IsEnabled = true;
            ClearInfo();
        }

        private void EnableControlPanel()
        {
            ControlPanelParent.IsEnabled = true;
            //ControlPanelParent.Background = new SolidColorBrush(Colors.White);
            ShutdownButton.IsEnabled = true;
            InitButton.IsEnabled = false;
            DisableInfoPanel();
        }

        private void DisableInfoPanel()
        {
            InfoPanelParent.IsEnabled = false;
        }

        private void EnableInfoPanel()
        {
            InfoPanelParent.IsEnabled = true;
        }

        private void ClearInfo()
        {
            ChatNameTextBox.Clear();
            ChatIdTextBox.Clear();
        }

        private void ClearEventInfo()
        {
            EventNameTextBox.Clear();
            EventDateTextBox.Clear();
        }

        private void InitButton_Click(object sender, RoutedEventArgs e)
        {
            string botid = BotIdTextBox.GetLineText(0);

            try
            {
                bot = new TelegramBotClient(botid);
                var message = bot.GetMeAsync().Result;
                ConsoleLogMessage($"{message.FirstName} with id: {message.Id} has initialzed!");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Invalid bot token", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();
            EnableControlPanel();
        }

        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            if(bot != null)
            {
                bot.StopReceiving();
                bot = null;
                ConsoleLogMessage("Session was terminated");
                DisableControlPanel();
                ClearInfo();
                ClearEventInfo();
            }
        }

        private void ConsoleScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0)
            {
                if (ConsoleScrollViewer.VerticalOffset == ConsoleScrollViewer.ScrollableHeight)
                {
                    autoScroll = true;
                }
                else
                {
                    autoScroll = false;
                }
            }
            if (autoScroll == true && e.ExtentHeightChange != 0)
            {
                ConsoleScrollViewer.ScrollToVerticalOffset(ConsoleScrollViewer.ExtentHeight);
            }
        }

        private void ChatAddButton_Click(object sender, RoutedEventArgs e)
        {
            ChatEntryDialog chatEntryDialog = new ChatEntryDialog();

            if (chatEntryDialog.ShowDialog() == true)
            {
                ChatGroup newChat = new ChatGroup(chatEntryDialog.ResponseName, chatEntryDialog.ResponseId);
                chatGroupList.Add(newChat);
            }
            else
            {
                //The user canceled the dialog
            }
        }

        private void ChatRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmed = MessageBox.Show("Remove this chat group? \n All events will be lost.", "Remove Chat Group", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (confirmed == MessageBoxResult.Yes)
            {
                chatGroupList.Remove((ChatGroup)ChatListBox.SelectedItem);
                ClearInfo();
            }
            else
            {
                //The user clicked no
            }
        }

        private void ChatListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatListBox.SelectedIndex == -1 || ChatListBox.SelectedItem == null)
            {
                DisableInfoPanel();
                return;
            }

            EnableInfoPanel();
            ChatGroup chatGroup = (ChatGroup)ChatListBox.SelectedItem;
            ChatNameTextBox.Text = chatGroup.chatName;
            ChatIdTextBox.Text = chatGroup.chatId.ToString();
            EventListBox.ItemsSource = chatGroup.eventList;
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            EventDialog eventDialog = new EventDialog();

            if (eventDialog.ShowDialog() == true)
            {
                ChatGroup chatGroup = (ChatGroup)ChatListBox.SelectedItem;
                Event newEvent = new Event(eventDialog.ResponseName, eventDialog.ResponseDate, chatGroup.chatId);
                chatGroup.AddEvent(newEvent);
            }
            else
            {
                //The user canceled the dialog
            }
        }

        private void RemoveEventButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult response = MessageBox.Show("Remove event? \n Action cannot be undone.", "Remove Event", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (response == MessageBoxResult.Yes)
            {
                ChatGroup chatGroup = (ChatGroup)ChatListBox.SelectedItem;
                chatGroup.RemoveEvent((Event)EventListBox.SelectedItem);
                ClearEventInfo();
            }
            else
            {
                //The user clicked no
            }
        }

        private void EventListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Event selectedEvent = (Event)EventListBox.SelectedItem;

            if (selectedEvent == null)
            {
                return;
            }

            EventNameTextBox.Text = selectedEvent.eventName;
            EventDateTextBox.Text = selectedEvent.eventDate.ToString();
            if (selectedEvent.hasAlarm)
            { 
                EventAlarmTextBox.Text = selectedEvent.eventAlarmDate.ToString();
            }
        }

        private void SetAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            Event selectedEvent = (Event)EventListBox.SelectedItem;

            if (selectedEvent == null)
            {
                return;
            }

            EventAlarmDialog alarmDialog = new EventAlarmDialog();

            if (alarmDialog.ShowDialog() == true)
            {
                selectedEvent.SetAlarm(alarmDialog.ResponseDate);
                EventAlarmTextBox.Text = selectedEvent.eventAlarmDate.ToString();
                selectedEvent.AlarmActivated += OnAlarmActivated;
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new UpdateUIDelegate(ConsoleLogMessage), $"An Alarm for '{selectedEvent.eventName}'({selectedEvent.eventDate}) was set for {selectedEvent.eventAlarmDate}");
            }
            else
            {
                //The user canceled the dialog
            }

        }

        private void RemoveAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            Event selectedEvent = (Event)EventListBox.SelectedItem;

            if (selectedEvent == null)
            {
                return;
            }

            selectedEvent.RemoveAlarm();
            EventAlarmTextBox.Clear();
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new UpdateUIDelegate(ConsoleLogMessage), $"The alarm for '{selectedEvent.eventName}'({selectedEvent.eventDate}) was removed");
        }

        private void OnAlarmActivated(object sender, EventArgs e)
        {
            Event alarmEvent = (Event)sender;
            string message = $"This is a set reminder for the event '{alarmEvent.eventName}' on {alarmEvent.eventDate}";
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new UpdateUIDelegate(ConsoleLogMessage), message);
            Bot_SendMessage(message, alarmEvent.chatId);
        }

        private void ConsoleLogMessage(string message)
        {
            ConsoleOutputTextBox.AppendText($"[{DateTime.Now}] {message}\n");
        }

        private void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text == null)
            {
                return;
            }
            string consoleAppend = $"{e.Message.From.FirstName} {e.Message.From.LastName} @ {e.Message.Chat.Type}: {e.Message.Text}";
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new UpdateUIDelegate(ConsoleLogMessage), consoleAppend);
        }

        private async void Bot_SendMessage(string message, long id)
        {
            await bot.SendTextMessageAsync(id, message);
        }
    }
}

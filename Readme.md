#TelegramEventBot
##v2.0

**A handy Telegram bot that records event dates, and can even send reminder texts**

**New in v2.0:**
* TelegramEventBot now manages a list of events, so multiple events can be set and managed

TelegramEventBot is as easy to use as:  
1. Set up a new bot via "BotFather" (instructions from Telegram's website: https://core.telegram.org/bots#3-how-do-i-create-a-bot)
2. Update the `botID` in "BotMain.cs" to reflect your new bot's ID
3. Build and run the application
4. Add your bot to the chat group and begin issuing commands  

TelegramEventBot list of commands:
* /newevent "EventName"(DateTime) - Sets a new event date
* /currentevents - Displays the list of events
* /setreminder "EventName" (DateTime) - Sets a new date to remind the group of an event date
* /deleteevent "EventName" - Deletes the event
* /help - Lists all commands

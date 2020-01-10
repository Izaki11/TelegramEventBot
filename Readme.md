# TelegramEventBot
## v2.1

**A handy Telegram bot that records event dates, and can even send reminder texts**
*Due to the simplicity of this bot and the nature of Telegram's bot system, the use of a private chat id is highly encouraged*

**New in v2.1:**
* TelegramEventBot now supports a private chat id, for adhering to a single group or private chat

TelegramEventBot is as easy to use as:  
1. Set up a new bot via "BotFather" (instructions from Telegram's website: https://core.telegram.org/bots#3-how-do-i-create-a-bot)
2. Update the `botID` in "BotMain.cs" to reflect your new bot's ID
3. Build and run the application
4. Enter the desired chat id into the console
5. Add your bot to the chat group and begin issuing commands  

TelegramEventBot list of commands:
* /newevent "EventName"(DateTime) - Sets a new event date
* /currentevents - Displays the list of events
* /setreminder "EventName" (DateTime) - Sets a new date to remind the group of an event date
* /deleteevent "EventName" - Deletes the event
* /pingid - Requests the host console to display your current chat's id
* /help - Lists all commands

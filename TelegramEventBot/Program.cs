using System;

namespace TelegramEventBot
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("Please enter bot id: ");
                string botId = Console.ReadLine();
                Console.WriteLine("Please enter chat id (typing '/pingid' to bot will fetch chat id): \n");
                long id;
                try
                {
                    id = Convert.ToInt64(Console.ReadLine());
                }
                catch (Exception)
                {
                    id = 0;
                }

                try
                {
                    BotMain.Init(botId, id);
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}

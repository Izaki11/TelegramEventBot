using System;

namespace TelegramEventBot
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Please enter chat id: \n");
            long id;
            try
            {
                id = Convert.ToInt64(Console.ReadLine());
            }
            catch (Exception)
            {
                id = 0;
            }
            BotMain.Init(id);
        }
    }
}

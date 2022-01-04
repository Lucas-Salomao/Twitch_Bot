using System;
using System.Threading.Tasks;

namespace Twitch_Bot
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Bot my_bot = new Bot();

            my_bot.Connect(false);

            Console.ReadLine();

            my_bot.Disconnect();
        }
    }
}

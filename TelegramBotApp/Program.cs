using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramBotApp
{
    class Program
    {
        private static TelegramBotClient _botClient;

        static void Main(string[] args)
        {
            _botClient = new TelegramBotClient("1846376011:AAEHgzBmg7uYh6VsNiNmFs4oqEqIIJJYMO8");

            var me = _botClient.GetMeAsync().Result;
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

            _botClient.OnMessage += OnBotMessageReceived;
            _botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
        }

        private static async void OnBotMessageReceived(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                await _botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "You said:\n" + e.Message.Text
                );
            }
        }
    }
}
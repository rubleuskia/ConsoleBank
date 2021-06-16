using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounting;
using Accounting.Tracking;
using Common;
using Currencies.Apis.Byn;
using Currencies.Common.Caching;
using Currencies.Common.Conversion;
using Currencies.Common.Infos;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Portal
{
    class Program
    {
        private static IEventBus _eventBus = new EventBus();
        private static ICurrenciesApiCacheService apiCache =
            new CurrenciesApiCacheService(new BynCurrenciesApi());
        private static ICurrencyInfoService _infoService =
            new CurrencyInfoService(apiCache, new CurrencyConversionService(apiCache));

        private static IAccountsRepository repository = new AccountsRepository();
        private static IAccountAcquiringService acquiringService = new AccountAcquiringService(repository, _eventBus);
        private static ICurrencyConversionService conversionService = new CurrencyConversionService(apiCache);
        private static IAccountTransferService transferService =
            new AccountTransferService(repository, acquiringService,conversionService, _eventBus);

        private static AccountManagementService _accountManagementService =
            new(repository, acquiringService, transferService);

        private static AccountOperationsTrackingService _trackingService =
            new(() => DateTime.Now.AddHours(3), _eventBus);

        private static TelegramBotClient _botClient;

        static async Task Main(string[] args)
        {
            // await RunAccounting();
            // await RunInfo();

            // https://github.com/TelegramBots/Telegram.Bot
            // (!) DO NOT COMMIT PRIVATE TOKENS
            _botClient = new TelegramBotClient("1846376011:AAEHgzBmg7uYh6VsNiNmFs4oqEqIIJJYMO8");

            var cts = new CancellationTokenSource();

            _botClient.StartReceiving(
                new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync),
                cts.Token
            );

            Console.ReadLine();
            cts.Cancel();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(update.Message),
                UpdateType.EditedMessage => BotOnMessageReceived(update.Message),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(update.InlineQuery),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult),
                _ => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, ct);
            }
        }

        private static async Task BotOnMessageReceived(Message message)
        {
            Console.WriteLine($"Receive message type: {message?.Type}");
            if (message == null || message.Type != MessageType.Text)
            {
                return;
            }

            var action = (message.Text.Split(' ').First()) switch
            {
                "/inline" => SendInlineKeyboard(message),
                "/keyboard" => SendReplyKeyboard(message),
                "/photo" => SendFile(message),
                "/request" => RequestContactAndLocation(message),
                "/rate" => RequestRate(message),
                _ => Usage(message)
            };

            await action;

            // Send inline keyboard
            // You can process responses in BotOnCallbackQueryReceived handler
            static async Task SendInlineKeyboard(Message message)
            {
                await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                // Simulate longer running task
                await Task.Delay(500);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    }
                });

                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: inlineKeyboard
                );
            }

            static async Task SendReplyKeyboard(Message message)
            {
                var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "1.1", "1.2" },
                        new KeyboardButton[] { "2.1", "2.2" },
                    },
                    resizeKeyboard: true
                );

                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: replyKeyboardMarkup
                );
            }

            static async Task SendFile(Message message)
            {
                await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string filePath = @"Files/tux.png";

                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                await _botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: new InputOnlineFile(fileStream, fileName),
                    caption: "Nice Picture"
                );
            }

            static async Task RequestContactAndLocation(Message message)
            {
                var requestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });

                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Who or Where are you?",
                    replyMarkup: requestReplyKeyboard
                );
            }

            static async Task Usage(Message message)
            {
                const string usage = "Usage:\n" +
                                        "/inline   - send inline keyboard\n" +
                                        "/keyboard - send custom keyboard\n" +
                                        "/photo    - send a photo\n" +
                                        "/request  - request location or contact";

                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: usage,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }
        }

        private static async Task RequestRate(Message message)
        {
            var charCode = message.Text.Split(" ").Last();
            var rate = await _infoService.GetCurrencyRate(charCode);

            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"{charCode} rate today is: {rate}"
            );
        }

        // Process Inline Keyboard callback data
        private static async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            await _botClient.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"Received {callbackQuery.Data}"
            );

            await _botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"Received {callbackQuery.Data}"
            );
        }

        #region Inline Mode

        private static async Task BotOnInlineQueryReceived(InlineQuery inlineQuery)
        {
            Console.WriteLine($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };

            await _botClient.AnswerInlineQueryAsync(inlineQuery.Id, results, isPersonal: true, cacheTime: 0);
        }

        private static Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        #endregion

        private static Task UnknownUpdateHandlerAsync(Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }


        private static async Task RunAccounting()
        {
            var accountUsd = await _accountManagementService.CreateAccount(Guid.NewGuid(), "USD");
            var accountEur = await _accountManagementService.CreateAccount(Guid.NewGuid(), "EUR");
            var accountByn = await _accountManagementService.CreateAccount(Guid.NewGuid(), "BYN");
            var accountRur = await _accountManagementService.CreateAccount(Guid.NewGuid(), "RUR");

            await _accountManagementService.Acquire(accountUsd, 100);
            await _accountManagementService.Acquire(accountEur, 100);
            await _accountManagementService.Acquire(accountByn, 1000);
            await _accountManagementService.Acquire(accountRur, 10000);

            await _accountManagementService.Transfer(new AccountTransferParameters
            {
                FromAccount = accountEur,
                ToAccount = accountUsd,
                Amount = 7200,
                CurrencyCharCode = "RUB"
            });

            var usd = await _accountManagementService.GetAccount(accountUsd);
            var byn = await _accountManagementService.GetAccount(accountByn);
            var eur = await _accountManagementService.GetAccount(accountEur);
            var rur = await _accountManagementService.GetAccount(accountRur);

            foreach (var operation in _trackingService.GetOperations())
            {
                Console.WriteLine($"Operation: {operation.AccountId} - {operation.Type} - {operation.Amount}");
            }
        }

        private static async Task RunInfo()
        {
            var currencies = await _infoService.GetAvailableCurrencies();

            foreach (var currency in currencies)
            {
                Console.WriteLine(currency);
            }

            var usdRate = await _infoService.GetCurrencyRate("USD");
            var eurRate = await _infoService.GetCurrencyRate("EUR", DateTime.Now.AddDays(-1000));
            Console.WriteLine($"USD rate: {usdRate}");
            Console.WriteLine($"EUR rate: {eurRate}");

            var result1 = await _infoService.ConvertFromLocal(100, "USD");
            Console.WriteLine("1: " + result1);
            var result2 = await _infoService.ConvertToLocal(1000, "RUB");
            Console.WriteLine("2: " + result2);

            var avg = await _infoService.GetAvgRate("USD", new DateTime(2020, 1, 1), new DateTime(2020, 12, 31));
            var min = await _infoService.GetMinRate("USD", new DateTime(2020, 1, 1), new DateTime(2020, 12, 31));
            var max = await _infoService.GetMaxRate("USD", new DateTime(2020, 1, 1), new DateTime(2020, 12, 31));

            Console.WriteLine("avg: " + avg);
            Console.WriteLine("min: " + min);
            Console.WriteLine("max: " + max);
        }
    }
}

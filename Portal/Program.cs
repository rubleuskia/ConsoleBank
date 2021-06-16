﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            await RunAccounting();
            // await RunInfo();

            _botClient = new TelegramBotClient("1846376011:AAEHgzBmg7uYh6VsNiNmFs4oqEqIIJJYMO8");

            _botClient.OnMessage += OnBotMessage;
            _botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            _botClient.StopReceiving();
        }

        private static async void OnBotMessage(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message.Text;
            if (message == null)
            {
                return;
            }

            var parts = message.Split(" ");
            if (parts.First() == "rate")
            {
                var charCode = parts.Last();
                var rate = await _infoService.GetCurrencyRate(charCode);
                await _botClient.SendTextMessageAsync(
                    chatId: eventArgs.Message.Chat,
                    text: $"{charCode} rate: {rate}"
                );

                return;
            }

            Console.WriteLine($"Received a text message in chat {eventArgs.Message.Chat.Id}.");

            await _botClient.SendTextMessageAsync(
                chatId: eventArgs.Message.Chat,
                text: "You said:\n" + eventArgs.Message.Text
            );
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

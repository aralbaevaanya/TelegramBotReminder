using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramBot.SchedulerService;
using static TelegramBot.BotCommands.BotCommands;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace TelegramBot;

static class Program
{
	static ITelegramBotClient bot = new TelegramBotClient(ConfigurationManager.AppSettings["telegramToken"]);

	static async Task Main(string[] args)
	{
		Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
		var cts = new CancellationTokenSource();
		var cancellationToken = cts.Token;
		var receiverOptions = new ReceiverOptions
		{
			AllowedUpdates = { }, 
		};
		bot.StartReceiving(
			HandleUpdateAsync,
			HandleErrorAsync,
			receiverOptions,
			cancellationToken
		);
		await Schedule.StartScheduler(bot);
		Console.ReadLine();
	}
}
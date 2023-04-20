using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramBot.SchedulerService;
using static TelegramBot.BotCommands.BotCommands;

namespace TelegramBot;

static class Program
{
	static ITelegramBotClient bot = new TelegramBotClient("6130059484:AAFMihpOr0P2zScjiPgYi4DdatB4-BZbfSU");


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
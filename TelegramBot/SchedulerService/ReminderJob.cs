using Quartz;
using Telegram.Bot;
using static TelegramBot.BotCommands.BotCommands;

namespace TelegramBot.SchedulerService;

abstract class ReminderJob: IJob
{
	[Obsolete]
	public Task Execute(IJobExecutionContext context)
	{
		var schedulerContext = context.Scheduler.Context;
		var bot = (TelegramBotClient)schedulerContext.Get("bot");
		var tgId = (long)context.MergedJobDataMap["tgId"];
		Console.WriteLine($"started sending job for user {tgId}");
		return SendRandomNote(bot, tgId);
	}
}
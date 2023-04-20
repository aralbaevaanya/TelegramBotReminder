using Quartz;
using Telegram.Bot;
using static TelegramBot.BotCommands.BotCommands;

namespace TelegramBot.SchedulerService;

class ReminderJob: IJob
{
	[Obsolete]
	public Task Execute(IJobExecutionContext context)
	{
		Console.WriteLine($"started sending job");
		var schedulerContext = context.Scheduler.Context;
		var bot = (TelegramBotClient)schedulerContext.Get("bot");
		var tgId = (long)context.MergedJobDataMap["tgId"];
		return SendRandomNote(bot, tgId);
	}
}
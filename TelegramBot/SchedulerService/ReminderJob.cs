using Quartz;
using Telegram.Bot;
using TelegramBot.dbutils;
using static TelegramBot.BotCommands.BotCommands;
using static TelegramBot.SchedulerService.Schedule;

namespace TelegramBot.SchedulerService;

class ReminderJob: IJob
{
	private static readonly Random Random = new();
	[Obsolete]
	public async Task Execute(IJobExecutionContext context)
	{
		//step 1 - send a note
		var schedulerContext = context.Scheduler.Context;
		var bot = (TelegramBotClient)schedulerContext.Get("bot");
		var tgId = (long)context.MergedJobDataMap["tgId"];
		Console.WriteLine($"started sending job for user {tgId}");
		await SendRandomNote(bot, tgId);
		
		// step 2 - set the same job for next day
		var schedule = DataBaseMethods.GetSchedule(tgId).Result;
		var date = DateTime.Today.AddHours(12 + Random.Next(schedule.StartTimeOfDay, schedule.EndTimeOfDay));
		await AddScheduleJob(tgId, date);
	}
	
	/*public Task Execute(IJobExecutionContext context)
	{
		//step 1 - send a note
		var schedulerContext = context.Scheduler.Context;
		var bot = (TelegramBotClient)schedulerContext.Get("bot");
		var tgId = (long)context.MergedJobDataMap["tgId"];
		Console.WriteLine($"started sending job for user {tgId}");
		var sendRandomNote = SendRandomNote(bot, tgId);
		sendRandomNote.Wait(); 
		// step 2 - set the same job for next day
		var schedule = DataBaseMethods.GetSchedule(tgId).Result;
		var date = DateTime.Today.AddHours(12 + Random.Next(schedule.StartTimeOfDay, schedule.EndTimeOfDay));
		return  AddScheduleJob(tgId, date);
	}*/
}
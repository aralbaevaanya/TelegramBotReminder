using Quartz;
using Quartz.Impl;
using Telegram.Bot;
using TelegramBot.dbutils;

namespace TelegramBot.SchedulerService;

public static class Schedule
{
	private static readonly IScheduler Scheduler;

	static Schedule()
	{
		var factory = new StdSchedulerFactory();
		Scheduler = factory.GetScheduler().Result;
		Console.WriteLine("Init schedule");
	}
	public static async Task StartScheduler(ITelegramBotClient bot)
	{
		await Scheduler.Start();
		Scheduler.Context.Put("bot",bot);
		Console.WriteLine("Start schedule");
		IAsyncEnumerator<ReminderSchedule> scheduleEnumerator = DataBaseMethods.GetScheduleEnumerator();
		var rand = new Random();
		var tomorrowDateTime = DateTime.Today.AddDays(1);
		while (scheduleEnumerator.MoveNextAsync().Result)
		{
			ReminderSchedule reminderSchedule = scheduleEnumerator.Current;
			await AddScheduleJob(reminderSchedule.TgId,
				tomorrowDateTime.AddHours(rand.Next(reminderSchedule.StartTime,reminderSchedule.EndTime)));
		}
	}

	public static Task AddScheduleJob(long tgId) => AddScheduleJob(tgId,DateTime.Now.AddDays(1));
	
	public static async Task AddScheduleJob(long tgId, DateTimeOffset dateTime)
	{
		IJobDetail job = JobBuilder.Create<ReminderJob>()
			.WithIdentity(tgId.ToString(), "SendMessageJob")
			.UsingJobData("tgId", tgId)
			.Build();
		//this is block for test
		// ITrigger trigger = TriggerBuilder.Create()
		// 	.WithIdentity(tgId.ToString(), "SendMessageTrigger")
		// 	.StartNow()
		// 	.WithSimpleSchedule(x => x
		// 		.WithIntervalInSeconds(10)
		// 		.RepeatForever())
		// 	.Build();
		
		ITrigger trigger = TriggerBuilder.Create()
			.WithIdentity(tgId.ToString(), "SendMessageTrigger")
			.StartAt(dateTime)
			.WithSimpleSchedule(x => x.WithIntervalInMinutes(1).WithRepeatCount(0).Build())
			.Build();

		await Scheduler.ScheduleJob(job, trigger);
	}
}
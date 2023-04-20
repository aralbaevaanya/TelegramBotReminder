using Quartz;
using Quartz.Impl;
using Telegram.Bot;

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
	public static async Task InitScheduler(ITelegramBotClient bot)
	{
		await Scheduler.Start();
		Scheduler.Context.Put("bot",bot);
		Console.WriteLine("Start schedule");
	}
	public static async Task AddScheduleJob(long tgId)
	{
		IJobDetail job = JobBuilder.Create<ReminderJob>()
			.WithIdentity(tgId.ToString(), "SendMessageJob")
			.UsingJobData("tgId", tgId)
			.Build();
		ITrigger trigger = TriggerBuilder.Create()
			.WithIdentity(tgId.ToString(), "SendMessageTrigger")
			.StartNow()
			.WithSimpleSchedule(x => x
				.WithIntervalInSeconds(10)
				.RepeatForever())
			.Build();

		// Tell quartz to schedule the job using our trigger
		await Scheduler.ScheduleJob(job, trigger);
		await Scheduler.TriggerJob(job.Key);
	}
}
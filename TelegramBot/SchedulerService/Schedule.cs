using Microsoft.EntityFrameworkCore;
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
		DateTime today = DateTime.Today;
		IJobDetail job = JobBuilder.Create<FillScheduleForDayJob>()
			.WithIdentity(today.ToString(), "FillScheduleForToday")
			.UsingJobData("date", today.Ticks)
			.Build();
		ITrigger trigger = TriggerBuilder.Create()
			.WithIdentity(today.ToString(), "FillScheduleForTodayTrigger")
			.StartNow()
			.WithSimpleSchedule(x => x.WithIntervalInMinutes(1).WithRepeatCount(0).Build())
			.Build();
		await Scheduler.ScheduleJob(job, trigger);
	}

	public static async void StatsSendingScheduledMessageForUser(long tgId)
	{
		try
		{
			//save default schedule in db
			await DataBaseMethods.SaveScheduleForUser(tgId, 10, 22);
			//add job for tomorrow at 17:00 for example
			await AddScheduleJob(tgId, DateTime.Today.AddDays(1).AddHours(17));
		}
		catch (DbUpdateException e)
		{
			Console.WriteLine($"Schedule for user {tgId} already exist in database");
		}
	}
	
	public static async Task AddScheduleJob(long tgId, DateTimeOffset dateTime)
	{
		IJobDetail job = JobBuilder.Create<ReminderJob>()
			.WithIdentity(tgId.ToString(), "SendMessageJob")
			.UsingJobData("tgId", tgId)
			.Build();
		
		ITrigger trigger = TriggerBuilder.Create()
			.WithIdentity(tgId.ToString(), "SendMessageTrigger")
			.StartAt(dateTime)
			.WithSimpleSchedule(x => x.WithIntervalInMinutes(1).WithRepeatCount(0).Build())
			.Build();

		await Scheduler.ScheduleJob(job, trigger);
	}
}
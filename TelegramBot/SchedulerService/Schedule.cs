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
		Scheduler.Context.Put("bot", bot);
		Console.WriteLine("Start scheduler");

		var todayDate = DateTime.Today.AddHours(-12); //12:00AM -> 00:00AM
		var rand = new Random();
		var scheduleBook = DataBaseMethods.GetSchedules();
		foreach (var scheduleList in scheduleBook)
		{
			foreach (var schedule in scheduleList)
			{
				await AddScheduleJob(schedule.TgId,
					todayDate.AddHours(rand.Next(schedule.StartTimeOfDay, schedule.EndTimeOfDay)));
			}
		}
	}

	public static async void StartSendingScheduledMessageForUser(long tgId)
	{
		try
		{
			//save default schedule in db
			await DataBaseMethods.SaveScheduleForUser(tgId, 10, 22);
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
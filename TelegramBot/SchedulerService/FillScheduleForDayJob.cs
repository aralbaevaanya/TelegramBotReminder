using Quartz;
using TelegramBot.dbutils;

namespace TelegramBot.SchedulerService;

abstract class FillScheduleForDayJob: IJob
{
	[Obsolete]
	public Task Execute(IJobExecutionContext context)
	{
		var dateTicks = (long)context.MergedJobDataMap["date"];
		var dateTime = new DateTime(dateTicks).AddHours(-12); //12AM -> 00:00
		var scheduleEnumerator = DataBaseMethods.GetScheduleEnumerator();
		var rand = new Random();
		while (scheduleEnumerator.MoveNext())
		{
			var reminderSchedule = scheduleEnumerator.Current;
			var notificationDateTime
				= dateTime.AddHours(rand.Next(reminderSchedule.StartTimeOfDay, reminderSchedule.EndTimeOfDay));
			try
			{
				Task.Run(() => Schedule.AddScheduleJob(reminderSchedule.TgId, notificationDateTime));
			}
			catch(Exception e)
			{
				Console.WriteLine($"Exception during adding task to quartz for day {dateTime.Date}, message: {e.Message}");
			}
		}
		return Task.CompletedTask;
	}
}
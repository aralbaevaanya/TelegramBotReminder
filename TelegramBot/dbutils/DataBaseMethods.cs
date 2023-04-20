using Microsoft.EntityFrameworkCore;

namespace TelegramBot.dbutils;

public static class DataBaseMethods
{
	private static readonly Random Rand = new Random();

	public static async Task<Note?> GetRandomNote(long tgId)
	{
		var notes = await GetAllNotes(tgId);
		return (notes.Count > 0)? notes[Rand.Next(0, notes.Count)] : null;
	}
	
	public static async Task<List<Note>> GetAllNotes(long tgId)
	{
		//to-do add cache
		using (ApplicationContext db = new ApplicationContext())
		{
			var result = await Task.Run(() => db.Note.FromSql($"SELECT * FROM public.\"Note\" WHERE \"TgId\" = {tgId}").ToList());
			return result;
		}
	}
	
	public static async Task AddOrUpdateUser(long tgId, long tgChatId)
	{
		using (ApplicationContext db = new ApplicationContext())
		{
			var user = await db.Users.FirstOrDefaultAsync(x => x.TgId == tgId);
			if (user is null)
			{
				var newUser = new Users { TgId = tgId, TgChatId = tgChatId };
				await db.Users.AddAsync(newUser);
				await db.SaveChangesAsync();
			}
			else
			{
				if (user.TgChatId != tgChatId)
				{
					user.TgChatId = tgChatId;
					db.Users.Update(user);
					await db.SaveChangesAsync();
				}
			}
		}
	}

	public static async Task AddNote(long tgId, string note)
	{
		using (ApplicationContext db = new ApplicationContext())
		{
			await db.Note.AddAsync(new Note { TgId = tgId, TextValue = note});
			await db.SaveChangesAsync();
		}
	}

	public static async Task<ReminderSchedule?> GetSchedule(long tgId)
	{
		using (ApplicationContext db = new ApplicationContext())
		{
			var result = await db.Schedules.FindAsync(tgId);
			return result;
		}
	}
	public static IAsyncEnumerator<ReminderSchedule> GetScheduleEnumerator()
	{
		using (ApplicationContext db = new ApplicationContext())
		{
			var result = db.Schedules.GetAsyncEnumerator();
			return result;
		}
	}
	public static async Task SetSchedule(long tgId, int startTime, int endTime)
	{
		using (ApplicationContext db = new ApplicationContext())
		{
			await db.Schedules.AddAsync(new ReminderSchedule {TgId = tgId, StartTime = startTime, EndTime = endTime});
			await db.SaveChangesAsync();

		}
	}
}
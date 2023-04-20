using System.ComponentModel.DataAnnotations;

namespace TelegramBot.dbutils
{
	public class ReminderSchedule
	{
		[Key]
		public long TgId { get; set; }
		public int StartTime {get; set;}
		public int EndTime { get; set; }
		
	}
}

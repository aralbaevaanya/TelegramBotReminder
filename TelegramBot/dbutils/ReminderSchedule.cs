using System.ComponentModel.DataAnnotations;

namespace TelegramBot.dbutils
{
	public class ReminderSchedule
	{
		[Key]
		public long TgId { get; set; }
		public int StartTimeOfDay {get; set;}
		public int EndTimeOfDay { get; set; }
		
	}
}

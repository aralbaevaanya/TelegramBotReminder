using System.ComponentModel.DataAnnotations;

namespace TelegramBot.dbutils;

public class Note
{
	public long TgId { get; set; }
	public string TextValue { get; set; }
	[Key]
	public Guid NoteId { get; set; }
}
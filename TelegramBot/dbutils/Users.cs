using System.ComponentModel.DataAnnotations;
namespace TelegramBot.dbutils;

public class Users
{
	[Key]
	public long TgId { get; set; }
	public long TgChatId { get; set; }
}
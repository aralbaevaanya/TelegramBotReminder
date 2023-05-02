using Microsoft.EntityFrameworkCore;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace TelegramBot.dbutils;

public class ApplicationContext : DbContext
{
	public DbSet<Users> Users { get; set; }
	public DbSet<Note> Note { get; set; }
	public DbSet<ReminderSchedule> Schedules { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql(ConfigurationManager.AppSettings["connectionString"]);
	}
}
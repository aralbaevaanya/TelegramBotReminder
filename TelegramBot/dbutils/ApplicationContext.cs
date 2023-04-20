using Microsoft.EntityFrameworkCore;

namespace TelegramBot.dbutils;

public class ApplicationContext : DbContext
{
	public DbSet<Users> Users { get; set; }
	public DbSet<Note> Note { get; set; }
	public DbSet<ReminderSchedule> Schedules { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql(
			"Host=localhost;" +
			"Port=5432;" +
			"Database=Users;" +
			"Username=postgres;" +
			"Password=03032013Aa");
	}
}
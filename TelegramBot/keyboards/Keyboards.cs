using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.keyboards;

public static class Keyboards
{
	public static readonly InlineKeyboardMarkup InlineKeyboard = new InlineKeyboardMarkup(new[]
	{
		new []{InlineKeyboardButton.WithCallbackData("Показать все мои заметки","showAllNotes")},
		new []{InlineKeyboardButton.WithCallbackData("Показать рандомную заметку", "showNote")}
	});
	public static ReplyKeyboardMarkup ReplyKeyboard = new ReplyKeyboardMarkup(new []
	{
		new []{new KeyboardButton("/showAllNotes")},
		new []{new KeyboardButton("/showNote")}
	});
}
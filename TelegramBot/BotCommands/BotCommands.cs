using Telegram.Bot;
using TelegramBot.dbutils;
using TelegramBot.keyboards;
using Telegram.Bot.Types.Enums;
using Update = Telegram.Bot.Types.Update;
namespace TelegramBot.BotCommands;

public static class BotCommands
{
	
	public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
		CancellationToken cancellationToken)
	{
		Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
		if (update.Type == UpdateType.Message)
		{
			var message = update.Message;
			if (message == null) return;
			if (message.Text.ToLower() == "/start")
			{
				await DataBaseMethods.AddOrUpdateUser(message.From.Id, message.Chat.Id);
				await SchedulerService.Schedule.AddScheduleJob(message.From.Id);
				await botClient.SendTextMessageAsync(
					chatId: message.Chat,
					text: "Привет, напиши заметку, и однажды я напомню тебе о ней",
					replyMarkup: Keyboards.ReplyKeyboard);

				return;
			}

			if (!message.Text.StartsWith("/"))
			{
				var ans = DataBaseMethods.AddNote(message.From.Id, message.Text).IsCompletedSuccessfully
					? "Заметка добавлена"
					: "о-оу, что-то пошло не так";
				await botClient.SendTextMessageAsync(
					chatId: message.Chat,
					text: ans,
					replyMarkup:Keyboards.InlineKeyboard);
			}
		}

		if (update.Type == UpdateType.CallbackQuery)
		{
			// Тут получает нажатия на inline кнопки
			var callBackQuery = update.CallbackQuery;

			async void SendNote(Note? notes) => await botClient.SendTextMessageAsync(
				callBackQuery.Message.Chat.Id, text: notes.TextValue, replyMarkup: Keyboards.ReplyKeyboard);

			try
			{
				if (callBackQuery.Data == "showNote")
				{
					var request = DataBaseMethods.GetRandomNote(callBackQuery.From.Id);
					SendNote(request.Result);
				}

				if (callBackQuery.Data == "showAllNotes")
				{
					var request = DataBaseMethods.GetAllNotes(callBackQuery.From.Id);
					request.Result.ForEach(SendNote);
				}
			}
			catch (Exception) // clarify exception type
			{
				await botClient.SendTextMessageAsync(callBackQuery.Message.Chat.Id,
					text: "Кажется, у вас еще нет заметок", replyMarkup:Keyboards.ReplyKeyboard);
			}
		}
	}

	public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
		CancellationToken cancellationToken)
	{
		Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
	}

	public static async Task SendRandomNote(ITelegramBotClient botClient, long tgId)
	{
		var request = DataBaseMethods.GetRandomNote(tgId);
		if (request.Result.TextValue != null)
		{
			await botClient.SendTextMessageAsync(
				tgId, text: request.Result.TextValue, replyMarkup: Keyboards.ReplyKeyboard);
		}
	}
}
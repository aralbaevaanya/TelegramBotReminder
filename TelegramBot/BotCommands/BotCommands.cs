using Telegram.Bot;
using TelegramBot.dbutils;
using TelegramBot.keyboards;
using static TelegramBot.SchedulerService.Schedule;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using UpdateType = Telegram.Bot.Types.Enums.UpdateType;

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
			var messageText = message.Text;
			switch (messageText.ToLower())
			{
				case "/start": StartCommand(botClient, message); break;
				case "/shownote": SendRandomNote(botClient, message.From.Id); break;
				case "/showallnotes": SendAllNotes(botClient, message.From.Id); break;
			}

			if (!message.Text.StartsWith("/"))
			{
				var task = DataBaseMethods.AddNote(message.From.Id, messageText);
				task.Wait();
				var ans = task.IsCompletedSuccessfully ? "Заметка добавлена" : "о-оу, что-то пошло не так";
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
			try
			{
				switch (callBackQuery.Data)
				{
					case "showNote":
						await SendRandomNote(botClient, callBackQuery.From.Id);
						break;
					case "showAllNotes":
						SendAllNotes(botClient, callBackQuery.From.Id);
						break;
				}
			}
			catch (Exception) //to-do: clarify exception type
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

	public static void SendAllNotes(ITelegramBotClient botClient, long tgId)
	{
		async void SendNote(Note? notes) => await botClient.SendTextMessageAsync(
			tgId, text: notes.TextValue, replyMarkup: Keyboards.ReplyKeyboard);
		
		var request = DataBaseMethods.GetAllNotes(tgId);
		request.Result.ForEach(SendNote);
	}
	
	private static async void StartCommand(ITelegramBotClient botClient, Message message)
	{
		await DataBaseMethods.AddOrUpdateUser(message.From.Id, message.Chat.Id);
		StartSendingScheduledMessageForUser(message.From.Id);
		await botClient.SendTextMessageAsync(
			chatId: message.Chat,
			text: "Привет, напиши заметку, и однажды я напомню тебе о ней",
			replyMarkup: Keyboards.ReplyKeyboard);
	}
	
}
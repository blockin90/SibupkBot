using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace SibupkScheduleBot.Services
{
    public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IUpdateHandler
    {

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogInformation("HandleError: {Exception}", exception);
            // Cooldown in case of network connection error
            if (exception is RequestException) {
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogInformation("HandleError: {Exception}", exception);
            // Cooldown in case of network connection error
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await (update switch
            {
                { Message: { } message } => OnMessage(message),
                _ => UnknownUpdateHandlerAsync(update)
            });
        }


        private async Task OnMessage(Message msg)
        {
            logger.LogInformation("Receive message type: {MessageType}", msg.Type);
            if (msg.Text is not { } messageText)
                return;

            Message sentMessage = await (messageText.Split(' ')[0] switch
            {
                "/photo" => Usage(msg),
                _ => Usage(msg)
            });
            logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
        }

        async Task<Message> Usage(Message msg)
        {
            const string usage = """
                <b><u>Bot menu</u></b>:
            """;
            return await bot.SendTextMessageAsync(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }
    }
}

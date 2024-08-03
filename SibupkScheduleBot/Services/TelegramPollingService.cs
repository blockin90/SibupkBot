using SibupkScheduleBot.Abstract;

namespace SibupkScheduleBot.Services
{

    // Compose Polling and ReceiverService implementations
    public class TelegramPollingService(IServiceProvider serviceProvider, ILogger<TelegramPollingService> logger)
        : PollingServiceBase<ReceiverService>(serviceProvider, logger);
}

namespace SibupkScheduleBot.Services
{
    public class TelegramApiService
    {
        public TelegramApiService(IHttpClientFactory factory) 
        {
            factory.CreateClient(Constants.telegramApiHttpClientName);
        }
    }
}

namespace SibupkScheduleBot.Services
{
    public class SibupkPageDownloaderService
    {
        HttpClient _httpClient;
        public SibupkPageDownloaderService(HttpClient client)
        {
            _httpClient = client;
        }
    }
}

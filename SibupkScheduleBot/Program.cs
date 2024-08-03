
using Microsoft.Extensions.Options;
using Quartz;
using SibupkScheduleBot.Jobs;
using SibupkScheduleBot.Services;
using Telegram.Bot;

namespace SibupkScheduleBot
{
    public class Program
    {
        internal const string nskTimeZoneName = "N. Central Asia Standard Time";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection("BotConfiguration"));

            builder.Services.AddHttpClient(Constants.telegramApiHttpClientName)
                .RemoveAllLoggers()
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfiguration = sp.GetService<IOptions<BotConfiguration>>()?.Value;
                    ArgumentNullException.ThrowIfNull(botConfiguration);
                    TelegramBotClientOptions options = new(botConfiguration.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });
;
            builder.Services.AddHttpClient<SibupkPageDownloaderService>( httpClient =>
            {
                httpClient.BaseAddress = new Uri("");
            });
            builder.Services.AddScoped<UpdateHandler>();
            builder.Services.AddScoped<ReceiverService>();
            builder.Services.AddHostedService<TelegramPollingService>();

            //builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddQuartz(q =>
            {
                q.UseInMemoryStore();
                JobKey checkSiteUpdatesJobKey = new JobKey(nameof(CheckScheduleUpdatesJob));
                q.AddJob(typeof(CheckScheduleUpdatesJob), checkSiteUpdatesJobKey);
                q.AddTrigger(triggerConfig => triggerConfig
                    .ForJob(checkSiteUpdatesJobKey)
                    .WithCronSchedule(
                        "0 0 7-23 * * ?", //every hour from 7 to 23
                        cronBuilder => cronBuilder.InTimeZone( TimeZoneInfo.FindSystemTimeZoneById(nskTimeZoneName))
                     )
                );

                JobKey rabbitPollingJobKey = new JobKey(nameof(RabbitMqPoolingJob));
                q.AddJob(typeof(RabbitMqPoolingJob), rabbitPollingJobKey);
                q.AddTrigger(triggerConfig => triggerConfig
                    .ForJob(rabbitPollingJobKey)
                    .WithSimpleSchedule( builder => 
                        builder.WithInterval(TimeSpan.FromMicroseconds(500))
                            .RepeatForever()
                     )
                );
            });

            builder.Services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

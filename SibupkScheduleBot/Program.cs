
using Quartz;
using SibupkScheduleBot.Jobs;

namespace SibupkScheduleBot
{
    public class Program
    {
        internal const string nskTimeZoneName = "N. Central Asia Standard Time";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
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

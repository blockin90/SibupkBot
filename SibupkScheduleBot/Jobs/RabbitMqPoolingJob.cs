using Quartz;
using System.Diagnostics;

namespace SibupkScheduleBot.Jobs
{
    public class RabbitMqPoolingJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine($"RabbitMqPoolingJob executed at {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}

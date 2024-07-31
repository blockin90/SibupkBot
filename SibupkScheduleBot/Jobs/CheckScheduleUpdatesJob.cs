using Quartz;
using System.Diagnostics;

namespace SibupkScheduleBot.Jobs
{
    public class CheckScheduleUpdatesJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine($"{nameof(CheckScheduleUpdatesJob)} job executed at {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}

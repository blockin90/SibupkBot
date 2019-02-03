using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UpkModel.Database;
using UpkServices;

namespace TelegramClientCore.BotCacheDatabase
{
    public class CachedDataLoader : IDataLoader<WorkDay>
    {
        private readonly IEnumerable<WorkDay> workDays;

        public CachedDataLoader(IEnumerable<WorkDay> workDays)
        {
            this.workDays = workDays;
        }

        public IEnumerable<WorkDay> Load()
        {
            return workDays;
        }

        public Task<IEnumerable<WorkDay>> LoadAsync()
        {
            return Task.Run( ()=>workDays );
        }
    }
}

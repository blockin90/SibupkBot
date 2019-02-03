using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using UpkModel.Database;

namespace TelegramClientCore.BotCache.Database
{
    public class CachedGroupWorkDay
    {
        public int Id { get; set; }

        public WorkDay WorkDay { get; set; }
        public int WorkDayId { get; set; }

        [NotMapped]
        public DateTime Date
        {
            get => WorkDay.Date;
            set => WorkDay.Date = value;
        }

        public int CachedGroupId { get; set; }
        /// <summary>
        /// кэшированный объект группы, для которой сохранено расписание
        /// </summary>
        public virtual CachedGroup CachedGroup { get; set; }

        public CachedGroupWorkDay()
        {
            WorkDay = new WorkDay();
        }
        
        public CachedGroupWorkDay(int groupId, WorkDay workDay)
        {
            CachedGroupId = groupId;
            WorkDay = workDay;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TelegramClientCore.BotCache.Database
{
    public class CachedGroup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CachedGroupId { get; set; }

        /// <summary>
        /// последняя дата, для которой для данной группы загружалось расписание
        /// </summary>
        /// <remarks>данное поле необходимо для того, чтобы не перезагружать расписание, 
        /// если последняя загружаемая дата пришлась на выходной день и расписание на него отсутствует.
        /// В противном случае можно было бы брать последнюю дату CachedGroupWorkDays</remarks>
        public DateTime CachedTo { get; set; }

        public virtual ICollection<CachedGroupWorkDay> CachedGroupWorkDays { get; set; }
    }
}

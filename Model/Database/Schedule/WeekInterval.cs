using System;
using System.Collections.Generic;
using System.Text;

namespace UpkModel.Database.Schedule
{
    /// <summary>
    /// Интервал дат из понедельного вывода с сайта сибупк
    /// </summary>
    /// <remarks>представляет строку виду 02.04.2018 - 15.04.2018</remarks>
    public class WeekInterval
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string OriginalString { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UpkModel.Database.Users
{
    //ужасный с точки зрения БД класс :(
    public class UserConfig
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ChatId { get; set; }

        public bool ShowFullTime { get; set; }

        public bool ShowTeachers { get; set; }

        public bool ShowHolidays { get; set; }

        public bool EnableSiteUpdatesNotification { get; set; }

        public bool EnableBotUpdatesNotification { get; set; }

    }
}

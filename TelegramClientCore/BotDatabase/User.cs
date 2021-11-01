using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TelegramClientCore.BotDatabase
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ChatId { get; set; }

        [MaxLength(64)]
        public string UserName{ get; set; }
        [MaxLength(64)]
        public string LastName{ get; set; }
        [MaxLength(64)]
        public string FirstName{ get; set; }


        /// <summary>
        /// Название состояния,в котором находится пользователь
        /// </summary>
        [MaxLength(128)]
        public string StateName { get; set; }

        /// <summary>
        /// Информация для восстановления контекста.
        /// Для преподавателя - ФИО (id на сайте нет, в БД при пересоздании м.б. сгенерировано другое значение)
        /// Для студента - ...
        /// </summary>
        public string ExtraData { get; set; }
    }
}

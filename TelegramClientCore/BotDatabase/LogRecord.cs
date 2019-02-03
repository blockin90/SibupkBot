using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TelegramClientCore.BotDatabase
{
    public class LogRecord
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// Идентификатор чата
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// Время записываемого события
        /// </summary>
        public DateTime RecordTime { get; set; }


        /// <summary>
        /// Текущее состояние чата до обработки сообщения
        /// </summary>
        [MaxLength(128)]
        public string CurrentState { get; set; }
        /// <summary>
        /// Сообщение-запрос пользователя
        /// </summary>
        [MaxLength(64)]
        public string Message { get; set; }
    }
}

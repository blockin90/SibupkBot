using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UpkModel.Database.Users
{
    public class LogRecord
    {
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
        [MaxLength(128)]
        public string Message { get; set; }
    }
}

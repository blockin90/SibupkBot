using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UpkModel.Database
{
    /// <summary>
    /// Класс для настроек в формате ключ-значение
    /// </summary>
    public class Config
    {
        [Key, MaxLength(100)]
        public string Key { get; set; }
        [Required,MaxLength(255)]
        public string Value { get; set; }
    }
}

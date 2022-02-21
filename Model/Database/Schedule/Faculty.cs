using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UpkModel.Database.Schedule
{
    /// <summary>
    /// Факультет
    /// </summary>
    public class Faculty
    {
        public int Id { get; set; }

        /// <summary>
        /// Название факультета
        /// </summary>
        [Required, MaxLength(200)]
        public string Name { get; set; }
    }
}

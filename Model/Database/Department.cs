using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UpkModel.Database
{
    /// <summary>
    /// Кафедра
    /// </summary>
    public class Department
    {
        public int Id { get; set; }
        [Required,MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}

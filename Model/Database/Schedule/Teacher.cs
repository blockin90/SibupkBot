using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpkModel.Database.Schedule
{
    /// <summary>
    /// преподаватель
    /// </summary>
    public class Teacher : Actor
    {
        public int Id { get; set; }

        /// <summary> Кафедра</summary>
        public virtual Department Department { get; set; }
        public int? DepartmentId { get; set; }

        /// <summary> Полное ФИО  </summary>
        [Required, MaxLength(200)]
        public string FIO { get; set; }
        /// <summary>
        /// Сокращенное ФИО в формате Фамилия И.О., для сопоставления с расписанием, выгруженным с сайта (по аудиториям, по студентам)
        /// </summary>
        [Required,MaxLength(50)]
        public string ShortFio { get; set; }

        public override string IdentifierName => FIO;

        /*
        public Teacher()
        {
            IdentifierName = FIO;
        }*/

    }
}

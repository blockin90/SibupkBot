using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace UpkModel.Database.Schedule
{
    /// <summary>
    /// учебная группа
    /// </summary>
    /// <remarks>
    /// названия полей взяты из параметров POST запроса
    /// </remarks>
    public class Group : Actor
    {

        public int Id { get; set; }
        /// <summary>
        /// название подгруппы
        /// </summary>
        [Required, MaxLength(128)]
        public string NamePodGrup { get; set; }
        /// <summary>
        /// название подгруппы
        /// </summary>
        [Required, MaxLength(32)]
        public string ShortName { get; set; }
        /// <summary>
        /// форма обучения: 1 - очная
        /// </summary>
        public int id_Forma { get; set; }
        /// <summary>
        /// id факультета
        /// </summary>
        public int id_Fak { get; set; }

        /// <summary>
        /// курс обучения
        /// </summary>
        public int Kurs
        {
            get;set;
        }

        public override string IdentifierName => ShortName;
    }
}

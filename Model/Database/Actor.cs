using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UpkModel.Database
{
    public class ActorComparer : IComparer<Actor>
    {
        public int Compare(Actor x, Actor y)
        {
            return x.IdentifierName.CompareTo(y.IdentifierName);
        }
    }


    /// <summary>
    /// Сторона, для которой может быть загружено расписание.
    /// Например, преподаватель или группа
    /// </summary>
    public abstract class Actor
    {
       // private string _identifierName = String.Empty;

        /// <summary>
        /// Строка, представляющая данный объект
        /// </summary>
        [NotMapped]
        public abstract string IdentifierName
        {
            get;
           // {
               // return _identifierName;
           // }
            //protected set
            //{
             //   _identifierName = value.Trim().ToUpper();
            //}
        }
    }
}

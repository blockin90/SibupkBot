using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UpkModel.Database.Schedule
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

    public class ActorsEqualityComparer : IEqualityComparer<Actor>
    {
        public bool Equals(Actor x, Actor y)
        {
            return x.IdentifierName == y.IdentifierName;
        }

        public int GetHashCode(Actor obj)
        {
            return obj.IdentifierName.GetHashCode();
        }
    }
}

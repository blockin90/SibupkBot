using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UpkModel.Database;
using UpkModel.Database.Schedule;

namespace TelegramClientCore.BotServices
{
    /// <summary>
    /// Класс для определения типа пользователя по заданной строке, 
    /// т.е. поиск среди фамилий преподавателей и групп студентов
    /// </summary>
    /// <remarks>
    /// блокировки не ставятся, т.к. внешний доступ только на чтение
    /// </remarks>
    public class ActorByNameResolver : IActorResolver
    {
        /// <summary>
        /// регулярное выражения для вычленения названия группы и ее номера из строк вида пиб51, пиб-51, пиб 51.1 и т.п.
        /// </summary>
        static Regex groupRegex = new Regex(@"^([А-ЯЁ]+)[ \-]*(\d+.*)$");

        private Dictionary<char, SortedList<string, Actor>> _availableActors;

        public ActorByNameResolver(IEnumerable<Teacher> teachers, IEnumerable<UpkModel.Database.Schedule.Group> groups)
        {
            var emptySet = new Actor[0];
            var set = emptySet.Union(groups).Union(teachers);
            MakeActorsDictionary(set);
        }

        private void MakeActorsDictionary(IEnumerable<Actor> actors)
        {
            /* формируем словарь с ключом - первой буквой фио преподавателя или названия группы
             * затем для этого ключа формируем сортированный список по заданному идентификатору
             */
            _availableActors = actors.Distinct(new ActorsEqualityComparer()).GroupBy(a => a.IdentifierName.Substring(0,1).ToUpper()[0])
                .ToDictionary(group => group.Key, 
                            group => new SortedList<string, Actor>( group.ToDictionary( g=> g.IdentifierName.ToUpper())));
        }

        /// <summary>
        /// Преобразование пользовательского ввода к формату ГРУППА-НОМЕР в тех случаях, 
        /// когда в названии содержится число
        /// </summary>
        /// <returns></returns>
        string CorrectGroupName(string name)
        {
            name = name.ToUpper().Trim();
            var match = groupRegex.Match(name);
            if (match.Success) {
                return match.Groups[1].Value + "-" + match.Groups[2].Value.TrimEnd();
            }
            return name;
        }

        /// <summary>
        /// Попытка определения объекта по его имени
        /// </summary>
        public bool TryToGetActor(string userInput, out Actor actor)
        {
            actor = null;
            userInput = userInput.Trim().ToUpper();
            userInput = CorrectGroupName(userInput);
            if (_availableActors.TryGetValue(userInput[0], out SortedList<string,Actor> actorsSet)) {
                return actorsSet.TryGetValue(userInput, out actor);
            }
            return false;
        }

        /// <summary>
        /// Получение списка Actor'ов, начинающихся с userInput
        /// </summary>
        /// <param name="userInput">начало названия преподавателя или группы</param>
        /// <returns>возможные варианты</returns>
        public IEnumerable<string> GetSimilarActors(string userInput)
        {
            userInput = userInput.Trim().ToUpper();
            userInput = CorrectGroupName(userInput);
            if (_availableActors.TryGetValue(userInput[0], out SortedList<string, Actor> actorsSet)) {
                return actorsSet.Where(a => a.Key.StartsWith(userInput)).Select(a => a.Value.IdentifierName);
            }
            return new string[0];
        }

    }
}

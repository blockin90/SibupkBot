using System.Collections.Generic;
using UpkModel.Database;

namespace TelegramClientCore.BotServices
{
    internal interface IActorResolver
    {
        /// <summary>
        /// Попытка определения объекта по его имени
        /// </summary>
        bool TryToGetActor(string userInput, out Actor actor);

        /// <summary>
        /// Получение списка Actor'ов, начинающихся с userInput
        /// </summary>
        /// <param name="userInput">начало названия преподавателя или группы</param>
        /// <returns>возможные варианты</returns>
        IEnumerable<string> GetSimilarActors(string userInput);
    }
}

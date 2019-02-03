using System.Collections.Generic;
using System.Threading.Tasks;

namespace UpkServices
{
    /// <summary>
    /// Интерфейс загрузчика данных, не требующего параметров (список преподавателей, учебных недель и т.п.)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataLoader<T>
    {
        /// <summary>
        /// Загрузка объектов типа T
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Load();
        /// <summary>
        /// Загрузка объектов типа T
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> LoadAsync();
    }
}

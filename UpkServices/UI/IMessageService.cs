using System;
using System.Collections.Generic;
using System.Text;

namespace UpkServices.UI
{
    /// <summary>
    /// Перечисление кнопок диалога
    /// </summary>
    /// <remarks>
    /// реализовано для обеспечения кроссплатформенности
    /// </remarks>
    public enum Buttons
    {
        Ok,
        YesNo,
        YesNoCancel,
        OkCancel
    }
    /// <summary>
    /// Сервис всплывающих текстовых сообщений
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Показать диалоговое окно с заданным сообщением
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="buttons">Доступные кнопки диалога</param>
        /// <returns>true - если нажаты Ok, Yes, false - если нажаты No, null - при закрытии окна или Cancel</returns>
        bool? ShowDialog(string message, string caption, Buttons button);
    }
}

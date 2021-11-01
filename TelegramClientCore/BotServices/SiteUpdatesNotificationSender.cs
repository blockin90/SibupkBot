using System;
using System.Collections.Generic;
using System.Text;
using UpkServices;

namespace TelegramClientCore.BotServices
{
    /// <summary>
    /// Отправитель сообщений об обновлении сайта
    /// </summary>
    class SiteUpdatesNotificationSender
    {
        ISiteUpdateNotificator _updateNotificator;
        MessageSender _messageSender;
        public SiteUpdatesNotificationSender()
        {
            _updateNotificator = ServiceProvider.GetService<ISiteUpdateNotificator>();
            _messageSender = ServiceProvider.GetService<MessageSender>();
            _updateNotificator.OnSiteUpdate += UpdateNotificator_OnSiteUpdate;
        }

        private void UpdateNotificator_OnSiteUpdate(object sender, EventArgs e)
        {
            _messageSender.SendScheduleChangingNotification("❗️ На сайте расписания есть изменения! Рекомендуется проверить загруженное ранее расписание.");    
        }
    }
}

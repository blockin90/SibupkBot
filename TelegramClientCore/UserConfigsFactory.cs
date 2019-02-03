using System;
using System.Collections.Generic;
using System.Text;
using TelegramClientCore.BotDatabase;
using TelegramClientCore.BotServices;

namespace TelegramClientCore
{
    public class UserConfigsFactory
    {
        private static UserConfigsFactory configsFactory;

        public static UserConfigsFactory Instance
        {
            get { return configsFactory ?? (configsFactory = new UserConfigsFactory()); }
        }

        private UserConfigsFactory()
        { }

        public IUserConfigService GetUserConfigService(long chatId)
        {

            var config = BotDbContext.Instance.UserConfigs.Find(chatId);
            if( config == null) {
                config = new UserConfig()
                {
                    ChatId = chatId,
                    EnableBotUpdatesNotification = true,
                    EnableSiteUpdatesNotification = true,
                    ShowFullTime = false,
                    ShowTeachers = false
                };
                BotDbContext.Instance.UserConfigs.Add(config);
            }
            return new UserConfigService(config, BotDbContext.Instance);
        }
    }
}

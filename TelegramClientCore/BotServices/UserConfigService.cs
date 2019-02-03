using System;
using System.Collections.Generic;
using System.Text;
using TelegramClientCore.BotDatabase;

namespace TelegramClientCore.BotServices
{
    class UserConfigService : IUserConfigService
    {
        static object syncObject = new object();

        private readonly UserConfig userConfig;
        private readonly BotDbContext dbContext;

        public bool FullTimeVisibility => userConfig.ShowFullTime;

        public bool TeachersVisibility => userConfig.ShowTeachers;

        public bool HolidaysVisibility => userConfig.ShowHolidays;

        public bool SiteUpdatesVisibility => throw new NotImplementedException();

        public bool BotUpdatesVisibility => throw new NotImplementedException();

        public UserConfigService( UserConfig userConfig, BotDbContext dbContext )
        {
            this.userConfig = userConfig;
            this.dbContext = dbContext;
        }

        public bool ChangeFullTimeVisibility()
        {
            userConfig.ShowFullTime ^= true;
            lock (syncObject) {
                dbContext.SaveChanges();
            }
            return userConfig.ShowFullTime;
        }

        public bool ChangeTeachersVisibility()
        {
            userConfig.ShowTeachers ^= true;
            lock (syncObject) {
                dbContext.SaveChanges();
            }
            return userConfig.ShowTeachers;
        }

        public bool ChangeHolidaysVisibility()
        {
            userConfig.ShowHolidays ^= true;
            lock (syncObject) {
                dbContext.SaveChanges();
            }
            return userConfig.ShowHolidays;
        }

        public bool ChangeSiteUpdatesVisibility()
        {
            throw new NotImplementedException();
        }

        public bool ChangeBotUpdatesVisibility()
        {
            throw new NotImplementedException();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramClientCore.BotServices
{
    public interface IUserConfigService
    {
        bool ChangeFullTimeVisibility();
        bool ChangeTeachersVisibility();
        bool ChangeHolidaysVisibility();

        bool ChangeSiteUpdatesVisibility();
        bool ChangeBotUpdatesVisibility();

        bool FullTimeVisibility { get; }
        bool TeachersVisibility { get; }
        bool HolidaysVisibility { get; }

        bool SiteUpdatesVisibility { get; }
        bool BotUpdatesVisibility { get; }
    }
}

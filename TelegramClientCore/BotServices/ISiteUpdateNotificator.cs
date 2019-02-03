using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramClientCore.BotServices
{
    interface ISiteUpdateNotificator
    {
        event EventHandler<EventArgs> OnSiteUpdate;
    }
}

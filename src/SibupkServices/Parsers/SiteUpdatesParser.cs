using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SibupkCommonServices.Parsers
{
    //todo: для сбора информации об ошибках пробросить интерфейс для логирования
    public class SiteUpdatesParser : IStringParser<DateTime>
    {
        const string updateDateGroupName = "updateDate";
        public DateTime Parse(string value)
        {
            //Последнее обновление: 24.05.2024
            Regex regex = new Regex(@$"Последнее обновление:\s*(?<{updateDateGroupName}>\d{{1,2}}\.\d{{1,2}}\.\d{{4}})");
            var match = regex.Match(value);
            if (match.Success) {
                try {
                    return DateTime.ParseExact(match.Groups[updateDateGroupName].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                } catch (Exception ex) {
                    return DateTime.Today;
                }
            }
            return DateTime.Today;
        }
    }
}

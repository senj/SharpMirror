using Newtonsoft.Json;
using SmartMirror.Data.Speech.DefaultEntities;
using System.Collections.Generic;
using System.Linq;

namespace SmartMirror.Intents
{
    public class CalendarDisplayDays
    {
        public CalendarDisplayDays(IDictionary<string, object> entities)
        {
            NumberOfDays = 5;
            string entity = JsonConvert.SerializeObject(entities.Where(p => p.Key == "datetimeV2"));
            if (entity?.Length < 3)
            {
                return;
            }

            entity = entity.Substring(1, entity.Length - 2);
            DateTimeV2 dateTimeV2 = JsonConvert.DeserializeObject<DateTimeV2>(entity);
            if (dateTimeV2.Value?.FirstOrDefault()?.type == "daterange")
            {
                // (2020-12-02,2020-12-12,P10D)
                string timex = dateTimeV2.Value.First().values.First().timex;
                if (timex?.Length >= 3)
                {
                    string period = timex.Split(',')[2];
                    period = period.Remove(period.Length - 1);

                    // P10D)
                    if (period?.StartsWith("P") == true)
                    {
                        if (period.Last() == 'D')
                        {
                            string timeOnly = period.Replace("P", "").Replace("D", "");
                            NumberOfDays = int.Parse(timeOnly);
                        }
                        else if (period[2] == 'W')
                        {
                            string timeOnly = period.Replace("P", "").Replace("D", "");
                            NumberOfDays = int.Parse(timeOnly) * 7;
                        }
                    }
                }
            }
        }

        public int NumberOfDays { get; }
    }
}

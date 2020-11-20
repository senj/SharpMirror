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
                string timex = dateTimeV2.Value.First().values.First().timex;
                if (timex?.Length >= 3)
                {
                    string period = timex.Split(',')[2];
                    if (period?.StartsWith("P") == true)
                    {
                        // TODO: does not work for numbers with more than 1 digit
                        if (period[2] == 'D')
                        {
                            NumberOfDays = int.Parse(period[1].ToString());
                        }
                        else if (period[2] == 'W')
                        {
                            NumberOfDays = int.Parse(period[1].ToString()) * 7;
                        }
                    }
                }
            }
        }

        public int NumberOfDays { get; }
    }
}

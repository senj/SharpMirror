using Newtonsoft.Json;
using SmartMirror.Data.Speech.DefaultEntities;
using SmartMirror.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SmartMirror.Intents
{
    public class ClockTimer
    {
        public ClockTimer(IDictionary<string, object> entities)
        {
            Name = "Timer";
            entities.TryGetValueAsStringArray("Clock.Remember", out string[] content);
            if (content?.Any() == true)
            {
                Name = content.First();
            }

            string entity = JsonConvert.SerializeObject(entities.Where(p => p.Key == "datetimeV2"));
            if (entity?.Length < 3)
            {
                return;
            }

            entity = entity.Substring(1, entity.Length - 2);
            DateTimeV2 dateTimeV2 = JsonConvert.DeserializeObject<DateTimeV2>(entity);
            if (dateTimeV2.Value?.FirstOrDefault()?.type == "datetime")
            {
                // 2020-11-20T11:48:44Z
                string timex = dateTimeV2.Value.First().values.First().timex;

                DateTime endTime = DateTime.ParseExact(timex, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.CurrentCulture.DateTimeFormat);
                DurationSeconds = endTime.Subtract(DateTime.Now).TotalSeconds;
            }
            else if (dateTimeV2.Value?.FirstOrDefault()?.type == "duration")
            {
                // PT2H
                string timex = dateTimeV2.Value.First().values.First().timex;
                if (timex?.StartsWith("PT") == true)
                {
                    // TODO: does not work for numbers with more than 1 digit
                    if (timex[3] == 'S')
                    {
                        DurationSeconds = int.Parse(timex[2].ToString());
                    }
                    else if (timex[3] == 'M')
                    {
                        DurationSeconds = int.Parse(timex[2].ToString()) * 60;
                    }
                    else if (timex[3] == 'H')
                    {
                        DurationSeconds = int.Parse(timex[2].ToString()) * 3600;
                    }
                }
            }
        }

        public double DurationSeconds { get; internal set; }
        public string Name { get; internal set; }
    }
}

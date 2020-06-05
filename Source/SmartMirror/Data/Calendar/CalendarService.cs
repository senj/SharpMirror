using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartMirror.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartMirror.Data.Calendar
{
    public class CalendarService
    {
        private readonly ILogger<CalendarService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _cache;
        private readonly CalendarConfiguration _calendarConfiguration;

        public CalendarService(ILogger<CalendarService> logger, HttpClient httpClient, IDistributedCache cache, IOptions<CalendarConfiguration> calendarConfiguration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _cache = cache;
            _calendarConfiguration = calendarConfiguration.Value;
        }

        public async Task<IEnumerable<Event>> GetCalendarAsync(int numberOfDays)
        {
            if (_cache.TryGetValue("calendar", out IEnumerable<Event> cachedEvents))
            {
                _logger.LogInformation("[CACHE] Got calendar from cache.");
                return cachedEvents;
            }

            try
            {
                var stream = await _httpClient.GetStreamAsync(_calendarConfiguration.CalendarUrl);
                var calendar = Ical.Net.Calendar.Load(stream);
                var events = calendar.Events.Where(p => p.DtStart.AsSystemLocal.Date >= DateTime.Now.Date).OrderBy(p => p.DtStart).Take(numberOfDays);

                IEnumerable<Event> mappedEvents = events.Select(p => new Event 
                {
                    DtStart = p.DtStart.AsSystemLocal,
                    DtEnd = p.DtEnd.AsSystemLocal,
                    IsAllDay = p.IsAllDay,
                    Summary = p.Summary
                });

                if (events != null)
                {
                    _cache.Set("calendar", mappedEvents, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    });
                }
                else
                {
                    _logger.LogError("Unable to get calendar, events is null.");
                }

                _logger.LogInformation("Got calendar.");
                return mappedEvents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading calendar");
                return null;
            }
            
        }
    }
}

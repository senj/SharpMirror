using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.Calendar
{
    public class CalendarState
    {
        private readonly CalendarService _calendarService;

        public CalendarState(CalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public event Action OnChange;

        public IEnumerable<Event> Events { get; private set; }

        public int NumberOfDays { get; private set; }
        
        public async Task<IEnumerable<Event>> GetEventsAsync(int numberOfDays)
        {
            bool useCache = true;
            if (NumberOfDays != numberOfDays)
            {
                useCache = false;
            }

            NumberOfDays = numberOfDays;
            Events = await _calendarService.GetCalendarAsync(numberOfDays, useCache: useCache);
            OnChange?.Invoke();

            return Events;
        }
    }
}

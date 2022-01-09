using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.Calendar
{
    public class CalendarState : StateBase
    {
        private readonly CalendarService _calendarService;

        public CalendarState(CalendarService calendarService) : base("Calendar", typeof(Shared.Calendar))
        {
            _calendarService = calendarService;
        }

        public IEnumerable<Event> Events { get; private set; }

        public int NumberOfDays { get; private set; }
        
        public async Task<IEnumerable<Event>> GetEventsAsync(int numberOfDays = 0)
        {
            if (numberOfDays == 0) numberOfDays = NumberOfDays;

            bool useCache = true;
            if (NumberOfDays != numberOfDays)
            {
                useCache = false;
            }

            NumberOfDays = numberOfDays;
            Events = await _calendarService.GetCalendarAsync(numberOfDays, useCache: useCache);
            RaiseOnChangeEvent();

            return Events;
        }

        public void SetNumberOfDays(int numberOfDays)
        {
            NumberOfDays = numberOfDays;
        }
    }
}

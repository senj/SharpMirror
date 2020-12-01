namespace SmartMirror.Intents
{
    public class CalendarShow
    {
        public CalendarShow(bool displayCalendar)
        {
            DisplayCalendar = displayCalendar;
        }

        public bool DisplayCalendar { get; }
    }
}

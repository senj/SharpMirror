namespace SmartMirror.Data.Speech.DefaultEntities
{
    public class DateTimeV2
    {
        public string Key { get; set; }
        public Value[] Value { get; set; }
    }

    public class Value
    {
        public string type { get; set; }
        public Value1[] values { get; set; }
    }

    public class Value1
    {
        public string timex { get; set; }
        public Resolution[] resolution { get; set; }
    }

    public class Resolution
    {
        public string start { get; set; }
        public string end { get; set; }
    }

}

using System.Text.Json.Serialization;

namespace SmartMirror.Data.Fitbit
{
    public class ActiveMinutes
    {
        [JsonPropertyName("activities-minutesSedentary")]
        public ActivitiesMinutes[] Sedentary { get; set; }


        [JsonPropertyName("activities-minutesLightlyActive")]
        public ActivitiesMinutes[] LightlyActive { get; set; }


        [JsonPropertyName("activities-minutesFairlyActive")]
        public ActivitiesMinutes[] FairlyActive { get; set; }

        [JsonPropertyName("activities-minutesVeryActive")]
        public ActivitiesMinutes[] VeryActive { get; set; }        
    }

    public class ActivitiesMinutes
    {
        public string dateTime { get; set; }
        public string value { get; set; }
    }
}
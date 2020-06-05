namespace SmartMirror.Data.Fitbit
{
    public class FitbitUserResponse
    {
        public User user { get; set; }
    }

    public class User
    {
        public int age { get; set; }
        public bool ambassador { get; set; }
        public bool autoStrideEnabled { get; set; }
        public string avatar { get; set; }
        public string avatar150 { get; set; }
        public string avatar640 { get; set; }
        public int averageDailySteps { get; set; }
        public string clockTimeDisplayFormat { get; set; }
        public bool corporate { get; set; }
        public bool corporateAdmin { get; set; }
        public string dateOfBirth { get; set; }
        public string displayName { get; set; }
        public string displayNameSetting { get; set; }
        public string distanceUnit { get; set; }
        public string encodedId { get; set; }
        public bool familyGuidanceEnabled { get; set; }
        public Features features { get; set; }
        public string foodsLocale { get; set; }
        public string fullName { get; set; }
        public string gender { get; set; }
        public string glucoseUnit { get; set; }
        public int height { get; set; }
        public string heightUnit { get; set; }
        public bool isChild { get; set; }
        public bool isCoach { get; set; }
        public string locale { get; set; }
        public string memberSince { get; set; }
        public bool mfaEnabled { get; set; }
        public int offsetFromUTCMillis { get; set; }
        public string startDayOfWeek { get; set; }
        public float strideLengthRunning { get; set; }
        public string strideLengthRunningType { get; set; }
        public int strideLengthWalking { get; set; }
        public string strideLengthWalkingType { get; set; }
        public string swimUnit { get; set; }
        public string timezone { get; set; }
        public Topbadge[] topBadges { get; set; }
        public string waterUnit { get; set; }
        public string waterUnitName { get; set; }
        public float weight { get; set; }
        public string weightUnit { get; set; }
    }

    public class Features
    {
        public bool exerciseGoal { get; set; }
    }

    public class Topbadge
    {
        public string badgeGradientEndColor { get; set; }
        public string badgeGradientStartColor { get; set; }
        public string badgeType { get; set; }
        public string category { get; set; }
        public object[] cheers { get; set; }
        public string dateTime { get; set; }
        public string description { get; set; }
        public string earnedMessage { get; set; }
        public string encodedId { get; set; }
        public string image100px { get; set; }
        public string image125px { get; set; }
        public string image300px { get; set; }
        public string image50px { get; set; }
        public string image75px { get; set; }
        public string marketingDescription { get; set; }
        public string mobileDescription { get; set; }
        public string name { get; set; }
        public string shareImage640px { get; set; }
        public string shareText { get; set; }
        public string shortDescription { get; set; }
        public string shortName { get; set; }
        public int timesAchieved { get; set; }
        public int value { get; set; }
        public string unit { get; set; }
    }
}
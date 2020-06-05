namespace SmartMirror.SmartHome.Hue
{
    public class LightState
    {
        public bool on { get; set; }

        /// <summary>
        /// 0 - 254
        /// </summary>
        public int sat { get; set; }

        /// <summary>
        /// 0 - 254
        /// </summary>
        public int bri { get; set; }

        /// <summary>
        ///  0 - 65535
        /// </summary>
        public int hue { get; set; }
    }
}
namespace SmartMirror.Intents.Show
{
    public class BaseDisplayWidget
    {
        public BaseDisplayWidget(bool display)
        {
            Display = display;
        }

        public bool Display { get; }
    }
}

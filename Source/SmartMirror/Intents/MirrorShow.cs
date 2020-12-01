namespace SmartMirror.Intents
{
    public class MirrorShow
    {
        public MirrorShow(bool hideEverything)
        {
            ShowWidgets = !hideEverything;
        }

        public bool ShowWidgets { get; }
    }
}

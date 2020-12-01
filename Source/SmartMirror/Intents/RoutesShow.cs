namespace SmartMirror.Intents
{
    public class RoutesShow
    {
        public RoutesShow(bool displayRoutes)
        {
            DisplayRoutes = displayRoutes;
        }

        public bool DisplayRoutes { get; }
    }
}

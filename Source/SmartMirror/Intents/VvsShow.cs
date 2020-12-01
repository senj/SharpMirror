namespace SmartMirror.Intents
{
    public class VvsShow
    {
        public VvsShow(bool displayVvs)
        {
            DisplayVvs = displayVvs;
        }

        public bool DisplayVvs { get; }
    }
}

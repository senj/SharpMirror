namespace SmartMirror.Intents
{
    public class BringShow
    {
        public BringShow(bool displayBring)
        {
            DisplayBring = displayBring;
        }

        public bool DisplayBring { get; }
    }
}

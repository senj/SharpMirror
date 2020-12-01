namespace SmartMirror.Intents
{
    public class FuelShow
    {
        public FuelShow(bool displayFuel)
        {
            DisplayFuel = displayFuel;
        }

        public bool DisplayFuel { get; }
    }
}

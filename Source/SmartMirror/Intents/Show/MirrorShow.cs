using SmartMirror.Intents.Show;

namespace SmartMirror.Intents
{
    public class MirrorShow : BaseDisplayWidget
    {
        public MirrorShow(bool display) : base(!display)
        {
        }
    }
}

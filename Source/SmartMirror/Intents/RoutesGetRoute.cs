using SmartMirror.Extensions;
using System.Collections.Generic;

namespace SmartMirror.Intents
{
    public class RoutesGetRoute
    {
        public RoutesGetRoute(IDictionary<string, object> entities)
        {
            entities.TryGetValueAsStringArray("Places.Start", out string[] sourceArray);
            entities.TryGetValueAsStringArray("Places.Destination", out string[] destinationArray);

            Source = sourceArray?[0];
            Destination = destinationArray?[0];
        }

        public string Source { get; }

        public string Destination { get; }
    }
}
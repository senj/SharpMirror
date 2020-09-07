using MediatR;
using SmartMirror.Data.Routes;
using SmartMirror.Extensions;
using System.Collections.Generic;

namespace SmartMirror.Notifications
{
    public class GetDistanceRequest : IRequest<RouteResponse>
    {
        private IDictionary<string, object> _entities;

        public GetDistanceRequest(IDictionary<string, object> entities)
        {
            entities.TryGetValueAsStringArray("Places.Start", out string[] sourceArray);
            entities.TryGetValueAsStringArray("Places.AbsoluteLocation", out string[] destinationArray);

            Source = sourceArray?[0];
            Destination = destinationArray?[0];
        }

        public string Source { get; internal set; }

        public string Destination { get; internal set; }
    }
}
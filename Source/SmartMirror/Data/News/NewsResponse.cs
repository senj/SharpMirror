using Microsoft.SyndicationFeed;
using System;
using System.Collections.Generic;

namespace SmartMirror.Data.News
{
    public class NewsResponse
    {
        public string Id { get; internal set; }
        public string Title { get; internal set; }
        public DateTimeOffset Published { get; internal set; }
        public IEnumerable<ISyndicationLink> Links { get; internal set; }
        public IEnumerable<ISyndicationCategory> Categories { get; internal set; }
        public string Description { get; internal set; }
    }
}
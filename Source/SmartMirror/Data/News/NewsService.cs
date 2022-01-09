using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SmartMirror.Data.News
{
    public class NewsService
    {
        private readonly ILogger<NewsService> _logger;
        private NewsConfiguration _newsConfiguration;

        public NewsService(ILogger<NewsService> logger, IOptions<NewsConfiguration> newsConfiguration)
        {
            _newsConfiguration = newsConfiguration.Value;
            _logger = logger;
        }

        public async Task<List<NewsResponse>> GetFeeds()
        {
            List<NewsResponse> news = new();
            foreach (string feedUrl in _newsConfiguration.Feeds)
            {
                news.AddRange(await GetNews(feedUrl));
            }

            return news;
        }

        private async Task<List<NewsResponse>> GetNews(string feedUrl)
        {
            List<NewsResponse> news = new();
            Uri feedUri = new(feedUrl);

            using (XmlReader xmlReader = XmlReader.Create(feedUri.ToString(),
                   new XmlReaderSettings { Async = true }))
            {
                try
                {
                    RssFeedReader feedReader = new(xmlReader);

                    while (await feedReader.Read())
                    {
                        switch (feedReader.ElementType)
                        {
                            // RSS Item
                            case SyndicationElementType.Item:
                                ISyndicationItem item = await feedReader.ReadItem();
                                news.Add(new NewsResponse 
                                {
                                    Id = item.Id,
                                    Description = item.Description,
                                    Title = item.Title,
                                    Published = item.Published,
                                    Links = item.Links,
                                    Categories = item.Categories
                                });
                                break;

                            // Something else
                            default:
                                _logger.LogWarning("Encountered non-item element while reading news");
                                break;
                        }
                    }
                }
                catch (AggregateException ae)
                {
                    _logger.LogError(ae, "Error reading news");
                }
            }

            return news.OrderByDescending(story => story.Published).ToList();
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMirror.Data.News
{
    public class NewsState : StateBase
    {
        private readonly NewsService _newsService;

        public NewsState(NewsService newsService) : base("News", typeof(Shared.News))
        {
            _newsService = newsService;
        }

        public List<NewsResponse> News { get; private set; }

        public async Task<List<NewsResponse>> GetNewsAsync()
        {
            News = await _newsService.GetFeeds();
            RaiseOnChangeEvent();

            return News;
        }
    }
}
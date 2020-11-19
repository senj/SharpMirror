using System;
using System.Threading.Tasks;

namespace SmartMirror.Data.Bring
{
    public class BringState
    {
        private readonly BringService _bringService;

        public BringState(BringService bringService)
        {
            _bringService = bringService;
        }

        public event Action OnChange; 
        
        public BringItemResponse Items { get; private set; }

        public bool ShowDetails { get; private set; }

        public async Task<BringItemResponse> GetItemsAsync(bool loadFromCache)
        {
            BringItemResponse items = await _bringService.GetItemsAsync(loadFromCache);
            Items = items;
            OnChange?.Invoke();

            return items;
        }

        public async Task AddItemAsync(string name, string detail)
        {
            await _bringService.AddItemAsync(name, detail);

            // This will refresh items property and trigger onChange
            await GetItemsAsync(false);
        }

        public async Task RemoveItemAsync(string entry)
        {
            await _bringService.RemoveItemAsync(entry);

            // This will refresh items property and trigger onChange
            await GetItemsAsync(false);
        }

        public void SetShowDetails(bool showDetails)
        {
            ShowDetails = showDetails;
            OnChange?.Invoke();
        }
    }
}

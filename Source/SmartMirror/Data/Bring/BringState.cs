using System.Threading.Tasks;

namespace SmartMirror.Data.Bring
{
    public class BringState : StateBase
    {
        private readonly BringService _bringService;

        public BringState(BringService bringService) : base("Bring", typeof(Shared.Bring))
        {
            _bringService = bringService;
        }

        public BringItemResponse Items { get; private set; }

        public async Task<BringItemResponse> GetItemsAsync(bool loadFromCache)
        {
            Items = await _bringService.GetItemsAsync(loadFromCache);
            RaiseOnChangeEvent();

            return Items;
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
    }
}

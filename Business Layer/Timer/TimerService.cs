using Business_Layer.Interfaces;
using Business_Layer.SearchTries;
using TimeCounter = System.Threading.Timer;

namespace Business_Layer.Timer;

public class TimerService : IDisposable
{
    private readonly TimeCounter refreshProductsTrie;

    public IProductsBusiness ProductsBusiness { get; }
    public ProductTrie Trie { get; }

    public TimerService(IProductsBusiness productsBusiness, ProductTrie trie)
    {
        refreshProductsTrie = new TimeCounter(SetTrieWords, null, TimeSpan.Zero, TimeSpan.FromDays(1));
        ProductsBusiness = productsBusiness;
        Trie = trie;
    }

    private void SetTrieWords(object state) => SetTrie();
    private async Task SetTrie()
    {
        Trie.AddList(await ProductsBusiness.GetProductNames());
    }


    public void Dispose()
    {
        refreshProductsTrie?.Dispose();
    }
}

using ComparadorDePrecos.Models;
using ComparadorDePrecos.Providers;
using Microsoft.Extensions.Caching.Memory;

namespace ComparadorDePrecos.Services;

public class ComparadorService
{
    private readonly IEnumerable<IMarketProvider> _providers;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30); // Cache de 30 minutos

    public ComparadorService(IEnumerable<IMarketProvider> providers, IMemoryCache cache)
    {
        _providers = providers;
        _cache = cache;
    }

    public async Task<List<Produto>> BuscarProdutosAsync(string termo)
    {
        var cacheKey = $"busca_geral_{termo.ToLower()}";

        if (!_cache.TryGetValue(cacheKey, out List<Produto>? produtos))
        {
            var tasks = _providers.Select(provider => provider.BuscarProdutosAsync(termo));
            var resultados = await Task.WhenAll(tasks);

            produtos = resultados.SelectMany(produtos => produtos).ToList();

            _cache.Set(cacheKey, produtos, _cacheDuration);
        }

        return produtos ?? new List<Produto>(); ;
    }
    public async Task<List<Produto>> BuscarProdutosPorMercadoAsync(string termo, string mercado)
    {
        var cacheKey = $"busca_{mercado}_{termo.ToLower()}";
        List<Produto>? products;
        //if (!_cache.TryGetValue(cacheKey, out List<Produto>? products))
        {
            var provider = _providers.FirstOrDefault(p =>
                p.Nome.Equals(mercado, StringComparison.OrdinalIgnoreCase));

            if (provider is null)
                return new List<Produto>();

            products = await provider.BuscarProdutosAsync(termo);
            _cache.Set(cacheKey, products, _cacheDuration);
        }

        return products ?? new List<Produto>();
    }
    
    public List<string> ObterMercadosDisponiveis()
    {
        return _providers.Select(p => p.Nome).ToList();
    }
}
using ComparadorDePrecos.Models;

namespace ComparadorDePrecos.Providers;

public interface IMarketProvider
{
    string Nome { get; }
    Task<List<Produto>> BuscarProdutosAsync(string termo);
}
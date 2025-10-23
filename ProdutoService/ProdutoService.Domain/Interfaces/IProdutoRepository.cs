using ProdutoService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProdutoService.Domain.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IEnumerable<Produto>> GetByNomeAsync(string nome);
    Task<IEnumerable<Produto>> GetByMercadoAsync(string mercado);
}
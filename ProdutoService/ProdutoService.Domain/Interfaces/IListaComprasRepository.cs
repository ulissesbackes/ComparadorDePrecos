using ProdutoService.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProdutoService.Domain.Interfaces;

public interface IListaComprasRepository : IRepository<ListaCompras>
{
    Task<IEnumerable<ListaCompras>> GetByUsuarioAsync(int usuarioId);
    Task<ListaCompras?> GetWithItensAsync(int id);
    Task AddItemToListAsync(int listaId, int produtoId, int quantidade = 1);
    Task RemoveItemFromListAsync(int listaId, int produtoId);
    Task UpdateItemQuantityAsync(int listaId, int produtoId, int quantidade);
}
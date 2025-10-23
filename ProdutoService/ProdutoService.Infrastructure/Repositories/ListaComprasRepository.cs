using Microsoft.EntityFrameworkCore;
using ProdutoService.Domain.Interfaces;
using ProdutoService.Domain.Models;
using ProdutoService.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdutoService.Infrastructure.Repositories;

public class ListaComprasRepository : BaseRepository<ListaCompras>, IListaComprasRepository
{
    public ListaComprasRepository(ProdutoContext context) : base(context) { }

    public async Task<IEnumerable<ListaCompras>> GetByUsuarioAsync(int usuarioId)
    {
        return await _dbSet
            .Where(l => l.UsuarioId == usuarioId)
            .Include(l => l.Itens)
                .ThenInclude(li => li.Produto)
            .ToListAsync();
    }

    public async Task<ListaCompras?> GetWithItensAsync(int id)
    {
        return await _dbSet
            .Include(l => l.Itens)
                .ThenInclude(li => li.Produto)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task AddItemToListAsync(int listaId, int produtoId, int quantidade = 1)
    {
        var listaItem = new ListaItem
        {
            ListaId = listaId,
            ProdutoId = produtoId,
            Quantidade = quantidade,
            AdicionadoEm = System.DateTime.UtcNow
        };

        _context.ListaItens.Add(listaItem);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveItemFromListAsync(int listaId, int produtoId)
    {
        var listaItem = await _context.ListaItens
            .FirstOrDefaultAsync(li => li.ListaId == listaId && li.ProdutoId == produtoId);

        if (listaItem != null)
        {
            _context.ListaItens.Remove(listaItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateItemQuantityAsync(int listaId, int produtoId, int quantidade)
    {
        var listaItem = await _context.ListaItens
            .FirstOrDefaultAsync(li => li.ListaId == listaId && li.ProdutoId == produtoId);

        if (listaItem != null)
        {
            listaItem.Quantidade = quantidade;
            await _context.SaveChangesAsync();
        }
    }
}
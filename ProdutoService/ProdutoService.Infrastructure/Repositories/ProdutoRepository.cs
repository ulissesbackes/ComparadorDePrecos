using Microsoft.EntityFrameworkCore;
using ProdutoService.Domain.Interfaces;
using ProdutoService.Domain.Models;
using ProdutoService.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdutoService.Infrastructure.Repositories;

public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
{
    public ProdutoRepository(ProdutoContext context) : base(context) { }

    public async Task<IEnumerable<Produto>> GetByNomeAsync(string nome)
    {
        return await _dbSet
            .Where(p => p.Nome.ToLower().Contains(nome.ToLower()))
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> GetByMercadoAsync(string mercado)
    {
        return await _dbSet
            .Where(p => p.Mercado.ToLower() == mercado.ToLower())
            .ToListAsync();
    }
}
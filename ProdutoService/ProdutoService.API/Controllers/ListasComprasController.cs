using Microsoft.AspNetCore.Mvc;
using ProdutoService.Domain.Interfaces;
using ProdutoService.API.DTOs;
using ProdutoService.Domain.Models;

namespace ProdutoService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ListasComprasController : ControllerBase
{
    private readonly IListaComprasRepository _listaComprasRepository;
    private readonly IProdutoRepository _produtoRepository;

    public ListasComprasController(IListaComprasRepository listaComprasRepository, IProdutoRepository produtoRepository)
    {
        _listaComprasRepository = listaComprasRepository;
        _produtoRepository = produtoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ListaComprasDto>>> GetListas([FromQuery] int usuarioId)
    {
        var listas = await _listaComprasRepository.GetByUsuarioAsync(usuarioId);
        var listasDto = listas.Select(MapToDto);
        return Ok(listasDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ListaComprasDto>> GetLista(int id)
    {
        var lista = await _listaComprasRepository.GetWithItensAsync(id);
        if (lista == null) return NotFound();
        return Ok(MapToDto(lista));
    }

    [HttpPost]
    public async Task<ActionResult<ListaComprasDto>> CriarLista(CriarListaComprasDto dto)
    {
        var lista = new ListaCompras
        {
            Nome = dto.Nome,
            UsuarioId = dto.UsuarioId,
            CriadaEm = DateTime.UtcNow
        };

        var listaCriada = await _listaComprasRepository.AddAsync(lista);
        return CreatedAtAction(nameof(GetLista), new { id = listaCriada.Id }, MapToDto(listaCriada));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarLista(int id, [FromBody] AtualizarListaComprasDto dto)
    {
        var lista = await _listaComprasRepository.GetByIdAsync(id);
        if (lista == null) return NotFound();

        lista.Nome = dto.Nome;
        await _listaComprasRepository.UpdateAsync(lista);

        return NoContent();
    }

    [HttpPost("{listaId}/item")]
    public async Task<IActionResult> AdicionarItem(int listaId, AdicionarItemListaDto dto)
    {
        // Verificar se o produto existe
        var produto = await _produtoRepository.GetByIdAsync(dto.ProdutoId);
        if (produto == null) return NotFound("Produto não encontrado");

        // Verificar se a lista existe
        var lista = await _listaComprasRepository.GetByIdAsync(listaId);
        if (lista == null) return NotFound("Lista não encontrada");

        await _listaComprasRepository.AddItemToListAsync(listaId, dto.ProdutoId, dto.Quantidade);
        return NoContent();
    }

    [HttpPut("{listaId}/item/{produtoId}")]
    public async Task<IActionResult> AtualizarQuantidadeItem(int listaId, int produtoId, [FromBody] int quantidade)
    {
        if (quantidade <= 0) return BadRequest("Quantidade deve ser maior que zero");

        await _listaComprasRepository.UpdateItemQuantityAsync(listaId, produtoId, quantidade);
        return NoContent();
    }

    [HttpDelete("{listaId}/item/{produtoId}")]
    public async Task<IActionResult> RemoverItem(int listaId, int produtoId)
    {
        await _listaComprasRepository.RemoveItemFromListAsync(listaId, produtoId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarLista(int id)
    {
        var lista = await _listaComprasRepository.GetByIdAsync(id);
        if (lista == null) return NotFound();

        await _listaComprasRepository.DeleteAsync(lista);
        return NoContent();
    }

    private static ListaComprasDto MapToDto(ListaCompras lista) => new()
    {
        Id = lista.Id,
        Nome = lista.Nome,
        UsuarioId = lista.UsuarioId,
        CriadaEm = lista.CriadaEm,
        Itens = lista.Itens.Select(li => new ListaItemDto
        {
            ProdutoId = li.ProdutoId,
            ProdutoNome = li.Produto.Nome,
            ProdutoPreco = li.Produto.PrecoAtual,
            Quantidade = li.Quantidade,
            AdicionadoEm = li.AdicionadoEm
        }).ToList()
    };
}
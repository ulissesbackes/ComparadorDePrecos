using Microsoft.AspNetCore.Mvc;
using ProdutoService.Domain.Interfaces;
using ProdutoService.API.DTOs;
using ProdutoService.Domain.Models;

namespace ProdutoService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutosController(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetProdutos()
    {
        var produtos = await _produtoRepository.GetAllAsync();
        var produtosDto = produtos.Select(p => MapToDto(p));
        return Ok(produtosDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoDto>> GetProduto(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null) return NotFound();
        return Ok(MapToDto(produto));
    }

    [HttpGet("buscar/{nome}")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> BuscarPorNome(string nome)
    {
        var produtos = await _produtoRepository.GetByNomeAsync(nome);
        var produtosDto = produtos.Select(p => MapToDto(p));
        return Ok(produtosDto);
    }

    [HttpGet("mercado/{mercado}")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetPorMercado(string mercado)
    {
        var produtos = await _produtoRepository.GetByMercadoAsync(mercado);
        var produtosDto = produtos.Select(p => MapToDto(p));
        return Ok(produtosDto);
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoDto>> CriarProduto(CriarProdutoDto dto)
    {
        var produto = new Produto
        {
            Nome = dto.Nome,
            Marca = dto.Marca,
            PrecoAtual = dto.PrecoAtual,
            Mercado = dto.Mercado,
            Url = dto.Url,
            UrlImagem = dto.UrlImagem,
            CriadoEm = DateTime.UtcNow
        };

        var produtoCriado = await _produtoRepository.AddAsync(produto);
        return CreatedAtAction(nameof(GetProduto), new { id = produtoCriado.Id }, MapToDto(produtoCriado));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarProduto(int id, CriarProdutoDto dto)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null) return NotFound();

        produto.Nome = dto.Nome;
        produto.Marca = dto.Marca;
        produto.PrecoAtual = dto.PrecoAtual;
        produto.Mercado = dto.Mercado;
        produto.Url = dto.Url;
        produto.UrlImagem = dto.UrlImagem;

        await _produtoRepository.UpdateAsync(produto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarProduto(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null) return NotFound();

        await _produtoRepository.DeleteAsync(produto);
        return NoContent();
    }

    private static ProdutoDto MapToDto(Produto produto) => new()
    {
        Id = produto.Id,
        Nome = produto.Nome,
        Marca = produto.Marca,
        PrecoAtual = produto.PrecoAtual,
        Mercado = produto.Mercado,
        Url = produto.Url,
        UrlImagem = produto.UrlImagem,
        CriadoEm = produto.CriadoEm
    };
}
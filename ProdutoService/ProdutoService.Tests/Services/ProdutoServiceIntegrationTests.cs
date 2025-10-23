using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProdutoService.Domain.DTOs;
using ProdutoService.Domain.Models;
using ProdutoService.Domain.Services;
using ProdutoService.Infrastructure.Data;
using ProdutoService.Infrastructure.Repositories;
using Xunit;

namespace ProdutoService.Tests.Services;

public class ProdutoServiceIntegrationTests : IDisposable
{
    private readonly ProdutoContext _context;
    private readonly Domain.Services.ProdutoService _produtoService;

    public ProdutoServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ProdutoContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ProdutoContext(options);
        var repository = new ProdutoRepository(_context);
        _produtoService = new Domain.Services.ProdutoService(repository);
    }

    [Fact]
    public async Task CriarProdutoAsync_ShouldPersistToDatabase()
    {
        // Arrange
        var dto = new CriarProdutoDto
        {
            Nome = "Produto Teste",
            Marca = "Marca Teste",
            PrecoAtual = 25.99m,
            Mercado = "Mercado Teste",
            Url = "https://example.com/produto",
            UrlImagem = "https://example.com/imagem.jpg"
        };

        // Act
        var result = await _produtoService.CriarProdutoAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be(dto.Nome);
        result.Marca.Should().Be(dto.Marca);
        result.PrecoAtual.Should().Be(dto.PrecoAtual);
        result.Mercado.Should().Be(dto.Mercado);
        result.Url.Should().Be(dto.Url);
        result.UrlImagem.Should().Be(dto.UrlImagem);
        result.CriadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // Verify persistence
        var produtoFromDb = await _context.Produtos.FindAsync(result.Id);
        produtoFromDb.Should().NotBeNull();
        produtoFromDb!.Nome.Should().Be(dto.Nome);
    }

    [Fact]
    public async Task AtualizarProdutoAsync_ShouldUpdateInDatabase()
    {
        // Arrange
        var produto = new Produto
        {
            Nome = "Produto Original",
            Marca = "Marca Original",
            PrecoAtual = 10.00m,
            Mercado = "Mercado Original",
            Url = "https://example.com/original",
            UrlImagem = "https://example.com/original.jpg",
            CriadoEm = DateTime.UtcNow
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        var dto = new CriarProdutoDto
        {
            Nome = "Produto Atualizado",
            Marca = "Marca Atualizada",
            PrecoAtual = 20.00m,
            Mercado = "Mercado Atualizado",
            Url = "https://example.com/atualizado",
            UrlImagem = "https://example.com/atualizado.jpg"
        };

        // Act
        var result = await _produtoService.AtualizarProdutoAsync(produto.Id, dto);

        // Assert
        result.Should().BeTrue();

        // Verify update in database
        var produtoAtualizado = await _context.Produtos.FindAsync(produto.Id);
        produtoAtualizado.Should().NotBeNull();
        produtoAtualizado!.Nome.Should().Be(dto.Nome);
        produtoAtualizado.Marca.Should().Be(dto.Marca);
        produtoAtualizado.PrecoAtual.Should().Be(dto.PrecoAtual);
        produtoAtualizado.Mercado.Should().Be(dto.Mercado);
        produtoAtualizado.Url.Should().Be(dto.Url);
        produtoAtualizado.UrlImagem.Should().Be(dto.UrlImagem);
    }

    [Fact]
    public async Task DeletarProdutoAsync_ShouldRemoveFromDatabase()
    {
        // Arrange
        var produto = new Produto
        {
            Nome = "Produto para Deletar",
            Marca = "Marca",
            PrecoAtual = 15.00m,
            Mercado = "Mercado",
            Url = "https://example.com/deletar",
            UrlImagem = "https://example.com/deletar.jpg",
            CriadoEm = DateTime.UtcNow
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        // Act
        var result = await _produtoService.DeletarProdutoAsync(produto.Id);

        // Assert
        result.Should().BeTrue();

        // Verify deletion from database
        var produtoDeletado = await _context.Produtos.FindAsync(produto.Id);
        produtoDeletado.Should().BeNull();
    }

    [Fact]
    public async Task BuscarProdutosPorNomeAsync_ShouldReturnCaseInsensitiveResults()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            new() { Nome = "Café Expresso", Marca = "Marca A", PrecoAtual = 12.50m, Mercado = "Mercado 1", Url = "url1", CriadoEm = DateTime.UtcNow },
            new() { Nome = "Café Tradicional", Marca = "Marca B", PrecoAtual = 8.75m, Mercado = "Mercado 2", Url = "url2", CriadoEm = DateTime.UtcNow },
            new() { Nome = "Chá Verde", Marca = "Marca C", PrecoAtual = 6.25m, Mercado = "Mercado 3", Url = "url3", CriadoEm = DateTime.UtcNow }
        };

        _context.Produtos.AddRange(produtos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _produtoService.BuscarProdutosPorNomeAsync("café");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Nome.Should().Contain("Café"));
    }

    [Fact]
    public async Task GetProdutosPorMercadoAsync_ShouldReturnCaseInsensitiveResults()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            new() { Nome = "Produto 1", Marca = "Marca A", PrecoAtual = 10.00m, Mercado = "Supermercado ABC", Url = "url1", CriadoEm = DateTime.UtcNow },
            new() { Nome = "Produto 2", Marca = "Marca B", PrecoAtual = 15.00m, Mercado = "Supermercado ABC", Url = "url2", CriadoEm = DateTime.UtcNow },
            new() { Nome = "Produto 3", Marca = "Marca C", PrecoAtual = 20.00m, Mercado = "Mercado XYZ", Url = "url3", CriadoEm = DateTime.UtcNow }
        };

        _context.Produtos.AddRange(produtos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _produtoService.GetProdutosPorMercadoAsync("supermercado abc");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Mercado.Should().Be("Supermercado ABC"));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

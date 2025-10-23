using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProdutoService.API.Controllers;
using ProdutoService.Domain.DTOs;
using ProdutoService.Domain.Services;
using Xunit;

namespace ProdutoService.Tests.Controllers;

public class ProdutosControllerTests
{
    private readonly Mock<Domain.Services.ProdutoService> _mockService;
    private readonly ProdutoService.API.Controllers.ProdutosController _controller;

    public ProdutosControllerTests()
    {
        _mockService = new Mock<Domain.Services.ProdutoService>(Mock.Of<Domain.Interfaces.IProdutoRepository>());
        _controller = new ProdutoService.API.Controllers.ProdutosController(_mockService.Object);
    }

    [Fact]
    public async Task GetProdutos_ShouldReturnOkWithProdutos()
    {
        // Arrange
        var produtos = new List<ProdutoDto>
        {
            new() { Id = 1, Nome = "Produto 1", Marca = "Marca A", PrecoAtual = 10.50m, Mercado = "Mercado 1", Url = "url1", CriadoEm = DateTime.UtcNow },
            new() { Id = 2, Nome = "Produto 2", Marca = "Marca B", PrecoAtual = 15.75m, Mercado = "Mercado 2", Url = "url2", CriadoEm = DateTime.UtcNow }
        };

        _mockService.Setup(x => x.GetAllProdutosAsync()).ReturnsAsync(produtos);

        // Act
        var result = await _controller.GetProdutos();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(produtos);
        _mockService.Verify(x => x.GetAllProdutosAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProduto_WhenProdutoExists_ShouldReturnOkWithProduto()
    {
        // Arrange
        var produto = new ProdutoDto
        {
            Id = 1,
            Nome = "Produto Teste",
            Marca = "Marca Teste",
            PrecoAtual = 25.99m,
            Mercado = "Mercado Teste",
            Url = "https://example.com/produto",
            UrlImagem = "https://example.com/imagem.jpg",
            CriadoEm = DateTime.UtcNow
        };

        _mockService.Setup(x => x.GetProdutoByIdAsync(1)).ReturnsAsync(produto);

        // Act
        var result = await _controller.GetProduto(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(produto);
        _mockService.Verify(x => x.GetProdutoByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetProduto_WhenProdutoDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(x => x.GetProdutoByIdAsync(999)).ReturnsAsync((ProdutoDto?)null);

        // Act
        var result = await _controller.GetProduto(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _mockService.Verify(x => x.GetProdutoByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task BuscarPorNome_ShouldReturnOkWithMatchingProdutos()
    {
        // Arrange
        var produtos = new List<ProdutoDto>
        {
            new() { Id = 1, Nome = "Café Expresso", Marca = "Marca A", PrecoAtual = 12.50m, Mercado = "Mercado 1", Url = "url1", CriadoEm = DateTime.UtcNow },
            new() { Id = 2, Nome = "Café Tradicional", Marca = "Marca B", PrecoAtual = 8.75m, Mercado = "Mercado 2", Url = "url2", CriadoEm = DateTime.UtcNow }
        };

        _mockService.Setup(x => x.BuscarProdutosPorNomeAsync("café")).ReturnsAsync(produtos);

        // Act
        var result = await _controller.BuscarPorNome("café");

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(produtos);
        _mockService.Verify(x => x.BuscarProdutosPorNomeAsync("café"), Times.Once);
    }

    [Fact]
    public async Task GetPorMercado_ShouldReturnOkWithMatchingProdutos()
    {
        // Arrange
        var produtos = new List<ProdutoDto>
        {
            new() { Id = 1, Nome = "Produto 1", Marca = "Marca A", PrecoAtual = 10.00m, Mercado = "Supermercado ABC", Url = "url1", CriadoEm = DateTime.UtcNow },
            new() { Id = 2, Nome = "Produto 2", Marca = "Marca B", PrecoAtual = 15.00m, Mercado = "Supermercado ABC", Url = "url2", CriadoEm = DateTime.UtcNow }
        };

        _mockService.Setup(x => x.GetProdutosPorMercadoAsync("Supermercado ABC")).ReturnsAsync(produtos);

        // Act
        var result = await _controller.GetPorMercado("Supermercado ABC");

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(produtos);
        _mockService.Verify(x => x.GetProdutosPorMercadoAsync("Supermercado ABC"), Times.Once);
    }

    [Fact]
    public async Task CriarProduto_ShouldReturnCreatedAtActionWithProduto()
    {
        // Arrange
        var dto = new CriarProdutoDto
        {
            Nome = "Novo Produto",
            Marca = "Nova Marca",
            PrecoAtual = 29.99m,
            Mercado = "Novo Mercado",
            Url = "https://example.com/produto",
            UrlImagem = "https://example.com/imagem.jpg"
        };

        var produtoCriado = new ProdutoDto
        {
            Id = 1,
            Nome = dto.Nome,
            Marca = dto.Marca,
            PrecoAtual = dto.PrecoAtual,
            Mercado = dto.Mercado,
            Url = dto.Url,
            UrlImagem = dto.UrlImagem,
            CriadoEm = DateTime.UtcNow
        };

        _mockService.Setup(x => x.CriarProdutoAsync(dto)).ReturnsAsync(produtoCriado);

        // Act
        var result = await _controller.CriarProduto(dto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(produtoCriado);
        createdResult.ActionName.Should().Be(nameof(_controller.GetProduto));
        createdResult.RouteValues!["id"].Should().Be(1);
        _mockService.Verify(x => x.CriarProdutoAsync(dto), Times.Once);
    }

    [Fact]
    public async Task AtualizarProduto_WhenProdutoExists_ShouldReturnNoContent()
    {
        // Arrange
        var dto = new CriarProdutoDto
        {
            Nome = "Produto Atualizado",
            Marca = "Marca Atualizada",
            PrecoAtual = 20.00m,
            Mercado = "Mercado Atualizado",
            Url = "https://example.com/atualizado",
            UrlImagem = "https://example.com/imagem-atualizada.jpg"
        };

        _mockService.Setup(x => x.AtualizarProdutoAsync(1, dto)).ReturnsAsync(true);

        // Act
        var result = await _controller.AtualizarProduto(1, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(x => x.AtualizarProdutoAsync(1, dto), Times.Once);
    }

    [Fact]
    public async Task AtualizarProduto_WhenProdutoDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var dto = new CriarProdutoDto
        {
            Nome = "Produto Atualizado",
            Marca = "Marca Atualizada",
            PrecoAtual = 20.00m,
            Mercado = "Mercado Atualizado",
            Url = "https://example.com/atualizado",
            UrlImagem = "https://example.com/imagem-atualizada.jpg"
        };

        _mockService.Setup(x => x.AtualizarProdutoAsync(999, dto)).ReturnsAsync(false);

        // Act
        var result = await _controller.AtualizarProduto(999, dto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockService.Verify(x => x.AtualizarProdutoAsync(999, dto), Times.Once);
    }

    [Fact]
    public async Task DeletarProduto_WhenProdutoExists_ShouldReturnNoContent()
    {
        // Arrange
        _mockService.Setup(x => x.DeletarProdutoAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeletarProduto(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(x => x.DeletarProdutoAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeletarProduto_WhenProdutoDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(x => x.DeletarProdutoAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeletarProduto(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockService.Verify(x => x.DeletarProdutoAsync(999), Times.Once);
    }
}

using FluentAssertions;
using Moq;
using ProdutoService.Domain.DTOs;
using ProdutoService.Domain.Interfaces;
using ProdutoService.Domain.Models;
using ProdutoService.Domain.Services;
using Xunit;

namespace ProdutoService.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _mockRepository;
    private readonly Domain.Services.ProdutoService _produtoService;

    public ProdutoServiceTests()
    {
        _mockRepository = new Mock<IProdutoRepository>();
        _produtoService = new Domain.Services.ProdutoService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllProdutosAsync_ShouldReturnAllProdutos()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            CreateProduto(1, "Produto 1", "Marca A", 10.50m, "Mercado 1"),
            CreateProduto(2, "Produto 2", "Marca B", 15.75m, "Mercado 2")
        };

        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(produtos);

        // Act
        var result = await _produtoService.GetAllProdutosAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Nome == "Produto 1" && p.Marca == "Marca A");
        result.Should().Contain(p => p.Nome == "Produto 2" && p.Marca == "Marca B");
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllProdutosAsync_WhenNoProdutos_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Produto>());

        // Act
        var result = await _produtoService.GetAllProdutosAsync();

        // Assert
        result.Should().BeEmpty();
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProdutoByIdAsync_WhenProdutoExists_ShouldReturnProduto()
    {
        // Arrange
        var produto = CreateProduto(1, "Produto Teste", "Marca Teste", 25.99m, "Mercado Teste");
        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(produto);

        // Act
        var result = await _produtoService.GetProdutoByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Nome.Should().Be("Produto Teste");
        result.Marca.Should().Be("Marca Teste");
        result.PrecoAtual.Should().Be(25.99m);
        result.Mercado.Should().Be("Mercado Teste");
        _mockRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetProdutoByIdAsync_WhenProdutoDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Produto?)null);

        // Act
        var result = await _produtoService.GetProdutoByIdAsync(999);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task BuscarProdutosPorNomeAsync_WhenProdutosExist_ShouldReturnMatchingProdutos()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            CreateProduto(1, "Café Expresso", "Marca A", 12.50m, "Mercado 1"),
            CreateProduto(2, "Café Tradicional", "Marca B", 8.75m, "Mercado 2")
        };

        _mockRepository.Setup(x => x.GetByNomeAsync("café")).ReturnsAsync(produtos);

        // Act
        var result = await _produtoService.BuscarProdutosPorNomeAsync("café");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Nome.Should().Contain("Café"));
        _mockRepository.Verify(x => x.GetByNomeAsync("café"), Times.Once);
    }

    [Fact]
    public async Task BuscarProdutosPorNomeAsync_WhenNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByNomeAsync("inexistente")).ReturnsAsync(new List<Produto>());

        // Act
        var result = await _produtoService.BuscarProdutosPorNomeAsync("inexistente");

        // Assert
        result.Should().BeEmpty();
        _mockRepository.Verify(x => x.GetByNomeAsync("inexistente"), Times.Once);
    }

    [Fact]
    public async Task GetProdutosPorMercadoAsync_WhenProdutosExist_ShouldReturnMatchingProdutos()
    {
        // Arrange
        var produtos = new List<Produto>
        {
            CreateProduto(1, "Produto 1", "Marca A", 10.00m, "Supermercado ABC"),
            CreateProduto(2, "Produto 2", "Marca B", 15.00m, "Supermercado ABC")
        };

        _mockRepository.Setup(x => x.GetByMercadoAsync("Supermercado ABC")).ReturnsAsync(produtos);

        // Act
        var result = await _produtoService.GetProdutosPorMercadoAsync("Supermercado ABC");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Mercado.Should().Be("Supermercado ABC"));
        _mockRepository.Verify(x => x.GetByMercadoAsync("Supermercado ABC"), Times.Once);
    }

    [Fact]
    public async Task GetProdutosPorMercadoAsync_WhenNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByMercadoAsync("Mercado Inexistente")).ReturnsAsync(new List<Produto>());

        // Act
        var result = await _produtoService.GetProdutosPorMercadoAsync("Mercado Inexistente");

        // Assert
        result.Should().BeEmpty();
        _mockRepository.Verify(x => x.GetByMercadoAsync("Mercado Inexistente"), Times.Once);
    }

    [Fact]
    public async Task CriarProdutoAsync_ShouldCreateAndReturnProduto()
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

        var produtoCriado = CreateProduto(1, dto.Nome, dto.Marca, dto.PrecoAtual, dto.Mercado);
        produtoCriado.Url = dto.Url;
        produtoCriado.UrlImagem = dto.UrlImagem;

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Produto>())).ReturnsAsync(produtoCriado);

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
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Produto>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarProdutoAsync_WhenProdutoExists_ShouldUpdateAndReturnTrue()
    {
        // Arrange
        var produtoExistente = CreateProduto(1, "Produto Original", "Marca Original", 10.00m, "Mercado Original");
        var dto = new CriarProdutoDto
        {
            Nome = "Produto Atualizado",
            Marca = "Marca Atualizada",
            PrecoAtual = 20.00m,
            Mercado = "Mercado Atualizado",
            Url = "https://example.com/atualizado",
            UrlImagem = "https://example.com/imagem-atualizada.jpg"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(produtoExistente);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Produto>())).Returns(Task.CompletedTask);

        // Act
        var result = await _produtoService.AtualizarProdutoAsync(1, dto);

        // Assert
        result.Should().BeTrue();
        produtoExistente.Nome.Should().Be(dto.Nome);
        produtoExistente.Marca.Should().Be(dto.Marca);
        produtoExistente.PrecoAtual.Should().Be(dto.PrecoAtual);
        produtoExistente.Mercado.Should().Be(dto.Mercado);
        produtoExistente.Url.Should().Be(dto.Url);
        produtoExistente.UrlImagem.Should().Be(dto.UrlImagem);
        _mockRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(produtoExistente), Times.Once);
    }

    [Fact]
    public async Task AtualizarProdutoAsync_WhenProdutoDoesNotExist_ShouldReturnFalse()
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

        _mockRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Produto?)null);

        // Act
        var result = await _produtoService.AtualizarProdutoAsync(999, dto);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task DeletarProdutoAsync_WhenProdutoExists_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var produto = CreateProduto(1, "Produto para Deletar", "Marca", 10.00m, "Mercado");
        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(produto);
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<Produto>())).Returns(Task.CompletedTask);

        // Act
        var result = await _produtoService.DeletarProdutoAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(x => x.DeleteAsync(produto), Times.Once);
    }

    [Fact]
    public async Task DeletarProdutoAsync_WhenProdutoDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Produto?)null);

        // Act
        var result = await _produtoService.DeletarProdutoAsync(999);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task BuscarProdutosPorNomeAsync_WithInvalidNome_ShouldCallRepositoryWithOriginalValue(string nome)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByNomeAsync(nome)).ReturnsAsync(new List<Produto>());

        // Act
        await _produtoService.BuscarProdutosPorNomeAsync(nome);

        // Assert
        _mockRepository.Verify(x => x.GetByNomeAsync(nome), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetProdutosPorMercadoAsync_WithInvalidMercado_ShouldCallRepositoryWithOriginalValue(string mercado)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByMercadoAsync(mercado)).ReturnsAsync(new List<Produto>());

        // Act
        await _produtoService.GetProdutosPorMercadoAsync(mercado);

        // Assert
        _mockRepository.Verify(x => x.GetByMercadoAsync(mercado), Times.Once);
    }

    private static Produto CreateProduto(int id, string nome, string marca, decimal preco, string mercado)
    {
        return new Produto
        {
            Id = id,
            Nome = nome,
            Marca = marca,
            PrecoAtual = preco,
            Mercado = mercado,
            Url = $"https://example.com/produto{id}",
            UrlImagem = $"https://example.com/imagem{id}.jpg",
            CriadoEm = DateTime.UtcNow
        };
    }
}

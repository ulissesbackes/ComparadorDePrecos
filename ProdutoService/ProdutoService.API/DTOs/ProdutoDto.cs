namespace ProdutoService.API.DTOs;

public class ProdutoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public decimal PrecoAtual { get; set; }
    public string Mercado { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? UrlImagem { get; set; }
    public DateTime CriadoEm { get; set; }
}

public class CriarProdutoDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public decimal PrecoAtual { get; set; }
    public string Mercado { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? UrlImagem { get; set; }
}
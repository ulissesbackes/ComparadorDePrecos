namespace ProdutoService.Domain.Models;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public decimal PrecoAtual { get; set; }
    public string Mercado { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? UrlImagem { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public List<ListaItem> ListaItens { get; set; } = new();
}
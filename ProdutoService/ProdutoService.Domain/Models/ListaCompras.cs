namespace ProdutoService.Domain.Models;

public class ListaCompras
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public List<ListaItem> Itens { get; set; } = new();
}

public class ListaItem
{
    public int ListaId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; } = 1;
    public DateTime AdicionadoEm { get; set; } = DateTime.UtcNow;

    public ListaCompras Lista { get; set; } = null!;
    public Produto Produto { get; set; } = null!;
}
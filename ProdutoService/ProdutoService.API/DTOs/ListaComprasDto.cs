namespace ProdutoService.API.DTOs;

public class ListaComprasDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public DateTime CriadaEm { get; set; }
    public List<ListaItemDto> Itens { get; set; } = new();
}

public class CriarListaComprasDto
{
    public string Nome { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
}

public class ListaItemDto
{
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public decimal ProdutoPreco { get; set; }
    public int Quantidade { get; set; }
    public DateTime AdicionadoEm { get; set; }
}

public class AdicionarItemListaDto
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; } = 1;
}
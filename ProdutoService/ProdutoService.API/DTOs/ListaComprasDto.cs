using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome não pode exceder 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "UsuarioId é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "UsuarioId deve ser maior que 0")]
    public int UsuarioId { get; set; }
}

public class AtualizarListaComprasDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome não pode exceder 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
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
    [Required(ErrorMessage = "ProdutoId é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ProdutoId deve ser maior que 0")]
    public int ProdutoId { get; set; }

    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(1, 100, ErrorMessage = "Quantidade deve ser entre 1 e 100")]
    public int Quantidade { get; set; } = 1;
}
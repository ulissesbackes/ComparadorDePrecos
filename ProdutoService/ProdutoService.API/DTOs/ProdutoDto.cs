using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome não pode exceder 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Marca não pode exceder 50 caracteres")]
    public string? Marca { get; set; }

    [Required(ErrorMessage = "Preço é obrigatório")]
    [Range(0.01, 10000, ErrorMessage = "Preço deve ser entre 0.01 e 10000")]
    public decimal PrecoAtual { get; set; }

    [Required(ErrorMessage = "Mercado é obrigatório")]
    [StringLength(50, ErrorMessage = "Mercado não pode exceder 50 caracteres")]
    public string Mercado { get; set; } = string.Empty;

    [Required(ErrorMessage = "URL é obrigatória")]
    [Url(ErrorMessage = "URL deve ser válida")]
    public string Url { get; set; } = string.Empty;

    [Url(ErrorMessage = "URL da imagem deve ser válida")]
    public string? UrlImagem { get; set; }
}
namespace ComparadorDePrecos.Models;

public class Produto
{
    public string Nome { get; set; } = "";
    public decimal Preco { get; set; }
    public decimal PrecoOriginal { get; set; }
    public string Mercado { get; set; } = "";
    public string Url { get; set; } = "";
    public string Imagem { get; set; } = "";
}
using System.Net;
using ComparadorDePrecos.Models;
using Microsoft.Playwright;

namespace ComparadorDePrecos.Providers;

public class MinhacooperProvider : IMarketProvider, IAsyncDisposable
{
    public string Nome => "Minhacooper";

    private readonly HttpClient _http;
    private IPlaywright _playwright;
    private IBrowser _browser;
    private bool _playwrightInitialized = false;

    public MinhacooperProvider(HttpClient httpClient)
    {
        _http = httpClient;
        _http.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

    public async Task<List<Produto>> BuscarProdutosAsync(string termo)
    {
        Console.WriteLine($"🔍 Minhacooper: Buscando '{termo}'");

        var produtos = await BuscarComPlaywright(termo);

        if (produtos.Any())
        {
            Console.WriteLine($"✅ Minhacooper: Encontrados {produtos.Count} produtos");
            return produtos;
        }

        Console.WriteLine($"❌ Minhacooper: Nenhum produto encontrado");
        return new List<Produto>();
    }

    private async Task<List<Produto>> BuscarComPlaywright(string termo)
    {
        IPage page = null;

        try
        {
            await InitializePlaywright();
            page = await _browser.NewPageAsync();
            page.SetDefaultTimeout(30000);

            var url = $"https://minhacooper.com.br/loja/v.nova-bnu/produto/busca?q={WebUtility.UrlEncode(termo)}";
            Console.WriteLine($"🌐 Navegando para: {url}");

            var response = await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            if (response?.Status != 200)
            {
                Console.WriteLine($"❌ HTTP Error: {response?.Status}");
                return new List<Produto>();
            }

            await page.WaitForTimeoutAsync(3000);

            var content = await page.ContentAsync();
            if (content.Contains("Desculpe, não encontramos resultado"))
            {
                Console.WriteLine("❌ Mensagem 'não encontrado' detectada");
                return new List<Produto>();
            }

            var produtos = await ExtrairProdutosMinhacooper(page);

            if (!produtos.Any())
            {
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
                await page.WaitForTimeoutAsync(2000);
                produtos = await ExtrairProdutosMinhacooper(page);
            }

            return produtos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro Playwright: {ex.Message}");
            return new List<Produto>();
        }
        finally
        {
            if (page != null)
                await page.CloseAsync();
        }
    }

    private async Task<List<Produto>> ExtrairProdutosMinhacooper(IPage page)
    {
        var produtos = new List<Produto>();

        try
        {
            var result = await page.EvaluateAsync<string>(@"
                () => {
                    const produtos = [];
                    const seen = new Set();
                    
                    const productElements = document.querySelectorAll('.product-list-item, .product-variation');
                    
                    console.log('Encontrados ' + productElements.length + ' elementos de produto');
                    
                    productElements.forEach(productEl => {
                        const nameEl = productEl.querySelector('.product-variation__name');
                        const nome = nameEl ? nameEl.innerText.trim() : '';
                        
                        const priceEl = productEl.querySelector('.product-variation__final-price');
                        const preco = priceEl ? priceEl.innerText.trim() : '';
                        
                        const imgEl = productEl.querySelector('.product-variation__image');
                        let imagem = '';
                        if (imgEl) {
                            imagem = imgEl.getAttribute('src') || '';
                            if (imagem.startsWith('//')) {
                                imagem = 'https:' + imagem;
                            }
                        }
                        
                        const linkEl = productEl.querySelector('a[href]');
                        const url = linkEl ? linkEl.href : '';
                        
                        if (nome && preco && preco.includes('R$')) {
                            const chave = nome + '|' + preco;
                            
                            if (!seen.has(chave)) {
                                seen.add(chave);
                                produtos.push({
                                    nome: nome,
                                    preco: preco,
                                    url: url,
                                    imagem: imagem
                                });
                            }
                        }
                    });
                    
                    return JSON.stringify(produtos);
                }
            ");

            produtos = ProcessarResultadoMinhacooper(result);

            var produtosComImagem = produtos.Count(p => !string.IsNullOrEmpty(p.Imagem));
            Console.WriteLine($"📊 Produtos encontrados: {produtos.Count} ({produtosComImagem} com imagem)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro extração Minhacooper: {ex.Message}");
        }

        return produtos;
    }

    private List<Produto> ProcessarResultadoMinhacooper(string jsonResult)
    {
        var produtos = new List<Produto>();

        if (string.IsNullOrEmpty(jsonResult))
            return produtos;

        try
        {
            var produtosJson = System.Text.Json.JsonSerializer.Deserialize<List<ProdutoJson>>(jsonResult);

            foreach (var p in produtosJson)
            {
                var preco = ParsePreco(p.preco);
                var nome = p.nome?.Trim();

                if (preco > 0 && !string.IsNullOrEmpty(nome))
                {
                    var urlCompleta = p.url;
                    if (!string.IsNullOrEmpty(urlCompleta) && !urlCompleta.StartsWith("http"))
                    {
                        urlCompleta = "https://minhacooper.com.br" + urlCompleta;
                    }

                    var imagemCompleta = p.imagem;
                    if (!string.IsNullOrEmpty(imagemCompleta) && !imagemCompleta.StartsWith("http"))
                    {
                        if (imagemCompleta.StartsWith("//"))
                            imagemCompleta = "https:" + imagemCompleta;
                        else if (imagemCompleta.StartsWith("/"))
                            imagemCompleta = "https://minhacooper.com.br" + imagemCompleta;
                    }

                    produtos.Add(new Produto
                    {
                        Nome = nome,
                        Preco = preco,
                        Mercado = Nome,
                        Url = urlCompleta,
                        Imagem = imagemCompleta
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro processar JSON Minhacooper: {ex.Message}");
        }

        return produtos;
    }

    private decimal ParsePreco(string textoPreco)
    {
        if (string.IsNullOrWhiteSpace(textoPreco))
            return 0;

        try
        {
            var match = System.Text.RegularExpressions.Regex.Match(textoPreco, @"[\d.,]+");
            if (match.Success)
            {
                var numero = match.Value
                    .Replace(".", "")
                    .Replace(",", ".");

                if (decimal.TryParse(numero, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal preco))
                {
                    return preco;
                }
            }
        }
        catch
        {
            // Ignora erro de parse
        }

        return 0;
    }

    private async Task InitializePlaywright()
    {
        if (!_playwrightInitialized)
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Timeout = 30000
            });
            _playwrightInitialized = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser != null)
            await _browser.DisposeAsync();
        _playwright?.Dispose();
    }

    // Classe auxiliar para desserialização JSON
    private class ProdutoJson
    {
        public string nome { get; set; }
        public string preco { get; set; }
        public string url { get; set; }
        public string imagem { get; set; }
    }
}
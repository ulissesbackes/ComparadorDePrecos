using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ComparadorDePrecos.Models;

namespace ComparadorDePrecos.Providers;

public class AngeloniProvider : IMarketProvider
{
    public string Nome => "Angeloni";

    private readonly HttpClient _http;

    public AngeloniProvider(HttpClient httpClient)
    {
        _http = httpClient;

        // Configurar headers para evitar bloqueios
        _http.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        _http.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<List<Produto>> BuscarProdutosAsync(string termo)
    {
        var produtos = new List<Produto>();

        try
        {
            var url = ConstruirUrlGraphQL(termo);
            var response = await _http.GetStringAsync(url);

            produtos = ProcessarRespostaGraphQL(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar produtos no Angeloni: {ex.Message}");
        }

        return produtos;
    }

    private string ConstruirUrlGraphQL(string termo)
    {
        // Parâmetros base fixos
        var baseParams = new Dictionary<string, string>
        {
            ["workspace"] = "master",
            ["maxAge"] = "short",
            ["appsEtag"] = "remove",
            ["domain"] = "store",
            ["locale"] = "pt-BR",
            ["__bindingId"] = "d44a2c5b-9d10-4104-85cb-7ae32e06f776",
            ["operationName"] = "productSearchV3",
            ["variables"] = "{}"
        };

        // Construir as extensions com o termo de busca
        var extensions = new
        {
            persistedQuery = new
            {
                version = 1,
                sha256Hash = "efcfea65b452e9aa01e820e140a5b4a331adfce70470d2290c08bc4912b45212",
                sender = "vtex.store-resources@0.x",
                provider = "vtex.search-graphql@0.x"
            },
            variables = new
            {
                hideUnavailableItems = false,
                skusFilter = "FIRST_AVAILABLE",
                simulationBehavior = "default",
                installmentCriteria = "MAX_WITHOUT_INTEREST",
                productOriginVtex = false,
                map = "ft",
                query = termo,
                orderBy = "OrderByScoreDESC",
                from = 0,
                to = 50, // Número de produtos a retornar
                selectedFacets = new[]
                {
                    new { key = "ft", value = termo }
                },
                fullText = termo,
                facetsBehavior = "Static",
                categoryTreeBehavior = "default",
                withFacets = false,
                variant = "68308769ced074d095f0a8e5-variantTreatment",
                advertisementOptions = new
                {
                    showSponsored = true,
                    sponsoredCount = 3,
                    advertisementPlacement = "top_search",
                    repeatSponsoredProducts = true
                }
            }
        };

        // Serializar e codificar as extensions
        var extensionsJson = JsonSerializer.Serialize(extensions, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var variablesBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(extensions.variables, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        ));

        baseParams["extensions"] = JsonSerializer.Serialize(new
        {
            persistedQuery = extensions.persistedQuery,
            variables = variablesBase64
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // Construir URL final
        var queryString = string.Join("&", baseParams.Select(p =>
            $"{p.Key}={Uri.EscapeDataString(p.Value)}"));

        return $"https://www.angeloni.com.br/super/_v/segment/graphql/v1?{queryString}";
    }

    private List<Produto> ProcessarRespostaGraphQL(string responseJson)
    {
        var produtos = new List<Produto>();

        try
        {
            using var doc = JsonDocument.Parse(responseJson);

            // Navegar até a lista de produtos
            if (doc.RootElement.TryGetProperty("data", out var data) &&
                data.TryGetProperty("productSearch", out var productSearch) &&
                productSearch.TryGetProperty("products", out var productsArray))
            {
                foreach (var product in productsArray.EnumerateArray())
                {
                    var produto = ExtrairProdutoDoJson(product);
                    if (produto != null)
                    {
                        produtos.Add(produto);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar resposta GraphQL: {ex.Message}");
        }

        return produtos;
    }

    private Produto? ExtrairProdutoDoJson(JsonElement product)
    {
        try
        {
            // Extrair informações básicas do produto
            var nome = product.GetProperty("productName").GetString() ?? "";
            var link = product.GetProperty("link").GetString() ?? "";
            var brand = product.GetProperty("brand").GetString() ?? "";

            // Extrair itens (SKUs)
            if (!product.TryGetProperty("items", out var items) || items.GetArrayLength() == 0)
                return null;

            var firstItem = items.EnumerateArray().First();
            
            // Extrair imagem
            var imagem = "";
            if (firstItem.TryGetProperty("images", out var images) && images.GetArrayLength() > 0)
            {
                var firstImage = images.EnumerateArray().First();
                imagem = firstImage.GetProperty("imageUrl").GetString() ?? "";
            }

            // Extrair oferta comercial
            if (!firstItem.TryGetProperty("sellers", out var sellers) || sellers.GetArrayLength() == 0)
                return null;

            var firstSeller = sellers.EnumerateArray().First();
            var offer = firstSeller.GetProperty("commertialOffer");

            var price = offer.GetProperty("Price").GetDecimal();
            var listPrice = offer.GetProperty("ListPrice").GetDecimal();
            var available = offer.GetProperty("AvailableQuantity").GetInt32() > 0;

            if (!available) return null;

            return new Produto
            {
                Nome = nome,
                Preco = price,
                PrecoOriginal = listPrice,
                Mercado = Nome,
                Url = $"https://www.angeloni.com.br/super/{link}",
                Imagem = imagem
            };
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Propriedade não encontrada no produto: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao extrair produto: {ex.Message}");
            return null;
        }
    }
}
using ComparadorDePrecos.Providers;
using ComparadorDePrecos.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddHttpClient<AngeloniProvider>();
builder.Services.AddHttpClient<MinhacooperProvider>();
builder.Services.AddMemoryCache(); // ← ADD CACHE

builder.Services.AddSingleton<IMarketProvider, AngeloniProvider>();
builder.Services.AddSingleton<IMarketProvider, MinhacooperProvider>();

builder.Services.AddScoped<ComparadorService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // URL do React
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("ReactApp");

app.MapGet("/", () => "Comparador de Preços API ativa!");

// Busca geral (mantém compatibilidade)
app.MapGet("/produtos/{termo}", async (string termo, ComparadorService service) =>
{
    var produtos = await service.BuscarProdutosAsync(termo);
    return Results.Ok(produtos);
});

// NOVOS ENDPOINTS - Busca por mercado específico
app.MapGet("/produtos/{mercado}/{termo}", async (string mercado, string termo, ComparadorService service) =>
{
    var produtos = await service.BuscarProdutosPorMercadoAsync(termo, mercado);
    return Results.Ok(produtos);
});

// Lista todos os mercados disponíveis
app.MapGet("/mercados", (ComparadorService service) =>
{
    var mercados = service.ObterMercadosDisponiveis();
    return Results.Ok(mercados);
});

app.Run();
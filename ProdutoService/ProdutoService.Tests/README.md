# ProdutoService Tests

Este projeto contém os testes xUnit para o ProdutoService, incluindo testes unitários e de integração.

## Estrutura dos Testes

### Testes Unitários (`Services/ProdutoServiceTests.cs`)
- **GetAllProdutosAsync**: Testa a recuperação de todos os produtos
- **GetProdutoByIdAsync**: Testa a busca por ID (casos de sucesso e falha)
- **BuscarProdutosPorNomeAsync**: Testa a busca por nome (case-insensitive)
- **GetProdutosPorMercadoAsync**: Testa a busca por mercado
- **CriarProdutoAsync**: Testa a criação de novos produtos
- **AtualizarProdutoAsync**: Testa a atualização de produtos existentes
- **DeletarProdutoAsync**: Testa a exclusão de produtos

### Testes de Integração (`Services/ProdutoServiceIntegrationTests.cs`)
- Testes que usam banco de dados em memória (InMemory)
- Verificam a persistência real dos dados
- Testam cenários de busca case-insensitive

### Testes do Controller (`Controllers/ProdutosControllerTests.cs`)
- Testam os endpoints da API
- Verificam os códigos de resposta HTTP
- Testam cenários de sucesso e erro

## Tecnologias Utilizadas

- **xUnit**: Framework de testes
- **Moq**: Biblioteca para mocking de dependências
- **FluentAssertions**: Biblioteca para assertions mais legíveis
- **Entity Framework InMemory**: Para testes de integração

## Como Executar os Testes

### Via Visual Studio
1. Abra o Solution no Visual Studio
2. Vá em Test > Test Explorer
3. Execute todos os testes ou selecione testes específicos

### Via Linha de Comando
```bash
# Navegar para o diretório do projeto
cd ProdutoService.Tests

# Executar todos os testes
dotnet test

# Executar com detalhes
dotnet test --verbosity normal

# Executar apenas testes unitários
dotnet test --filter "Category=Unit"

# Executar apenas testes de integração
dotnet test --filter "Category=Integration"
```

### Via Docker (se aplicável)
```bash
# Se estiver usando Docker
docker-compose -f docker-compose.yml up --build
```

## Cobertura de Testes

Os testes cobrem:
- ✅ Todos os métodos do ProdutoService
- ✅ Cenários de sucesso e falha
- ✅ Validação de dados de entrada
- ✅ Persistência no banco de dados
- ✅ Endpoints da API
- ✅ Códigos de resposta HTTP

## Estrutura dos Arquivos

```
ProdutoService.Tests/
├── Services/
│   ├── ProdutoServiceTests.cs           # Testes unitários
│   └── ProdutoServiceIntegrationTests.cs # Testes de integração
├── Controllers/
│   └── ProdutosControllerTests.cs        # Testes do controller
├── ProdutoService.Tests.csproj          # Configuração do projeto
└── README.md                            # Este arquivo
```

## Padrões de Teste

### Arrange-Act-Assert (AAA)
Todos os testes seguem o padrão AAA:
- **Arrange**: Configuração dos dados e mocks
- **Act**: Execução do método sendo testado
- **Assert**: Verificação dos resultados

### Nomenclatura
- Métodos de teste: `MethodName_Scenario_ExpectedResult`
- Exemplo: `GetProdutoByIdAsync_WhenProdutoExists_ShouldReturnProduto`

### Mocking
- Uso do Moq para simular dependências
- Verificação de chamadas com `Verify()`
- Configuração de retornos com `Setup()`

## Exemplo de Teste

```csharp
[Fact]
public async Task GetProdutoByIdAsync_WhenProdutoExists_ShouldReturnProduto()
{
    // Arrange
    var produto = CreateProduto(1, "Produto Teste", "Marca", 25.99m, "Mercado");
    _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(produto);

    // Act
    var result = await _produtoService.GetProdutoByIdAsync(1);

    // Assert
    result.Should().NotBeNull();
    result!.Nome.Should().Be("Produto Teste");
    _mockRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
}
```

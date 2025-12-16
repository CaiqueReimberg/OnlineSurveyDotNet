# Testes da API - Infnet.OnlineSurveys.Api.Tests

Este projeto contém testes unitários completos para os controllers da API de Pesquisas Online.

## ?? Estrutura dos Testes

```
Infnet.OnlineSurveys.Api.Tests/
??? Controllers/
?   ??? SurveysControllerTests.cs      # 18 testes
?   ??? ResponsesControllerTests.cs    # 10 testes
??? README.md
```

## ?? Frameworks e Bibliotecas

- **xUnit**: Framework de testes
- **Moq**: Framework para mocking de dependências
- **.NET 9**: Plataforma de execução

## ?? Cobertura de Testes

### SurveysControllerTests (18 testes)

#### Testes de Consulta (GET)
- ? `GetAll_ShouldReturnAllSurveys` - Retorna todas as pesquisas
- ? `GetById_WithValidId_ShouldReturnSurvey` - Retorna pesquisa por ID válido
- ? `GetById_WithInvalidId_ShouldReturnNotFound` - Retorna 404 para ID inválido
- ? `GetByStatus_WithValidStatus_ShouldReturnSurveys` - Retorna pesquisas por status
- ? `GetByStatus_WithInvalidStatus_ShouldReturnBadRequest` - Retorna 400 para status inválido

#### Testes de Criação (POST)
- ? `Create_WithValidData_ShouldReturnCreatedSurvey` - Cria pesquisa com sucesso
- ? `AddQuestion_WithValidData_ShouldReturnCreated` - Adiciona questão com sucesso
- ? `AddQuestion_WhenSurveyNotFound_ShouldReturnBadRequest` - Retorna erro quando pesquisa não existe

#### Testes de Atualização (PUT)
- ? `Update_WithValidId_ShouldReturnNoContent` - Atualiza pesquisa com sucesso
- ? `Update_WithInvalidId_ShouldReturnNotFound` - Retorna 404 para ID inválido

#### Testes de Exclusão (DELETE)
- ? `Delete_WithValidId_ShouldReturnNoContent` - Exclui pesquisa com sucesso
- ? `Delete_WithInvalidId_ShouldReturnNotFound` - Retorna 404 para ID inválido

#### Testes de Ações Especiais
- ? `Publish_WithValidSurvey_ShouldReturnOk` - Publica pesquisa válida
- ? `Publish_WithoutQuestions_ShouldReturnBadRequest` - Impede publicação sem questões
- ? `Publish_WithInvalidId_ShouldReturnNotFound` - Retorna 404 para ID inválido
- ? `Close_WithValidId_ShouldReturnOk` - Fecha pesquisa com sucesso
- ? `Close_WithInvalidId_ShouldReturnNotFound` - Retorna 404 para ID inválido

### ResponsesControllerTests (10 testes)

#### Testes de Consulta (GET)
- ? `GetAll_ShouldReturnAllResponses` - Retorna todas as respostas
- ? `GetById_WithValidId_ShouldReturnResponse` - Retorna resposta por ID válido
- ? `GetById_WithInvalidId_ShouldReturnNotFound` - Retorna 404 para ID inválido
- ? `GetBySurveyId_WithValidId_ShouldReturnResponses` - Retorna respostas de uma pesquisa
- ? `GetBySurveyId_WithInvalidId_ShouldReturnNotFound` - Retorna 404 para pesquisa inválida
- ? `GetByEmail_WithValidEmail_ShouldReturnResponses` - Retorna respostas por email

#### Testes de Criação (POST)
- ? `Create_WithValidData_ShouldReturnCreated` - Cria resposta com sucesso
- ? `Create_WithInvalidSurveyId_ShouldReturnNotFound` - Retorna 404 para pesquisa inválida
- ? `Create_WithUnpublishedSurvey_ShouldReturnBadRequest` - Impede resposta em pesquisa não publicada

#### Testes de Exclusão (DELETE)
- ? `Delete_WithValidId_ShouldReturnNoContent` - Exclui resposta com sucesso
- ? `Delete_WithInvalidId_ShouldReturnNotFound` - Retorna 404 para ID inválido

## ?? Como Executar os Testes

### Executar todos os testes do projeto:
```bash
dotnet test Infnet.OnlineSurveys.Api.Tests\Infnet.OnlineSurveys.Api.Tests.csproj
```

### Executar com detalhes:
```bash
dotnet test Infnet.OnlineSurveys.Api.Tests\Infnet.OnlineSurveys.Api.Tests.csproj --logger "console;verbosity=detailed"
```

### Executar com cobertura de código:
```bash
dotnet test Infnet.OnlineSurveys.Api.Tests\Infnet.OnlineSurveys.Api.Tests.csproj --collect:"XPlat Code Coverage"
```

### Executar apenas testes de um controller específico:
```bash
# Apenas SurveysController
dotnet test --filter FullyQualifiedName~SurveysControllerTests

# Apenas ResponsesController
dotnet test --filter FullyQualifiedName~ResponsesControllerTests
```

## ?? Padrões de Teste Utilizados

### Arrange-Act-Assert (AAA)
Todos os testes seguem o padrão AAA:
- **Arrange**: Configuração dos mocks e dados de teste
- **Act**: Execução do método sendo testado
- **Assert**: Verificação dos resultados

### Exemplo:
```csharp
[Fact]
public async Task GetById_WithValidId_ShouldReturnSurvey()
{
    // Arrange
    var surveyId = Guid.NewGuid();
    var survey = new Survey("Test Survey");
    _mockSurveyRepository.Setup(repo => repo.GetByIdWithQuestionsAsync(surveyId))
        .ReturnsAsync(survey);

    // Act
    var result = await _controller.GetById(surveyId);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var returnedSurvey = Assert.IsType<SurveyDto>(okResult.Value);
    Assert.Equal("Test Survey", returnedSurvey.Title);
}
```

## ?? Cenários Testados

### Cenários de Sucesso
- Operações CRUD básicas
- Validações de regras de negócio
- Transformações de dados (Entity ? DTO)
- Ações especiais (Publish, Close)

### Cenários de Erro
- Entidades não encontradas (404)
- Dados inválidos (400)
- Violações de regras de negócio
- Estados inválidos

## ?? Resultados dos Testes

```
Resumo do teste:
??? Total: 28
??? Falhou: 0
??? Bem-sucedido: 28 ?
??? Ignorado: 0
??? Duração: ~2.2s
```

## ?? Mocks Utilizados

Os testes utilizam mocks para isolar as dependências:

- `Mock<ISurveyRepository>` - Repositório de pesquisas
- `Mock<IResponseRepository>` - Repositório de respostas
- `Mock<ILogger<T>>` - Logger da aplicação

Isso permite testar apenas a lógica dos controllers sem dependências externas.

## ?? Boas Práticas Implementadas

1. ? **Isolamento**: Cada teste é independente
2. ? **Nomenclatura Clara**: Nomes descritivos indicam o que está sendo testado
3. ? **Cobertura Completa**: Testa cenários de sucesso e falha
4. ? **Mocks Apropriados**: Usa mocks apenas para dependências externas
5. ? **Assertions Específicas**: Verifica tipos e valores específicos
6. ? **Sem Dependências Externas**: Não requer banco de dados ou serviços externos

## ?? Aprendizados

### Testando Controllers com ASP.NET Core
- Como mockar repositórios e serviços
- Como validar ActionResults (OkResult, NotFoundResult, etc.)
- Como testar métodos assíncronos

### Usando Moq
- Setup de métodos com `Setup()` e `ReturnsAsync()`
- Verificação de parâmetros com `It.IsAny<T>()`
- Simulação de exceções com `ThrowsAsync()`

### xUnit
- Atributo `[Fact]` para testes simples
- Padrão AAA (Arrange-Act-Assert)
- Assertions específicas (`IsType`, `Equal`, `Single`, etc.)

## ?? Manutenção

Para adicionar novos testes:

1. Crie um novo método com atributo `[Fact]`
2. Siga o padrão AAA
3. Use nomenclatura descritiva: `NomeDoMetodo_Cenario_ResultadoEsperado`
4. Configure os mocks necessários
5. Execute os testes para validar

## ?? Suporte

Para dúvidas sobre os testes, consulte:
- [Documentação xUnit](https://xunit.net/)
- [Documentação Moq](https://github.com/moq/moq4)
- [Documentação ASP.NET Core Testing](https://docs.microsoft.com/aspnet/core/test/)

---

**Desenvolvido com ?? para garantir a qualidade do código**

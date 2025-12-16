# Testes de Infraestrutura - Infnet.OnlineSurveys.Infrastructure.Data.Tests

Este projeto contém testes de integração completos para os repositórios da camada de infraestrutura.

## ?? Estrutura dos Testes

```
Infnet.OnlineSurveys.Infrastructure.Data.Tests/
??? Repositories/
?   ??? SurveyRepositoryTests.cs      # 13 testes
?   ??? ResponseRepositoryTests.cs    # 12 testes
??? README.md
```

## ?? Frameworks e Bibliotecas

- **xUnit**: Framework de testes
- **Entity Framework Core InMemory**: Banco de dados em memória para testes
- **.NET 9**: Plataforma de execução

## ?? Cobertura de Testes

### SurveyRepositoryTests (13 testes)

#### Testes CRUD Básico
- ? `AddAsync_ShouldAddSurveyToDatabase` - Adiciona pesquisa ao banco
- ? `GetByIdAsync_WithValidId_ShouldReturnSurvey` - Busca por ID válido
- ? `GetByIdAsync_WithInvalidId_ShouldReturnNull` - Retorna null para ID inválido
- ? `GetAllAsync_ShouldReturnAllSurveys` - Retorna todas as pesquisas
- ? `UpdateAsync_ShouldSaveChanges` - Salva alterações
- ? `DeleteAsync_ShouldRemoveSurveyFromDatabase` - Remove pesquisa

#### Testes de Consulta Específica
- ? `GetByStatusAsync_ShouldReturnSurveysWithMatchingStatus` - Filtra por status
- ? `GetByIdWithQuestionsAsync_ShouldReturnSurveyWithQuestions` - Carrega com relacionamentos

#### Testes de Funcionalidades Especiais
- ? `AddQuestionToSurveyAsync_ShouldAddQuestionSuccessfully` - Adiciona questão
- ? `AddQuestionToSurveyAsync_WithInvalidSurveyId_ShouldThrowException` - Valida ID
- ? `AddQuestionToSurveyAsync_ToPublishedSurvey_ShouldThrowException` - Valida status

#### Testes de Relacionamentos
- ? Verifica se Questions são salvas com Options
- ? Verifica se Include() funciona corretamente
- ? Verifica shadow properties (SurveyId em Question)

### ResponseRepositoryTests (12 testes)

#### Testes CRUD Básico
- ? `AddAsync_ShouldAddResponseToDatabase` - Adiciona resposta ao banco
- ? `GetByIdAsync_WithValidId_ShouldReturnResponse` - Busca por ID válido
- ? `GetByIdAsync_WithInvalidId_ShouldReturnNull` - Retorna null para ID inválido
- ? `GetAllAsync_ShouldReturnAllResponsesWithAnswers` - Retorna todas com answers
- ? `DeleteAsync_ShouldRemoveResponseFromDatabase` - Remove resposta

#### Testes de Consulta Específica
- ? `GetBySurveyIdAsync_ShouldReturnResponsesForSpecificSurvey` - Filtra por survey
- ? `GetByIdWithAnswersAsync_ShouldReturnResponseWithAnswers` - Carrega com answers
- ? `GetByEmailAsync_ShouldReturnResponsesForSpecificEmail` - Filtra por email

#### Testes de Relacionamentos
- ? `AddAsync_WithMultipleAnswers_ShouldSaveAllAnswers` - Salva múltiplos answers
- ? Verifica se shadow property ResponseId é definida corretamente

#### Testes de Casos Extremos
- ? `GetBySurveyIdAsync_WithNoResponses_ShouldReturnEmptyList` - Lista vazia
- ? `GetByEmailAsync_WithNoResponses_ShouldReturnEmptyList` - Lista vazia

## ?? Como Executar os Testes

### Executar todos os testes do projeto:
```bash
dotnet test Infnet.OnlineSurveys.Infrastructure.Data.Tests\Infnet.OnlineSurveys.Infrastructure.Data.Tests.csproj
```

### Executar com detalhes:
```bash
dotnet test Infnet.OnlineSurveys.Infrastructure.Data.Tests\Infnet.OnlineSurveys.Infrastructure.Data.Tests.csproj --logger "console;verbosity=detailed"
```

### Executar apenas testes de um repositório específico:
```bash
# Apenas SurveyRepository
dotnet test --filter FullyQualifiedName~SurveyRepositoryTests

# Apenas ResponseRepository
dotnet test --filter FullyQualifiedName~ResponseRepositoryTests
```

## ?? Padrões de Teste Utilizados

### Arrange-Act-Assert (AAA)
Todos os testes seguem o padrão AAA:
- **Arrange**: Configuração dos dados e contexto
- **Act**: Execução do método sendo testado
- **Assert**: Verificação dos resultados no banco

### Exemplo:
```csharp
[Fact]
public async Task AddAsync_ShouldAddSurveyToDatabase()
{
    // Arrange
    var survey = new Survey("Test Survey");

    // Act
    await _repository.AddAsync(survey);

    // Assert
    var savedSurvey = await _context.Surveys.FindAsync(survey.Id);
    Assert.NotNull(savedSurvey);
    Assert.Equal("Test Survey", savedSurvey.Title);
}
```

### Padrão IDisposable
Cada classe de teste implementa `IDisposable` para limpar o banco de dados:

```csharp
public class SurveyRepositoryTests : IDisposable
{
    public SurveyRepositoryTests()
    {
        // Cria novo contexto com banco único para cada teste
        var options = new DbContextOptionsBuilder<OnlineSurveysDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new OnlineSurveysDbContext(options);
    }

    public void Dispose()
    {
        // Limpa o banco após cada teste
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

## ?? Características dos Testes

### Testes de Integração
- ? Usa banco de dados real (InMemory)
- ? Testa integração com Entity Framework
- ? Verifica relacionamentos e navegação
- ? Valida configurações do DbContext

### Isolamento de Testes
- ? Cada teste usa banco de dados separado
- ? Dados são limpos após cada teste
- ? Testes podem rodar em qualquer ordem
- ? Sem dependências entre testes

### Cobertura Completa
- ? Operações CRUD
- ? Consultas com filtros
- ? Relacionamentos (Include/ThenInclude)
- ? Shadow Properties
- ? Validações de regras de negócio
- ? Casos extremos e??

## ?? O que é Testado

### Entity Framework Core
- **Configuração do DbContext**: Verifica se as entidades são mapeadas corretamente
- **Relacionamentos**: Testa OneToMany, cascade delete, shadow properties
- **Include/ThenInclude**: Verifica eager loading de relacionamentos
- **Tracking**: Valida se as mudanças são rastreadas e salvas

### Repositórios
- **Métodos CRUD**: Add, Get, Update, Delete
- **Consultas Específicas**: GetByStatus, GetBySurveyId, GetByEmail
- **Métodos Especiais**: AddQuestionToSurveyAsync
- **Validações**: Verifica se exceções são lançadas corretamente

### Integridade de Dados
- **Persistência**: Dados são salvos corretamente
- **Relacionamentos**: Entidades relacionadas são salvas juntas
- **Shadow Properties**: Chaves estrangeiras são definidas corretamente
- **Cascade Delete**: Exclusões em cascata funcionam

## ?? Resultados Esperados

```
Resumo do teste:
??? Total: 25
??? Falhou: 0
??? Bem-sucedido: 25 ?
??? Ignorado: 0
??? Duração: ~2-3s
```

## ?? Tecnologias Usadas

### Entity Framework Core InMemory
O provedor InMemory é usado para testes porque:
- ? **Rápido**: Executa testes em memória
- ? **Isolado**: Cada teste tem seu próprio banco
- ? **Simples**: Não requer configuração externa
- ? **Confiável**: Comportamento próximo ao banco real

### Diferenças do Banco Real
?? **Atenção**: O InMemory tem algumas limitações:
- Não valida constraints de FK como banco real
- Não suporta transactions distribuídas
- Comportamento de concorrência pode diferir

Para testes mais rigorosos, considere usar **SQLite InMemory** ou containers Docker com bancos reais.

## ?? Boas Práticas Implementadas

1. ? **Isolamento**: Cada teste tem seu próprio banco de dados
2. ? **Limpeza**: Dispose() garante limpeza após cada teste
3. ? **Nomenclatura**: Nomes descritivos no padrão `Method_Scenario_ExpectedResult`
4. ? **Cobertura**: Testa cenários positivos e negativos
5. ? **Verificação Completa**: Assert verifica dados no banco
6. ? **Sem Mocks**: Testa integração real com EF Core

## ?? Aprendizados

### Testando Repositórios com EF Core
- Como configurar DbContext para testes
- Como usar InMemory database
- Como testar relacionamentos
- Como verificar shadow properties

### Entity Framework Core
- Eager loading com Include/ThenInclude
- Shadow properties para chaves estrangeiras
- Change tracking e SaveChanges
- Configurações com Fluent API

### xUnit
- Padrão IDisposable para cleanup
- Fixtures para compartilhar contexto
- Assertions específicas para collections
- Testes assíncronos com async/await

## ?? Manutenção

Para adicionar novos testes:

1. Crie um novo método com atributo `[Fact]`
2. Siga o padrão AAA
3. Use o padrão `Method_Scenario_ExpectedResult` para nomear
4. Adicione entidades ao banco usando os repositórios
5. Verifique os dados diretamente no _context
6. Garanta limpeza no Dispose()

## ?? Exemplos de Cenários Testados

### Teste de Relacionamento
```csharp
[Fact]
public async Task GetByIdWithQuestionsAsync_ShouldReturnSurveyWithQuestions()
{
    // Arrange - Cria survey com questões e opções
    var survey = new Survey("Test Survey");
    var question = new Question("Test Question", 1);
    question.AddOption(new Option("Option 1", 1));
    question.AddOption(new Option("Option 2", 2));
    survey.AddQuestion(question);
    await _repository.AddAsync(survey);

    // Act - Carrega com Include
    var result = await _repository.GetByIdWithQuestionsAsync(survey.Id);

    // Assert - Verifica relacionamentos
    Assert.NotNull(result);
    Assert.Single(result.Questions);
    Assert.Equal(2, result.Questions.First().Options.Count);
}
```

### Teste de Shadow Property
```csharp
[Fact]
public async Task AddQuestionToSurveyAsync_ShouldAddQuestionSuccessfully()
{
    // Arrange
    var survey = new Survey("Test Survey");
    await _repository.AddAsync(survey);
    var question = new Question("New Question", 1);
    question.AddOption(new Option("Option 1", 1));

    // Act - Usa método que define shadow property
    await _repository.AddQuestionToSurveyAsync(survey.Id, question);

    // Assert - Verifica se foi salvo corretamente
    var updatedSurvey = await _repository.GetByIdWithQuestionsAsync(survey.Id);
    Assert.Single(updatedSurvey.Questions);
}
```

## ?? Suporte

Para dúvidas sobre os testes, consulte:
- [Documentação xUnit](https://xunit.net/)
- [Documentação EF Core InMemory](https://docs.microsoft.com/ef/core/providers/in-memory/)
- [Documentação EF Core Testing](https://docs.microsoft.com/ef/core/testing/)

---

**Desenvolvido com ?? para garantir a qualidade da camada de dados**
